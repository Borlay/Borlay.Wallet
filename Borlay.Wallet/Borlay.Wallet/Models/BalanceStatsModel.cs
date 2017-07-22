using Borlay.Wallet.Balances;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
                var iotaBalance = new BalanceItemModel(Storage.WalletType.Iota) { Value = 0 };
                var eurBalance = new BalanceItemModel(Storage.WalletType.Eur) { Value = 123456789 };
                this.Balances.Add(iotaBalance);
                this.Balances.Add(eurBalance);

                iotaBalance.Changed(b => b.Value, (b, o) =>
                {
                    RefreshBalances(Storage.WalletType.Iota);
                });
            }
        }

        public async Task RefreshBalances(Storage.WalletType originalCurrency)
        {
            await RefreshBalances(originalCurrency, CancellationToken.None);
        }

        public async Task RefreshBalances(Storage.WalletType originalCurrency, CancellationToken cancellationToken)
        {
            var originalBalanceModel = Balances.FirstOrDefault(b => b.WalletType == originalCurrency);
            if (originalBalanceModel == null)
                throw new ArgumentException($"Balance item model for '{originalCurrency}' not found");

            BalanceConverter converter = new BalanceConverter();
            foreach (var balanceItemModel in Balances)
            {
                if (balanceItemModel.WalletType != originalCurrency)
                {
                    var balance = await converter.GetBalance(originalCurrency, originalBalanceModel.Value,
                        balanceItemModel.WalletType, cancellationToken);
                    balanceItemModel.Value = balance;
                }
            }
        }


        public ObservableCollection<BalanceItemModel> Balances { get; set; }
    }
}
