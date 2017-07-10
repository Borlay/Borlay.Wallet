using Borlay.Wallet.Models;
using Borlay.Wallet.Models.General;
using Borlay.Wallet.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Borlay.Wallet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly AccountStorageManager storageManager = new AccountStorageManager();

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            // to remember ListCollectionView

            var accounts = storageManager.GetAccounts();
            var lastAccount = accounts?.Accounts?.OrderByDescending(o => o.LastLoginDate).FirstOrDefault();

            this.view = new UserLoginModel(async (model) =>
            {
                try
                {
                    var account = await Login(model.UserName, model.Password);
                    // do loged stuffs
                    View = new WalletModel();
                    return null;
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }, () => Environment.Exit(0))
            {
                UserName = lastAccount?.UserName
            };

            Header = new WalletTabsModel();
            Header.TabItems.Add(new Models.TabItem() { Name = "iota", IsSelected = true });
            Header.TabItems.Add(new Models.TabItem() { Name = "bitcoin" });
            Header.TabItems.Add(new Models.TabItem() { Name = "very very long name" });

            View = new WalletModel();
        }

        private async Task<AccountConfiguration> Login(string userName, SecureString password)
        {
            var passwordHash = Security.EncryptPassword(password.GetString(), "");

            var account = storageManager.GetAccount(userName, passwordHash);
            if(account == null)
            {
                account = await CreateAccount(userName, passwordHash);
                return account;
            }
            else
            {
                return account;
            }
        }

        public Task<AccountConfiguration> CreateAccount(string userName, string passwordHash)
        {
            var tcs = new TaskCompletionSource<AccountConfiguration>();

            var oldView = this.View;
            this.View = new ConfirmPasswordModel(async (model) =>
            {
                await Task.Yield();

                if (tcs.Task.IsCanceled || tcs.Task.IsCompleted || tcs.Task.IsFaulted)
                    return "Something bad happened";

                try
                {
                    if (Security.EncryptPassword(model.Password.GetString(), "") != passwordHash)
                        return "Bad password";

                    var account = storageManager.CreateAccount(userName, passwordHash);

                    // create new wallet
                    account.Wallets = new WalletConfiguration[]
                    {
                        new WalletConfiguration()
                        {
                            PrivateKey = Borlay.Iota.Library.Utils.IotaApiUtils.GenerateRandomTrytes(),
                            WalletType = WalletType.Iota,
                            Name = "Iota"
                        }
                    };

                    storageManager.SaveAccount(passwordHash, account);
                    tcs.SetResult(account);
                    return null;
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }, () =>
            {
                this.View = oldView;
                tcs.SetCanceled();
            });

            return tcs.Task;
        }

        private INotifyPropertyChanged view;
        public INotifyPropertyChanged View
        {
            get
            {
                return view;
            }
            set
            {
                view = value;
                NotifyPropertyChanged();
            }
        }

        private WalletTabsModel header;
        public WalletTabsModel Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
                NotifyPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

        protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
