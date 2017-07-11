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
            if(isFirstTime)
            {
                isFirstTime = false;

                await InitializeAsync();
            }
        }

        private async Task InitializeAsync()
        {
            commandGroup.SetCanExecute(false);
            //addressesButtons.First().ButtonClick.SetCanExecute(false);
            await FullRefreshAddressesAsync();
            commandGroup.SetCanExecute(true);
            //addressesButtons.First().ButtonClick.SetCanExecute(true);
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
            yield return new IconButtonModel(b => RefreshAddressesAsync(), IconType.Restart);
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

        private async Task RefreshAddressesAsync()
        {
            var addresses = addressesModel.ContentItems.Select(a => a.Tag)
                .OfType<Borlay.Iota.Library.Models.AddressItem>().ToArray();
            var api = CreateIotaClient();
            await api.RenewAddresses(addresses);
        }

        private async Task FullRefreshAddressesAsync(bool force = false)
        {
            addressesModel.ContentItems.Clear();

            var api = CreateIotaClient();
            var seed = walletConfiguration.PrivateKey;

            var groupCount = 1;

            for (int i = 0; i < 500; i += groupCount)
            {
                var addresses = await api.GetAddresses(seed, i, groupCount, CancellationToken.None);
                
                if (addresses.All(a => a.TransactionCount == 0) && i == 0)
                {
                    var address = addresses.First();
                    await api.SendTransfer(new Borlay.Iota.Library.Models.TransferItem()
                    {
                        Address = address.Address,
                        Value = 0,
                        Message = "",
                        Tag = ""
                    }, CancellationToken.None);
                    await api.RenewAddresses(address);
                }

                if (addresses.All(a => a.TransactionCount == 0) && !force)
                    break;

                addresses = addresses.Where(a => a.TransactionCount > 0).ToArray();
                foreach (var addr in addresses)
                {
                    var addressItemModel = new AddressItemModel() { Tag = addr };
                    addr.BindTo(addressItemModel, d => d.Address, m => m.Address);
                    addr.BindTo(addressItemModel, d => d.Balance, m => m.Balance);
                    addressesModel.ContentItems.Add(addressItemModel);
                }
            }
        }

        private IotaApi CreateIotaClient()
        {
            // "http://iota.bitfinex.com:80"
            // "http://node.iotawallet.info:14265"
            // "http://node.deviceproof.org:14265"
            // "http://88.198.230.98:14265"
            // "http://iota.digits.blue:14265"
            var api = new IotaApi("http://iota.digits.blue:14265");
            api.NumberOfThreads = 5;
            return api;
        }
    }
}
