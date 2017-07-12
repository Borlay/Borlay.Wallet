using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Storage
{
    public class AccountConfiguration
    {
        public string AccountId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public WalletConfiguration[] Wallets { get; set; }
    }

    public class WalletConfiguration
    {
        private SecureString securePrivateKey;

        public WalletConfiguration()
        {
            this.WalletId = Guid.NewGuid().ToString();
            this.CreationDate = DateTime.Now;
            this.EncryptionType = EncryptionType.None;
            this.IsActive = true;
            this.AlwaysScan = false;
        }

        public string WalletId { get; set; }

        public string PrivateKey
        {
            get
            {
                return securePrivateKey.GetString();
            }
            set
            {
                securePrivateKey = SecureExtensions.GetSecureString(value);
            }
        }

        public string Name { get; set; }

        public WalletType WalletType { get; set; }

        public bool IsActive { get; set; }

        public bool IsImported { get; set; }

        public bool AlwaysScan { get; set; }

        public EncryptionType EncryptionType { get; set; }

        public DateTime CreationDate { get; set; }
    }

    public enum EncryptionType
    {
        None = 0,
        Rijndael
    }

    public enum WalletType
    {
        Iota = 1
    }
}
