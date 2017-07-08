using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models.General
{
    public class ConfirmPasswordModel : LoginModelBase
    {
        private string password;
        private string info;
        protected override string DefaultInfo => "Please confirm your password to create a new account.";

        public ConfirmPasswordModel(Func<ConfirmPasswordModel, Task<string>> loginAction, Action cancelAction)
        {
            Info = DefaultInfo;
            this.LoginCommand = new ActionCommandAsync(async(o) => Info = (await loginAction(this)) ?? DefaultInfo);
            this.CancelCommand = new ActionCommand(o => cancelAction());
        }
    }
}
