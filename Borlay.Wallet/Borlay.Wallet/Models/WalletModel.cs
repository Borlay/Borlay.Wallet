using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class WalletModel : ModelBase
    {
        private readonly IScanAddresses scanAddresses;

        public WalletModel(IScanAddresses scanAddresses, params TabItem[] menuItems)
            : this(scanAddresses, new BalanceStatsModel(), menuItems)
        {
        }

        public WalletModel(IScanAddresses scanAddresses, BalanceStatsModel balanceStatsModel, params TabItem[] menuItems)
        {
            this.MenuItems = new ObservableCollection<TabItem>();
            this.BalanceStats = new BalanceStatsModel();
            this.scanAddresses = scanAddresses;

            this.ScanAddresses = new ScanAddressesModel(scanAddresses);

            foreach (var item in menuItems)
                this.MenuItems.Add(item);

            if (menuItems == null || menuItems.Length == 0) // initialize test environment
                InitializeMenu();
        }

        private void InitializeMenu()
        {
            this.MenuItems.Add(new TabItem()
            {
                Name = "Addresses",
                Selected = (t) => OpenAddresses(),
                IsSelected = true
            });
            this.MenuItems.Add(new TabItem()
            {
                Name = "Transactions",
                Selected = (t) => OpenTransactions()
            });
            this.MenuItems.Add(new TabItem()
            {
                Name = "Paper",
                Selected = (t) => OpenPaper()
            });
        }

        protected virtual IEnumerable<IconButtonModel> OpenAddressesButtons()
        {
            yield return new IconButtonModel(IconType.Restart);
            yield return new IconButtonModel(IconType.Plus);
        }

        public virtual async void OpenAddresses()
        {
            var iconButtons = OpenAddressesButtons().ToArray();
            var addressesView = new ContentCollectionModel<AddressItemModel>(iconButtons);
            for (int i = 0; i < 30; i++)
            {
                addressesView.ContentItems.Add(new AddressItemModel() { Address = "asdfasdfasdfa", Balance = 1234567 });
                addressesView.ContentItems.Add(new AddressItemModel() { Address = "asdfasdfasdfa", Balance = 1234967 });
                addressesView.ContentItems.Add(new AddressItemModel() { Address = "asdfasdfasdfa", Balance = 1000 });
                addressesView.ContentItems.Add(new AddressItemModel() { Address = "bakljsdlfjasdf", Balance = 3000000 });
            }

            View = addressesView;
        }

        public virtual async void OpenTransactions()
        {
            var iconButtons = OpenAddressesButtons().ToArray();
            var addressesView = new ContentListModel<BundleItemModel>(iconButtons);

            for (int i = 0; i < 30; i++)
            {
                addressesView.ContentItems.Add(new BundleItemModel() { Hash = "asdfasdfasdfa", Balance = 1234567 });
                addressesView.ContentItems.Add(new BundleItemModel() { Hash = "asdfasdfasdfa", Balance = 1234967, Tag = "some tag" });
                addressesView.ContentItems.Add(new BundleItemModel() { Hash = "asdfasdfasdfa", Balance = 1000 });
                addressesView.ContentItems.Add(new BundleItemModel() { Hash = "bakljsdlfjasdf", Balance = 3000000 });
            }

            View = addressesView;
        }

        public virtual async void OpenPaper()
        {

        }

        public ScanAddressesModel ScanAddresses { get; private set; }

        public BalanceStatsModel BalanceStats { get; private set; }

        public ObservableCollection<TabItem> MenuItems { get; private set; }

        private object view;
        public object View
        {
            get
            {
                return view;
            }
            set
            {
                view = value;
                NotifyPropertyChanged();
            }
        }
    }
}
