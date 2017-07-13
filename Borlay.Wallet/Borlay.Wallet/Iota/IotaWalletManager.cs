﻿using Borlay.Iota.Library;
using Borlay.Wallet.Models;
using Borlay.Wallet.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Borlay.Wallet.Iota
{
    public class IotaWalletManager : IScanAddresses
    {
        private readonly WalletConfiguration walletConfiguration;
        private readonly WalletModel walletModel;
        private readonly ContentCollectionModel<AddressItemModel> addressesModel;
        private readonly ContentListModel<BundleItemModel> bundlesModel;
        private readonly IconButtonModel[] addressesButtons;
        private readonly ISelectedChanged selectedChanged;
        private readonly ActionCommandGroup commandGroup;

        private readonly IotaWalletTransactionManager transactionManager;

        private bool isFirstTime = true;

        public WalletModel Wallet => walletModel;

        public IotaWalletManager(WalletConfiguration walletConfiguration, ISelectedChanged selectedChanged)
        {
            this.walletConfiguration = walletConfiguration;
            this.selectedChanged = selectedChanged;

            addressesButtons = OpenAddressesButtons().ToArray();
            commandGroup = new ActionCommandGroup(addressesButtons.Select(b => b.ButtonClick).ToArray());
            addressesModel = new ContentCollectionModel<AddressItemModel>(addressesButtons);
            bundlesModel = new ContentListModel<BundleItemModel>(addressesButtons.First());
            transactionManager = new IotaWalletTransactionManager(bundlesModel.ContentItems);

            var balanceItems = CreateBalanceItems().ToArray();
            var menuItems = CreateMenuItems().ToArray();

            var balanceStatsModel = new BalanceStatsModel(balanceItems);
            walletModel = new WalletModel(this, balanceStatsModel, menuItems);

            menuItems.First().IsSelected = true;
            this.selectedChanged.SelectedChanged += SelectedChanged_SelectedChanged;
        }

        private void SelectedChanged_SelectedChanged(object arg1, bool arg2)
        {
            InitializeAsync();
        }

        public async void InitializeAsync(bool force = false)
        {
            if (isFirstTime)
            {
                isFirstTime = false;
                try
                {
                    await walletModel.ScanAddresses.ScanAddressesAsync(force);
                }
                catch(OperationCanceledException)
                {
                    // do nothing
                }
                catch(Exception e)
                {
                    // todo handle in better way
                    MessageBox.Show(e.Message);
                }
            }
        }


        private IEnumerable<BalanceItemModel> CreateBalanceItems()
        {
            yield break;
        }

        private IEnumerable<TabItem> CreateMenuItems()
        {
            yield return new TabItem()
            {
                Name = "Addresses",
                Selected = (t) => 
                    Wallet.View = addressesModel,
                IsSelected = false
            };
            yield return new TabItem()
            {
                Name = "Transactions",
                Selected = (t) => Wallet.View = bundlesModel, // OpenTransactions()
            };
            yield return new TabItem()
            {
                Name = "Paper",
                Selected = (t) => OpenPaper()
            };
        }

        protected IEnumerable<IconButtonModel> OpenAddressesButtons()
        {
            yield return new IconButtonModel(b => RefreshKnowAddressesAsync(), IconType.Restart);
            yield return new IconButtonModel(b => { }, IconType.Plus);
        }


        //public virtual async void OpenTransactions()
        //{
        //    var iconButtons = addressesButtons.First();
        //    var bundlesView = new ContentListModel<BundleItemModel>(iconButtons);
        //    bundlesView.ContentItems.Add(new BundleItemModel() { Hash = "asdfasdfasdfa", Balance = 1234567 });
        //    bundlesView.ContentItems.Add(new BundleItemModel() { Hash = "asdfasdfasdfa", Balance = -1234967, Tag = "some tag" });
        //    bundlesView.ContentItems.Add(new BundleItemModel() { Hash = "asdfasdfasdfa", Balance = 1000 });
        //    bundlesView.ContentItems.Add(new BundleItemModel() { Hash = "bakljsdlfjasdf", Balance = 3000000 });
        //    Wallet.View = bundlesView;
        //}

        public virtual async void OpenPaper()
        {
            Wallet.View = null;
        }

        public async Task EnsureFirstAddressAsync()
        {
            if (addressesModel.ContentItems.Count != 0)
                return;

            var addressItemModel = await CreateAddressAsync(0);

            if (addressesModel.ContentItems.Count == 0)
                addressesModel.ContentItems.Add(addressItemModel);
        }

        public async Task<AddressItemModel> CreateAddressAsync(int index)
        {
            var api = CreateIotaClient();
            var seed = walletConfiguration.PrivateKey;
            var address = await api.GetAddress(seed, index);
            if (address.TransactionCount == 0)
            {
                await api.SendTransfer(new Borlay.Iota.Library.Models.TransferItem()
                {
                    Address = address.Address,
                    Value = 0,
                    Message = "",
                    Tag = ""
                }, CancellationToken.None);
                await api.RenewAddresses(address);
            }
            var addressItemModel = CreateAddressItemModel(address);
            return addressItemModel;
        }

        private IEnumerable<Borlay.Iota.Library.Models.AddressItem> GetKnowAddresses()
        {
            return addressesModel.ContentItems.Select(a => a.Tag).OfType<Borlay.Iota.Library.Models.AddressItem>();
        }

        private async Task RefreshKnowAddressesAsync()
        {
            var addresses = GetKnowAddresses().ToArray();
            var api = CreateIotaClient();
            await api.RenewAddresses(addresses);
            List<string> transactionHashes = new List<string>();
            foreach(var address in addresses)
            {
                transactionHashes.AddRange(address.Transactions.Select(t => t.Hash).ToArray());
            }
            await transactionManager.AddTransactions(transactionHashes.ToArray());
        }

        async Task IScanAddresses.ScanAddressesAsync(IUpdateProgress updateProgress, bool force, CancellationToken cancellationToken)
        {
            var knowAddresses = GetKnowAddresses().ToArray();
            addressesModel.ContentItems.Clear();

            var api = CreateIotaClient();
            var seed = walletConfiguration.PrivateKey;

            var totalScan = 500;

            for (int i = 0; i < totalScan; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                updateProgress.UpdateProgress(i + 1, totalScan);

                var address = knowAddresses.FirstOrDefault(a => a.Index == i);
                if (address == null)
                    address = await api.GetAddress(seed, i);

                if (address.TransactionCount == 0 && !force)
                    break;

                if (address.TransactionCount > 0)
                {
                    var addressItemModel = CreateAddressItemModel(address);
                    addressesModel.ContentItems.Add(addressItemModel);

                    await transactionManager.AddTransactions(address.Transactions.Select(t => t.Hash).ToArray());
                }
            }
        }

        private AddressItemModel CreateAddressItemModel(Borlay.Iota.Library.Models.AddressItem addressItem)
        {
            var addressItemModel = new AddressItemModel() { Tag = addressItem };
            addressItem.BindTo(addressItemModel, d => d.Address, m => m.Address);
            addressItem.BindTo(addressItemModel, d => d.Balance, m => m.Balance);
            addressItem.Changed(a => a.Balance, (a, v) => RefreshBalanceStats());
            return addressItemModel;
        }

        private void RefreshBalanceStats()
        {
            var addresses = GetKnowAddresses();
            var iotaBalance = addresses.Sum(a => a.Balance);
            var balance = walletModel.BalanceStats.Balances.FirstOrDefault(b => b.WalletType == WalletType.Iota);
            balance.Value = iotaBalance;
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
