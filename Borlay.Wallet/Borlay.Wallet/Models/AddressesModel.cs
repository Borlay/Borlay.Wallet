using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Borlay.Wallet.Models
{
    public class AddressesModel // todo delete
    {
        public AddressesModel()
        {
            //this.ContentItems = new CollectionModel<AddressItemModel>();
            //this.ActionItems = new ObservableCollection<IconButtonModel>();

            //this.ActionItems.Add(new IconButtonModel(IconType.Restart));
            //this.ActionItems.Add(new IconButtonModel(IconType.Plus));
        }

        

        public CollectionModel<AddressItemModel> ContentItems { get; set; }
    }
}
