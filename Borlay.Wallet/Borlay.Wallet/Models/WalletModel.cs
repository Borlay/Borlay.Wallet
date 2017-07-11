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
        public WalletModel()
        {
            this.MenuItems = new ObservableCollection<TabItem>();
            this.MenuItems.Add(new TabItem()
            {
                Name = "Addresses",
                Selected = (t) =>
                {
                    var collectionModel = new CollectionModel<AddressItemModel>();
                    for (int i = 0; i < 30; i++)
                    {
                        collectionModel.Collection.Add(new AddressItemModel() { Address = "asdfasdfasdfa", Balance = 1234567 });
                        collectionModel.Collection.Add(new AddressItemModel() { Address = "asdfasdfasdfa", Balance = 1234967 });
                        collectionModel.Collection.Add(new AddressItemModel() { Address = "asdfasdfasdfa", Balance = 1000 });
                        collectionModel.Collection.Add(new AddressItemModel() { Address = "bakljsdlfjasdf", Balance = 3000000 });
                    }

                    var addressesView = new AddressesModel()
                    {
                        AddressItems = collectionModel
                    };

                    View = addressesView;
                },
                IsSelected = true
            });
            this.MenuItems.Add(new TabItem()
            {
                Name = "Transactions",
                Selected = (t) =>
                {
                    View = null;
                }
            });
            this.MenuItems.Add(new TabItem()
            {
                Name = "Paper",
                Selected = (t) =>
                {
                    View = null;
                }
            });

            this.BalanceStats = new BalanceStatsModel();
        }

        public BalanceStatsModel BalanceStats { get; set; }

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
