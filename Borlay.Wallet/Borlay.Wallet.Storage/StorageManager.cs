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
    public class StorageManager
    {
        public AccountCollectionConfiguration GetAccounts()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var filePath = Path.Combine(path, "AccountCollection.xml");
            if (File.Exists(filePath))
            {
                var fileText = File.ReadAllText(filePath);
                var acc = JsonConvert.DeserializeObject<AccountCollectionConfiguration>(fileText);
                return acc;
            }
            else
            {
                return new AccountCollectionConfiguration()
                {
                    Accounts = new AccountConfiguration[0]
                };
            }
        }

            public void SaveAccounts(AccountCollectionConfiguration accountCollection)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var filePath = Path.Combine(path, "AccountCollection.xml");
            var accJson = JsonConvert.SerializeObject(accountCollection);
            File.WriteAllText(filePath, accJson);
        }

        public AccountConfiguration GetAccount(string userName, string passwordHash)
        {
            var accounts = GetAccounts();
            var account = accounts?.Accounts?.FirstOrDefault(a => a.UserName.ToLower() == userName.ToLower());
            if (account == null)
                return null;

            if(Security.IsPasswordValid(passwordHash, account.Password))
            {
                account.LastLoginDate = DateTime.Now;
                SaveAccounts(accounts);

                DecryptWallets(passwordHash, account.Wallets);
                return account;
            }
            else
            {
                throw new SecurityException("Password or user is invalid");
            }
        }

        public AccountConfiguration CreateAccount(string userName, string passwordHash)
        {
            var doublePasswordHash = Security.EncryptPassword(passwordHash);

            return new AccountConfiguration()
            {
                UserGuid = Guid.NewGuid(),
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
            var userGuid = accountConfiguration.UserGuid;
            var account = accounts?.Accounts?.FirstOrDefault(a => a.UserGuid == userGuid);
            if (account != null)
            {
                if(!Security.IsPasswordValid(passwordHash, account.Password))
                {
                    throw new SecurityException("Bad password");
                }
            }

            EncryptWallets(passwordHash, accountConfiguration.Wallets);

            var list = accounts?.Accounts?.ToList() ?? new List<AccountConfiguration>();
            list.RemoveAll(a => a.UserGuid == accountConfiguration.UserGuid);
            list.RemoveAll(a => a.UserName.ToLower() == accountConfiguration.UserName.ToLower());
            list.Add(accountConfiguration);
            accounts.Accounts = list.ToArray();
            SaveAccounts(accounts);
        }

        public void EncryptWallets(string passwordHash, params WalletConfiguration[] wallets)
        {
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

        public void  DecryptWallets(string passwordHash, params WalletConfiguration[] wallets)
        {
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
