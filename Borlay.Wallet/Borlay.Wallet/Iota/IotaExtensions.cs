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
    public static class IotaExtensions
    {
        public static string ValidateAddress(string address)
        {
            address = address.ToUpper();

            if (string.IsNullOrWhiteSpace(address))
                throw new Exception("Address should not be empty");

            Borlay.Iota.Library.Utils.InputValidator.CheckAddress(address);
            if (address.Length == 90)
            {
                if (!Borlay.Iota.Library.Utils.Checksum.IsValidChecksum(address))
                    throw new Exception("Bad address checksum.");
            }

            return address;
        }

        public static AddressItemModel[] GetAddressModels(this ContentCollectionModel<AddressItemModel> content)
        {
            return content.ContentItems.ToArray();
        }

        public static AddressItem[] GetAddressItems(this ContentCollectionModel<AddressItemModel> content)
        {
            return content.GetAddressModels().GetAddressItems();
        }

        public static AddressItem[] GetAddressItems(this IEnumerable<AddressItemModel> addressItemModels)
        {
            var addressItems = addressItemModels.Select(a => a.Tag).OfType<AddressItem>().ToArray();
            return addressItems;
        }

        public static void EnsureTransferAddresses(this IotaApi iotaApi, TransferAddresses transferAddresses)
        {

        }

        public static IEnumerable<AddressItem> FilterBalance(this IEnumerable<AddressItem> addressItems, long needBalance)
        {
            foreach (var address in addressItems)
            {
                if (address.Balance == 0) continue;

                needBalance -= address.Balance;
                yield return address;

                if (needBalance <= 0)
                    yield break;
            }

            throw new Exception("Not enough balance");
        }


        public static TransactionItemModel ToModel(this TransactionItem transactionItem)
        {
            var model = new TransactionItemModel();
            return model.Update(transactionItem);
        }

        public static TransactionItemModel Update(this TransactionItemModel model, TransactionItem transactionItem)
        {
            model.Hash = transactionItem.Hash;
            model.Address = transactionItem.Address;
            model.Index = int.Parse(transactionItem.CurrentIndex);
            model.Balance = Int64.Parse(transactionItem.Value);
            model.IsConfirmed = transactionItem.Persistence;
            model.DateTime = new DateTime(long.Parse(transactionItem.Timestamp));
            model.TransactionTag = transactionItem.Tag;
            model.Tag = transactionItem;
            return model;
        }

        public static BundleItemModel UpdateBundleItems(this ObservableCollection<BundleItemModel> bundleItems, string bundleHash, TransactionItem[] transactionItems)
        {
            var bundle = bundleItems.FirstOrDefault(b => b.Hash == bundleHash);
            if (bundle == null)
            {
                var transactions = transactionItems.Select(t => t.ToModel()).ToArray();
                bundle = new BundleItemModel(transactions)
                {
                    Hash = bundleHash
                };
                bundle.ValuateBundle();
                bundleItems.Add(bundle);
                return bundle;
            }

            foreach (var transaction in transactionItems)
            {
                var tModel = bundle.BundleDetail.TransactionItems.FirstOrDefault(t => t.Hash == transaction.Hash);
                if (tModel != null)
                    tModel.Update(transaction);
                else
                {
                    tModel = transaction.ToModel();
                    bundle.BundleDetail.TransactionItems.Add(tModel);
                }
            }

            bundle.ValuateBundle();
            return bundle;
        }

        public static void ValuateBundle(this BundleItemModel bundleModel)
        {
            if (bundleModel == null)
                throw new ArgumentNullException(nameof(bundleModel));

            var transactions = bundleModel.BundleDetail.TransactionItems.Where(t => t.Index == 0).ToArray();
            if (transactions.Length == 0)
            {
                bundleModel.IsConfirmed = false;
                bundleModel.Balance = 0;
                return;
            }
            var first = transactions.First();
            var confirmed = transactions.FirstOrDefault(t => t.IsConfirmed);
            var isConfirmed = confirmed != null;
            var balance = confirmed != null ? confirmed.Balance : first.Balance;
            var dateTime = confirmed != null ? confirmed.DateTime : first.DateTime;
            var address = confirmed != null ? confirmed.Address : first.Address;

            bundleModel.IsConfirmed = isConfirmed;
            bundleModel.Balance = balance;
            bundleModel.DateTime = dateTime;
            bundleModel.Address = address;
        }
    }
}
