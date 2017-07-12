using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Borlay.Wallet.Models.General
{
    public interface IUserNameCredentials
    {
        string UserName { get; }
        SecureString Password { get; }
    }

    public class UserLoginModel : LoginModelBase, IUserNameCredentials
    {
        private string userName;
        protected override string DefaultInfo => "Login to your wallet using login name and password. Remember your credentials as we can't restore it.";

        public UserLoginModel(Func<UserLoginModel, Task<string>> loginAction, Action cancelAction)
        {
            Info = DefaultInfo;
            this.LoginCommand = new ActionCommandAsync(async (o) => Info = (await loginAction(this)) ?? DefaultInfo);
            this.CancelCommand = new ActionCommand(o => cancelAction());
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
    }
}
