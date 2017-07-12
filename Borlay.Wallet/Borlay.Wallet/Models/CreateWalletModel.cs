using Borlay.Wallet.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class CreateWalletModel : ModelBase
    {
        private readonly TaskCompletionSource<WalletConfiguration> tcs;
        private SecureString walletKey;
        private SecureString defaultWalletKey;
        private string name;

        public CreateWalletModel()
        {
            this.tcs = new TaskCompletionSource<WalletConfiguration>();
            var key = Borlay.Iota.Library.Utils.IotaApiUtils.GenerateRandomTrytes();
            this.defaultWalletKey = key.GetSecureString();
            this.WalletKey = key;
            this.Name = WalletType.Iota.ToString();

            this.CreateClick = new ActionCommandAsync(o => Create());
            this.CancelClick = new ActionCommandAsync(o => Cancel());
        }

        public Task<WalletConfiguration> CreateWallet()
        {
            return tcs.Task;
        }

        private async Task Create()
        {
            if(tcs.Task.Status == TaskStatus.WaitingForActivation)
            {
                var walletStringKey = walletKey.GetString();
                var isImporting = defaultWalletKey.GetString() != walletStringKey;
                var walletConfiguration = new WalletConfiguration()
                {
                    PrivateKey = walletStringKey,
                    WalletType = WalletType.Iota,
                    IsImported = isImporting,
                    Name = name
                };
                tcs.TrySetResult(walletConfiguration);
            }
        }

        private async Task Cancel()
        {
            tcs.TrySetCanceled();
        }

        public string WalletKey
        {
            get
            {
                return walletKey.GetString();
            }
            set
            {
                if(walletKey == null || walletKey.GetString() != value)
                {
                    walletKey = value.GetSecureString();
                    NotifyPropertyChanged();
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public IActionCommand CreateClick { get; set; }

        public IActionCommand CancelClick { get; set; }
    }
}
