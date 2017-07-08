using Borlay.Wallet.Models.General;
using Borlay.Wallet.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly StorageManager storageManager = new StorageManager();

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            this.view = new UserLoginModel(async (model) =>
            {
                try
                {
                    var account = await Login(model.UserName, model.Password);
                    // do loged stuffs
                    return null;
                }
                catch(Exception e)
                {
                    return e.Message;
                }
            }, ()=> Environment.Exit(0));
        }

        private async Task<AccountConfiguration> Login(string userName, string password)
        {
            var passwordHash = Security.EncryptPassword(password, "");

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
                    if (Security.EncryptPassword(model.Password, "") != passwordHash)
                        return "Bad password";

                    var account = storageManager.CreateAccount(userName, passwordHash);
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


        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

        protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
