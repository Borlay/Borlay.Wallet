using Borlay.Iota.Library;
using Borlay.Iota.Library.Models;
using Borlay.Wallet.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Iota
{
    public class IotaWalletTransactionManager
    {
        private readonly ObservableCollection<BundleItemModel> bundleItems;

        public IotaWalletTransactionManager(ObservableCollection<BundleItemModel> bundleItems)
        {
            this.bundleItems = bundleItems;
        }

        public async Task AddTransactions(params string[] transactionHashes)
        {
            var api = CreateIotaClient();
            var transactions = await api.GetTransactionItems(transactionHashes);
            var bundleHashes = transactions.Select(b => b.Bundle).Distinct().ToArray();
            await UpdateBundles(bundleHashes);
        }

        public async Task UpdateBundles(params string[] bundleHashes)
        {
            var api = CreateIotaClient();
            foreach (var bundleHash in bundleHashes)
            {
                var transactions = await api.GetBundleTransactionItems(bundleHash);
                if(transactions.Any(t => t.Persistence && Int64.Parse(t.Value) > 0))
                    bundleItems.UpdateBundleItems(bundleHash, transactions);
            }
        }

        public void Clear()
        {
            bundleItems.Clear();
        }

        public IEnumerable<TransactionItemModel> GetTransactions()
        {
            foreach(var bundleItem in bundleItems)
            {
                foreach (var transactionItem in bundleItem.BundleDetail.TransactionItems)
                    yield return transactionItem;
            }
        }

        private IotaApi CreateIotaClient()
        {
            // "http://iota.bitfinex.com:80"
            // "http://node.iotawallet.info:14265"
            // "http://node.deviceproof.org:14265"
            // "http://88.198.230.98:14265"
            // "http://iota.digits.blue:14265"
            var api = new IotaApi("http://iota.bitfinex.com:80");
            api.NumberOfThreads = 5;
            return api;
        }
    }
}
