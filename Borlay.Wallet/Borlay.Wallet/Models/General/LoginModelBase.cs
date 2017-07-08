using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Borlay.Wallet.Models.General
{
    public abstract class LoginModelBase : ModelBase
    {
        private string password;
        private string info;
        protected abstract string DefaultInfo { get; } 

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

        public ICommand CancelCommand
        {
            get; set;
        }
    }
}
