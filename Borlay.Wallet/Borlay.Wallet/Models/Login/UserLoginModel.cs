using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Borlay.Wallet.Models.Login
{
    public class UserLoginModel : ModelBase
    {
        private string userName;
        private string password;
        private string info;

        public UserLoginModel(Action<UserLoginModel> loginAction)
        {
            Info = "Login to your wallet using login name and password. Remember your credentials as we can't restore it.";
            this.LoginCommand = new ActionCommand(() => loginAction(this));
        }

        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                if (value != this.userName)
                {
                    this.userName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                if (value != this.password)
                {
                    this.password = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Info
        {
            get
            {
                return this.info;
            }
            set
            {
                if (value != this.info)
                {
                    this.info = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand LoginCommand
        {
            get; set;
        }
    }
}
