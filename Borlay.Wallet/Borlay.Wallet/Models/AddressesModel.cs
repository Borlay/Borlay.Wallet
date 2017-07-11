using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Borlay.Wallet.Models
{
    public class AddressesModel
    {
        public AddressesModel()
        {
            this.AddressItems = new CollectionModel<AddressItemModel>();
            this.ActionItems = new ObservableCollection<IconButtonModel>();

            this.ActionItems.Add(new IconButtonModel(IconType.Restart));
            this.ActionItems.Add(new IconButtonModel(IconType.Plus));
        }

        public ICommand Refresh { get; set; }

        public ICommand New { get; set; }

        public ObservableCollection<IconButtonModel> ActionItems { get; private set; }

        public CollectionModel<AddressItemModel> AddressItems { get; set; }
    }
}
