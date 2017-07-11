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
                this.Balances.Add(new BalanceItemModel() { Currency = "Eur", Value = 3.45m });
                this.Balances.Add(new BalanceItemModel() { Currency = "Iota", Value = 10000000 });
            }
        }

        public ObservableCollection<BalanceItemModel> Balances { get; set; }
    }
}
