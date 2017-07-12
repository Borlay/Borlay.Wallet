using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class BalanceStatsModel
    {
        public BalanceStatsModel(params BalanceItemModel[] balanceItems)
        {
            Balances = new ObservableCollection<BalanceItemModel>(balanceItems);

            if (balanceItems.Length == 0) // initialize test environment
            {
                this.Balances.Add(new BalanceItemModel(Storage.WalletType.Iota) { Value = 0 });
                this.Balances.Add(new BalanceItemModel(Storage.WalletType.Bitcoin) { Value = 123456789 });
            }
        }

        public ObservableCollection<BalanceItemModel> Balances { get; set; }
    }
}
