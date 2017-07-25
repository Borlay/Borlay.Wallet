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
        }

        

        public CollectionModel<AddressItemModel> ContentItems { get; set; }
    }
}
