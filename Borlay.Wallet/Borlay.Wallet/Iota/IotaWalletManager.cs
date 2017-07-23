using Borlay.Iota.Library;
using Borlay.Iota.Library.Models;
using Borlay.Wallet;
using Borlay.Wallet.Models;
using Borlay.Wallet.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Borlay.Wallet.Iota
{
    public class IotaWalletManager : IWalletManager
    {
        private readonly WalletConfiguration walletConfiguration;
        private readonly WalletModel walletModel;
        private readonly ContentCollectionModel<AddressItemModel> addressesModel;
        private readonly ContentListModel<BundleItemModel> bundlesModel;
        private readonly ObservableCollection<TransactionItemModel> transactionCollection;
        private readonly IconButtonModel[] addressesButtons;
        private readonly ISelectedChanged selectedChanged;
        private readonly ActionCommandGroup commandGroup;


        private readonly IotaWalletTransactionManager transactionManager;

        private bool isFirstTime = true;

        public WalletModel Wallet => walletModel;

        public IotaWalletManager(WalletConfiguration walletConfiguration, ISelectedChanged selectedChanged)
        {
            this.walletConfiguration = walletConfiguration;
            this.selectedChanged = selectedChanged;

            addressesButtons = OpenAddressesButtons().ToArray();
            commandGroup = new ActionCommandGroup(addressesButtons.Select(b => b.ButtonClick).ToArray());
            addressesModel = new ContentCollectionModel<AddressItemModel>(addressesButtons);
            bundlesModel = new ContentListModel<BundleItemModel>(addressesButtons.First());
            transactionCollection = new ObservableCollection<TransactionItemModel>();
            transactionManager = new IotaWalletTransactionManager(transactionCollection, bundlesModel.ContentItems);

            var balanceItems = CreateBalanceItems().ToArray();
            var menuItems = CreateMenuItems().ToArray();

            var balanceStatsModel = new BalanceStatsModel(balanceItems);
            walletModel = new WalletModel(this, balanceStatsModel, menuItems);
            walletModel.NewSend += WalletModel_NewSend;

            menuItems.First().IsSelected = true;
            this.selectedChanged.SelectedChanged += SelectedChanged_SelectedChanged;

            bundlesModel.ContentItems.CollectionChanged += ContentItems_CollectionChanged;
        }

        private void ContentItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in e.NewItems)
            {
                if (item is BundleItemModel bundleModel)
                {
                    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                        bundleModel.Rebroadcast += BundleModel_Rebroadcast;
                    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                        bundleModel.Rebroadcast -= BundleModel_Rebroadcast;
                }
            }
        }

        private async void BundleModel_Rebroadcast(BundleItemModel bundleModel)
        {
            var transactionItems = bundleModel.BundleDetail.TransactionItems.Select(t => t.Tag)
                .OfType<TransactionItem>()
                .OrderBy(t => long.Parse(t.Timestamp)).ToArray();

            if (transactionItems != null && transactionItems.Length > 0)
            {
                var indexString = transactionItems.First().LastIndex;
                var lastIndex = int.Parse(indexString);

                List<TransactionItem> transactions = new List<TransactionItem>();

                for(int i = 0; i <= lastIndex; i++)
                {
                    var iString = i.ToString();
                    var tr = transactionItems.Where(t => t.CurrentIndex == iString).First();
                    transactions.Add(tr);
                }

                await SendTransactions(transactions.ToArray(), true);
            }
        }

        private async void WalletModel_NewSend(WalletModel obj)
        {
            OpenSend();
        }

        private void SelectedChanged_SelectedChanged(object arg1, bool arg2)
        {
            InitializeAsync();
        }

        public async void InitializeAsync(bool force = false)
        {
            if (isFirstTime)
            {
                isFirstTime = false;
                try
                {
                    await walletModel.ScanAddresses.ScanAddressesAsync(force);
                }
                catch(OperationCanceledException)
                {
                    // do nothing
                }
                catch(Exception e)
                {
                    // todo handle in better way
                    MessageBox.Show(e.Message);
                }
            }
        }


        private IEnumerable<BalanceItemModel> CreateBalanceItems()
        {
            yield break;
        }

        private IEnumerable<TabItem> CreateMenuItems()
        {
            yield return new TabItem()
            {
                Name = "Addresses",
                Selected = (t) => 
                    Wallet.View = addressesModel,
                IsSelected = false
            };
            yield return new TabItem()
            {
                Name = "Transactions",
                Selected = (t) => Wallet.View = bundlesModel,
            };
            yield return new TabItem()
            {
                Name = "Paper",
                Selected = (t) => OpenPaper()
            };
        }

        protected IEnumerable<IconButtonModel> OpenAddressesButtons()
        {
            yield return new IconButtonModel(b => RefreshKnowAddressesAsync(), IconType.Restart);
            yield return new IconButtonModel(b => CreateNewAddressAsync(), IconType.Plus);
        }

        public virtual async void OpenPaper()
        {
            Wallet.View = new PaperModel(walletConfiguration.GetSecurePrivateKey());
        }

        public virtual async void OpenDonation()
        {
            if (Wallet.View is DonateModel) return;

            var oldView = Wallet.View;
            var donateModel = new DonateModel(
                (m, a, v) =>
                {
                    try
                    {
                        OpenSendAndSend(a, v);
                        Wallet.View = oldView;
                    }
                    catch(Exception e)
                    {
                        m.ErrorText = e.Message;
                    }
                }, 
                m => Wallet.View = oldView);
            Wallet.View = donateModel;
        }

        public virtual NewSendModel OpenSend(params AddressItemModel[] addressModels)
        {
            return OpenSend(null, null, addressModels);
        }

        public virtual NewSendModel OpenSend(string addressToSend, decimal? amount, params AddressItemModel[] addressModels)
        {
            if (Wallet.View is NewSendModel sm) return sm;

            var oldView = Wallet.View;
            var sendModel = new NewSendModel(Wallet, addressModels, (m) =>
            {
                SendTransfer(m);
            })
            {
                AddressValidation = a => IotaExtensions.ValidateAddress(a),
                Address = addressToSend,
                Value = amount,
            };
            sendModel.Open();
            return sendModel;
        }

        public virtual NewSendModel OpenSendAndSend(string addressToSend, decimal amount, 
            params AddressItemModel[] addressModels)
        {
            if (string.IsNullOrWhiteSpace(addressToSend))
                throw new ArgumentNullException(nameof(addressToSend));

            if (amount <= 0)
                throw new ArgumentException($"Amount should be more than zero", nameof(amount));

            var sendModel = OpenSend(addressToSend, amount, addressModels);
            sendModel.SendCommand.Execute(null);
            return sendModel;
        }

        public async void SendTransfer(NewSendModel sendModel)
        {
            var syncModel = new CancelSyncModel() { Text = "Sending the transaction" };
            walletModel.SyncModels.Add(syncModel);
            try
            {
                await SendTransfer(sendModel, syncModel);
                
            }
            catch(OperationCanceledException)
            {

            }
            catch(Exception e)
            {
                sendModel.ErrorText = e.Message;
                sendModel.Open();
            }
            finally
            {
                walletModel.SyncModels.Remove(syncModel);
                
            }
        }

        public async Task SendTransfer(NewSendModel sendModel, CancelSyncModel syncModel)
        {
            var addressModels = sendModel.Addresses.ToArray();
            if (addressModels == null || addressModels.Length == 0)
                addressModels = addressesModel.GetAddressModels();

            var addressItems = addressModels.Select(a => a.Tag).OfType<AddressItem>().ToArray();
            var api = CreateIotaClient();

            long value = 0;
            if (sendModel.Value.HasValue)
                value = (long)Convert.ChangeType(sendModel.Value.Value, typeof(long)); // long.Parse(sendModel.Value.ToString());

            if (value < 0)
                throw new Exception("Value cannot be less than zero");

            var addressToSend = sendModel.Address;
            addressToSend = IotaExtensions.ValidateAddress(addressToSend);
            if(addressToSend.Length == 90)
                addressToSend = Borlay.Iota.Library.Utils.Checksum.RemoveChecksum(addressToSend);
            

            var transfer = new TransferItem()
            {
                Address = addressToSend,
                Value = value,
                Tag = sendModel.MessageTag,
                Message = sendModel.Message
            };

            if (value > 0)
            {
                syncModel.Text = "Renew the balances";
                await api.RenewAddresses(addressItems);

                var filteredAddressItems = addressItems.FilterBalance(value).ToArray();
                var filterReminder = filteredAddressItems.Select(a => a.Address).ToList();
                filterReminder.Add(addressToSend);

                syncModel.Text = "Searching for the remainder";
                var remainderAddressItem = await GetRemainder(filterReminder.ToArray(), syncModel.Token);


                var transactions = transfer.CreateTransactions(remainderAddressItem.Address, filteredAddressItems).ToArray();
                await SendTransactions(transactions, syncModel, true);
            }
            else
            {
                var transactions = transfer.CreateTransactions().ToArray();
                await SendTransactions(transactions, syncModel, false);
            }
        }

        public async Task<AddressItem> GetRemainder(string[] addresses, CancellationToken cancellationToken)
        {
            var allAddressItems = addressesModel.GetAddressItems();
            foreach(var addressItem in allAddressItems)
            {
                if (addresses.Contains(addressItem.Address))
                    continue;

                if (addressItem.TransactionCount == 0)
                    return addressItem;

                var transactions = transactionCollection.Where(t => t.IsOwn && t.Address == addressItem.Address).ToArray();
                if (transactions.All(t => t.Balance >= 0))
                    return addressItem;
            }

            var lastIndex = allAddressItems.Max(a => a.Index);
            var api = CreateIotaClient();
            var seed = walletConfiguration.PrivateKey;
            var reminder = await api.FindReminderAddress(seed, lastIndex + 1, cancellationToken);
            return reminder;
        }

        private async Task SendTransactions(TransactionItem[] transactionItems, bool rebroadcast)
        {
            var syncModel = new CancelSyncModel() { Text = "The transaction is about to send" };
            walletModel.SyncModels.Add(syncModel);
            try
            {
                await SendTransactions(transactionItems, syncModel, rebroadcast);

            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                walletModel.SyncModels.Remove(syncModel);
            }
        }

        private async Task SendTransactions(TransactionItem[] transactionItems, CancelSyncModel syncModel, bool rebroadcast)
        {
            try
            {
                syncModel.Text = "Sending the transaction";
                var api = CreateIotaClient();
                var resultTransactionItems = await api.SendTransactions(transactionItems, syncModel.Token);
                await walletModel.ScanAddresses.ScanAddressesAsync(false);

                if (rebroadcast)
                {
                    syncModel.Text = "Forcing the transaction to confirm";
                    await RebroadcastTransactions(resultTransactionItems, syncModel);
                }
            }
            finally
            {
                await RefreshKnowAddressesAsync();
            }
        }

        private async Task RebroadcastTransactions(TransactionItem[] transactionItems, CancelSyncModel syncModel)
        {
            var api = CreateIotaClient();
            int count = 0;
            var transaction = transactionItems.Where(t => long.Parse(t.Value) < 0).FirstOrDefault();
            if (transaction != null)
            {
                bool firstTime = true;
                while (!syncModel.Token.IsCancellationRequested)
                {
                    var existingTransaction = (await api.GetTransactionItems(transaction.Hash)).FirstOrDefault();
                    if (existingTransaction.Persistence)
                        return;

                    if (!firstTime)
                        await Task.Delay(TimeSpan.FromMinutes(3), syncModel.Token);
                    firstTime = false;

                    if (syncModel.Token.IsCancellationRequested)
                        return;

                    existingTransaction = (await api.GetTransactionItems(transaction.Hash)).FirstOrDefault();
                    if (existingTransaction.Persistence)
                        return;

                    if(count >= 4)
                    {
                        await SendTransactions(transactionItems, syncModel, true);
                        break;
                    }

                    await api.Rebroadcast(transactionItems, syncModel.Token);
                    count++;
                    syncModel.Text = $"Forced {count} time(s)";
                }
            }
        }

        public decimal GetTotalBalance()
        {
            var addressItems = addressesModel.GetAddressItems();
            return addressItems.Sum(a => a.Balance);
        }

        private async Task CreateNewAddressAsync()
        {
            var beginIndex = 0;
            var knowAddresses = addressesModel.GetAddressItems();
            if (knowAddresses.Length > 0)
                beginIndex = knowAddresses.Max(a => a.Index) + 1;

            await CreateAddressAsync(beginIndex);
        }

        public async Task EnsureFirstAddressAsync()
        {
            if (addressesModel.ContentItems.Count != 0)
                return;

            await CreateAddressAsync(0);
        }

        public async Task CreateAddressAsync(int index)
        {
            var api = CreateIotaClient();
            var seed = walletConfiguration.PrivateKey;
            var address = await api.GetAddress(seed, index);
            await CreateAddressAsync(address);
        }

        private async Task CreateAddressAsync(AddressItem address)
        {
            var api = CreateIotaClient();
            if (address.TransactionCount == 0)
            {
                await api.SendTransfer(new Borlay.Iota.Library.Models.TransferItem()
                {
                    Address = address.Address,
                    Value = 0,
                    Message = "",
                    Tag = ""
                }, CancellationToken.None);
                await api.RenewAddresses(address);
                var addressItemModel = CreateAddressItemModel(address);
                await RefreshAddressTransactions(addressItemModel);
            }
        }

        private async Task RefreshKnowAddressesAsync()
        {
            var addressModels = addressesModel.GetAddressModels();
            if(addressModels.Length > 0)
                await RefreshAddressesAsync(addressModels);
        }

        private async Task RefreshAddressesAsync(AddressItemModel[] addressModels)
        {
            var api = CreateIotaClient();
            await addressModels.ParallelAsync(async a =>
            {
                var addressItem = (AddressItem)a.Tag;
                await api.RenewAddresses(addressItem);
                var task = RefreshAddressTransactions(a);
            });

            await walletModel.BalanceStats.RefreshBalances(WalletType.Iota);
        }

        async Task IScanAddresses.ScanAddressesAsync(IUpdateProgress updateProgress, bool force, CancellationToken cancellationToken)
        {
            var api = CreateIotaClient();
            var seed = walletConfiguration.PrivateKey;

            var beginIndex = 0;
            var knowAddresses = addressesModel.GetAddressItems();
            if (knowAddresses.Length > 0)
                beginIndex = knowAddresses.Max(a => a.Index) + 1;

            var totalScan = 500;
            List<Task> transactionTasks = new List<Task>();
            transactionTasks.Add(RefreshKnowAddressesAsync());

            for (int i = beginIndex; i < totalScan; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                updateProgress.UpdateProgress(i + 1, totalScan);

                var address = await api.GetAddress(seed, i);

                if (address.TransactionCount > 0)
                {
                    var addressItemModel = CreateAddressItemModel(address);
                    var task = RefreshAddressTransactions(addressItemModel);
                    transactionTasks.Add(task);
                }
                else
                {
                    if (i == 0)
                        await CreateAddressAsync(address);
                    if (!force)
                        break;
                }
            }

            var converttask = walletModel.BalanceStats.RefreshBalances(WalletType.Iota);
            transactionTasks.Add(converttask);
            await Task.WhenAll(transactionTasks);
            
        }

        private async Task RefreshAddressTransactions(AddressItemModel addressItemModel)
        {
            var address = (AddressItem)addressItemModel.Tag;
            var transactions = await transactionManager.AddTransactions(address.Transactions.Select(t => t.Hash).ToArray());
            if (!addressItemModel.HasWithdrawal)
                addressItemModel.HasWithdrawal = transactions.Any(t => long.Parse(t.Value) < 0);
        }


        private AddressItemModel CreateAddressItemModel(Borlay.Iota.Library.Models.AddressItem addressItem)
        {
            var existing = addressesModel.ContentItems.FirstOrDefault(a => a.Address == addressItem.Address);
            if (existing != null)
                return existing;

            var addressItemModel = new AddressItemModel((a) => OpenSend(a), a => Borlay.Iota.Library.Utils.Checksum.AddChecksum(a)) { Tag = addressItem };
            addressesModel.ContentItems.Add(addressItemModel);
            addressItem.BindTo(addressItemModel, d => d.Address, m => m.Address);
            addressItem.BindTo(addressItemModel, d => d.Balance, m => m.Balance);
            addressItem.Changed(a => a.Balance, (a, v) => RefreshBalanceStats());
            

            return addressItemModel;
        }

        private void RefreshBalanceStats()
        {
            var addresses = addressesModel.GetAddressItems();
            var iotaBalance = addresses.Sum(a => a.Balance);
            var balance = walletModel.BalanceStats.Balances.FirstOrDefault(b => b.WalletType == WalletType.Iota);
            balance.Value = iotaBalance;
        }

        private IotaApi CreateIotaClient()
        {
            // "http://iota.bitfinex.com:80"
            // "http://node.iotawallet.info:14265"
            // "http://node.deviceproof.org:14265"
            // "http://88.198.230.98:14265"
            // "http://iota.digits.blue:14265"
            var api = new IotaApi("http://node.iotawallet.info:14265");
            return api;
        }
    }
}
