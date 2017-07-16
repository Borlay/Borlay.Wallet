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
        private readonly ObservableCollection<TransactionItemModel> transactionCollection;
        private readonly ObservableCollection<string> ownTransactions = new ObservableCollection<string>();

        public IotaWalletTransactionManager(ObservableCollection<TransactionItemModel> transactionCollection,
            ObservableCollection<BundleItemModel> bundleItems)
        {
            this.transactionCollection = transactionCollection;
            this.bundleItems = bundleItems;
        }

        public async Task<TransactionItem[]> AddTransactions(params string[] transactionHashes)
        {
            foreach(var t in transactionHashes)
            {
                if (!ownTransactions.Contains(t))
                    ownTransactions.Add(t);
            }

            var api = CreateIotaClient();
            var transactions = await api.GetTransactionItems(transactionHashes);
            var bundleHashes = transactions.Select(b => b.Bundle).Distinct().ToArray();
            await UpdateBundles(bundleHashes);
            return transactions;
        }

        private void AppendTransactions(params TransactionItemModel[] transactionModels)
        {
            foreach (var transaction in transactionModels)
            {
                if (!transaction.IsOwn)
                    transaction.IsOwn = ownTransactions.Contains(transaction.Hash);

                if (!transactionCollection.Any(t => t.Hash == transaction.Hash))
                    transactionCollection.Add(transaction);
            }
        }

        public async Task UpdateBundles(string[] bundleHashes)
        {
            var api = CreateIotaClient();
            foreach (var bundleHash in bundleHashes)
            {
                var transactions = await api.GetBundleTransactionItems(bundleHash);
                //if(transactions.Any(t => t.Persistence && Math.Abs(Int64.Parse(t.Value)) > 0))
                var bundle = bundleItems.UpdateBundleItems(bundleHash, transactions);
                AppendTransactions(bundle.BundleDetail.TransactionItems.ToArray());
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
            //api.NumberOfThreads = 5;
            return api;
        }
    }
}
