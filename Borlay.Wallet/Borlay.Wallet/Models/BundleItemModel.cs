using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class BundleItemModel : TransactionItemBaseModel
    {
        public ObservableCollection<TransactionItemModel> TransactionItems { get; private set; }
    }
}
