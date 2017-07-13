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
    public partial class MainWindow : Window, INotifyPropertyChanged, ISyncView
    {
        private readonly AccountStorageManager storageManager = new AccountStorageManager();
        private readonly SyncModel syncModel;
        

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            this.syncModel = new SyncModel(this);
            // to remember ListCollectionView

            var accounts = storageManager.GetAccounts();
            var lastAccount = accounts?.Accounts?.OrderByDescending(o => o.LastLoginDate).FirstOrDefault();

            this.view = new UserLoginModel(async (model) =>
            {
                try
                {
                    var account = await Login(model.UserName, model.Password);
                    await Loged(account);

                    this.CreateWalletButton = new IconButtonModel((b) => CreateWalletAsync(model), IconType.Plus);

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
        }

        private async Task CreateWalletAsync(IUserNameCredentials credentials)
        {
            var oldView = this.View;

            var createWalletModel = new CreateWalletModel();
            this.View = createWalletModel;

            try
            {
                var wallet = await createWalletModel.CreateWallet();
                var syncContent = await syncModel.EnterSync();
                try
                {
                    syncContent.SetTextView("Your wallet is about to ready. We are creating the first address for you.");

                    var tabItem = new Models.TabItem()
                    {
                        Name = wallet.Name,
                        IsSelected = false,
                    };
                    var walletProvider = new Iota.IotaWalletManager(wallet, tabItem);

                    await walletProvider.EnsureFirstAddressAsync();

                    if (wallet.IsImported)
                    {
                        walletProvider.InitializeAsync(true);
                        //var cancelSync = syncContent
                        //    .SetCancelView("Since you are creating your wallet with an existing key we area searching for addresses. You can cancel if you know that there is no addresses.");
                        //try
                        //{
                            
                        //}
                        //catch(OperationCanceledException)
                        //{
                        //    // do nothing
                        //}
                    }

                    //storageManager.SaveAccount

                    tabItem.Selected = (t) => View = walletProvider.Wallet;
                    Header.TabItems.Add(tabItem);
                    foreach (var tab in Header.TabItems) // don't know why but need
                        tab.IsSelected = false;
                    tabItem.IsSelected = true;
                    return;
                }
                catch(OperationCanceledException)
                {
                    // do nothing
                }
                catch(Exception e)
                {
                    await syncContent.SetCancelView(e.Message).WaitAsync();
                }
                finally
                {
                    syncContent.Dispose();
                }
                
            }
            catch (OperationCanceledException)
            {
                // do nothing
            }
            this.View = oldView;
        }

        private async Task Loged(AccountConfiguration account)
        {
            Header.TabItems.Clear();

            foreach(var wallet in account.Wallets)
            {
                if (wallet.IsActive)
                {
                    var tabItem = new Models.TabItem()
                    {
                        Name = wallet.Name,
                        IsSelected = false,
                    };
                    var walletProvider = new Iota.IotaWalletManager(wallet, tabItem);
                    tabItem.Selected = (t) => View = walletProvider.Wallet;
                    Header.TabItems.Add(tabItem);
                }
            }
            var tab = Header.TabItems.FirstOrDefault();
            if (tab != null)
                tab.IsSelected = true;
            else
                View = null;
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

                    account.Wallets = new WalletConfiguration[]
                    {
                        new WalletConfiguration()
                        {
                            PrivateKey = Borlay.Iota.Library.Utils.IotaApiUtils.GenerateRandomTrytes(),
                            WalletType = WalletType.Iota,
                            Name = WalletType.Iota.ToString()
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

        private object view;
        public object View
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

        private object syncView;
        public object SyncView
        {
            get
            {
                return syncView;
            }
            set
            {
                syncView = value;
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

        private IconButtonModel createWalletButton;
        public IconButtonModel CreateWalletButton
        {
            get
            {
                return createWalletButton;
            }
            set
            {
                createWalletButton = value;
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
