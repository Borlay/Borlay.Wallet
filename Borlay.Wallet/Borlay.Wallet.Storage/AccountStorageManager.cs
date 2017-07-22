using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Storage
{
    public class AccountStorageManager
    {
        public AccountCollectionConfiguration GetAccounts()
        {
            return ConfigurationStorage.Get<AccountCollectionConfiguration>() ?? new AccountCollectionConfiguration();
        }

        public void SaveAccounts(AccountCollectionConfiguration accounts)
        {
            ConfigurationStorage.Save(accounts);
        }

        public AccountConfiguration GetAccount(string userName, string passwordHash)
        {
            var accounts = GetAccounts();
            var account = accounts?.Accounts?.FirstOrDefault(a => a.UserName.ToLower() == userName.ToLower());
            if (account == null)
                return null;

            ValidateAccount(account, passwordHash);

            account.LastLoginDate = DateTime.Now;
            ConfigurationStorage.Save(accounts);

            DecryptWallets(passwordHash, account.Wallets);
            return account;
        }

        public void ValidateAccount(AccountConfiguration account, string passwordHash)
        {
            if (!Security.IsPasswordValid(passwordHash, account.Password))
                throw new SecurityException("Password or username is invalid");
        }

        public AccountConfiguration CreateAccount(string userName, string passwordHash)
        {
            var doublePasswordHash = Security.EncryptPassword(passwordHash);

            return new AccountConfiguration()
            {
                AccountId = Guid.NewGuid().ToString(),
                UserName = userName,
                Password = doublePasswordHash,
                CreationDate = DateTime.Now,
                LastLoginDate = DateTime.Now,
                Wallets = new WalletConfiguration[] { }
            };
        }

        public void SaveAccount(string passwordHash, AccountConfiguration accountConfiguration)
        {
            var accounts = GetAccounts();
            if (accounts == null)
                throw new NullReferenceException(nameof(accounts));

            var accountId = accountConfiguration.AccountId;
            var account = accounts?.Accounts?.FirstOrDefault(a => a.AccountId == accountId);
            if (account != null)
            {
                if (!Security.IsPasswordValid(passwordHash, account.Password))
                    throw new SecurityException("Bad password");
            }

            EncryptWallets(passwordHash, accountConfiguration.Wallets);

            var list = accounts?.Accounts?.ToList() ?? new List<AccountConfiguration>();
            list.RemoveAll(a => a.AccountId == accountConfiguration.AccountId);
            list.RemoveAll(a => a.UserName.ToLower() == accountConfiguration.UserName.ToLower());
            list.Add(accountConfiguration);
            accounts.Accounts = list.ToArray();
            SaveAccounts(accounts);

            DecryptWallets(passwordHash, accountConfiguration.Wallets);
        }

        public void SaveWallet(string userName, string passwordHash, WalletConfiguration wallet)
        {
            var accounts = GetAccounts();
            var account = accounts?.Accounts?.FirstOrDefault(a => a.UserName.ToLower() == userName.ToLower());
            if (account == null)
                throw new NullReferenceException($"Account for username '{userName}' not found");

            ValidateAccount(account, passwordHash);

            DecryptWallets(passwordHash, account.Wallets);
            var wallets = account.Wallets.ToList();

            wallets.RemoveAll(w => w.PrivateKey == wallet.PrivateKey);

            wallets.Add(wallet);
            account.Wallets = wallets.ToArray();

            EncryptWallets(passwordHash, account.Wallets);
            SaveAccounts(accounts);
            DecryptWallets(passwordHash, account.Wallets);
        }

        public void EncryptWallets(string passwordHash, params WalletConfiguration[] wallets)
        {
            if (wallets == null) return;

            foreach (var wallet in wallets)
            {
                if (wallet.EncryptionType == EncryptionType.None)
                {
                    var encryptedPrivateKey = Security.Encrypt(wallet.PrivateKey, passwordHash);
                    wallet.PrivateKey = encryptedPrivateKey;
                    wallet.EncryptionType = EncryptionType.Rijndael;
                }
            }
        }

        public void DecryptWallets(string passwordHash, params WalletConfiguration[] wallets)
        {
            if (wallets == null) return;

            foreach (var wallet in wallets)
            {
                if (wallet.EncryptionType == EncryptionType.Rijndael)
                {
                    var decryptedPrivateKey = Security.Decrypt(wallet.PrivateKey, passwordHash);
                    wallet.PrivateKey = decryptedPrivateKey;
                    wallet.EncryptionType = EncryptionType.None;
                }
            }
        }
    }
}
