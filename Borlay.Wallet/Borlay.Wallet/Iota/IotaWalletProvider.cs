using Borlay.Iota.Library;
using Borlay.Wallet.Models;
using Borlay.Wallet.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Wallet.Iota
{
    public class IotaWalletProvider
    {
        private readonly WalletConfiguration walletConfiguration;
        private readonly WalletModel walletModel;
        private readonly ContentCollectionModel<AddressItemModel> addressesModel;
        private readonly IconButtonModel[] addressesButtons;
        private readonly ISelectedChanged selectedChanged;
        private readonly ActionCommandGroup commandGroup;

        private bool isFirstTime = true;

        public WalletModel Wallet => walletModel;

        public IotaWalletProvider(WalletConfiguration walletConfiguration, ISelectedChanged selectedChanged)
        {
            this.walletConfiguration = walletConfiguration;
            this.selectedChanged = selectedChanged;

            addressesButtons = OpenAddressesButtons().ToArray();
            commandGroup = new ActionCommandGroup(addressesButtons.Select(b => b.ButtonClick).ToArray());
            addressesModel = new ContentCollectionModel<AddressItemModel>(addressesButtons);

            var balanceItems = CreateBalanceItems().ToArray();
            var menuItems = CreateMenuItems().ToArray();

            var balanceStatsModel = new BalanceStatsModel(balanceItems);
            walletModel = new WalletModel(balanceStatsModel, menuItems);

            menuItems.First().IsSelected = true;
            this.selectedChanged.SelectedChanged += SelectedChanged_SelectedChanged;
        }

        private async void SelectedChanged_SelectedChanged(object arg1, bool arg2)
        {
            await InitializeAsync(CancellationToken.None);
        }

        public async Task InitializeAsync(CancellationToken cancellationToken, bool force = false)
        {
            if (isFirstTime)
            {
                isFirstTime = false;
                //commandGroup.SetCanExecute(false);
                try
                {
                    await RefreshAllAddressesAsync(cancellationToken, force);
                }
                finally
                {
                    //commandGroup.SetCanExecute(true);
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
                Selected = (t) => OpenTransactions()
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
            yield return new IconButtonModel(b => { }, IconType.Plus);
        }


        public virtual async void OpenTransactions()
        {
            var iconButtons = addressesButtons.First();
            var addressesView = new ContentListModel<BundleItemModel>(iconButtons);

            for (int i = 0; i < 30; i++)
            {
                addressesView.ContentItems.Add(new BundleItemModel() { Hash = "asdfasdfasdfa", Balance = 1234567 });
                addressesView.ContentItems.Add(new BundleItemModel() { Hash = "asdfasdfasdfa", Balance = 1234967, Tag = "some tag" });
                addressesView.ContentItems.Add(new BundleItemModel() { Hash = "asdfasdfasdfa", Balance = 1000 });
                addressesView.ContentItems.Add(new BundleItemModel() { Hash = "bakljsdlfjasdf", Balance = 3000000 });
            }

            Wallet.View = addressesView;
        }

        public virtual async void OpenPaper()
        {
            Wallet.View = null;
        }

        public async Task EnsureFirstAddressAsync()
        {
            if (addressesModel.ContentItems.Count != 0)
                return;

            var addressItemModel = await CreateAddressAsync(0);

            if (addressesModel.ContentItems.Count == 0)
                addressesModel.ContentItems.Add(addressItemModel);
        }

        public async Task<AddressItemModel> CreateAddressAsync(int index)
        {
            var api = CreateIotaClient();
            var seed = walletConfiguration.PrivateKey;
            var address = await api.GetAddress(seed, index);
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
            }
            var addressItemModel = CreateAddressItemModel(address);
            return addressItemModel;
        }

        private IEnumerable<Borlay.Iota.Library.Models.AddressItem> GetKnowAddresses()
        {
            return addressesModel.ContentItems.Select(a => a.Tag).OfType<Borlay.Iota.Library.Models.AddressItem>();
        }

        private async Task RefreshKnowAddressesAsync()
        {
            var addresses = GetKnowAddresses().ToArray();
            var api = CreateIotaClient();
            await api.RenewAddresses(addresses);
        }

        private async Task RefreshAllAddressesAsync(CancellationToken cancellationToken, bool force = false)
        {
            var knowAddresses = GetKnowAddresses().ToArray();
            addressesModel.ContentItems.Clear();

            var api = CreateIotaClient();
            var seed = walletConfiguration.PrivateKey;

            for (int i = 0; i < 500; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var address = knowAddresses.FirstOrDefault(a => a.Index == i);
                if(address == null)
                    address = await api.GetAddress(seed, i);
                
                //if (i == 0 && address.TransactionCount == 0)
                //{
                //    await api.SendTransfer(new Borlay.Iota.Library.Models.TransferItem()
                //    {
                //        Address = address.Address,
                //        Value = 0,
                //        Message = "",
                //        Tag = ""
                //    }, cancellationToken);
                //    await api.RenewAddresses(address);
                //}

                if (address.TransactionCount == 0 && !force)
                    break;

                if (address.TransactionCount > 0)
                {
                    var addressItemModel = CreateAddressItemModel(address);
                    addressesModel.ContentItems.Add(addressItemModel);
                }
            }
        }

        private AddressItemModel CreateAddressItemModel(Borlay.Iota.Library.Models.AddressItem addressItem)
        {
            var addressItemModel = new AddressItemModel() { Tag = addressItem };
            addressItem.BindTo(addressItemModel, d => d.Address, m => m.Address);
            addressItem.BindTo(addressItemModel, d => d.Balance, m => m.Balance);
            return addressItemModel;
        }

        private IotaApi CreateIotaClient()
        {
            // "http://iota.bitfinex.com:80"
            // "http://node.iotawallet.info:14265"
            // "http://node.deviceproof.org:14265"
            // "http://88.198.230.98:14265"
            // "http://iota.digits.blue:14265"
            var api = new IotaApi("http://iota.bitfinex.com:80");
            api.NumberOfThreads = 5;
            return api;
        }
    }
}
