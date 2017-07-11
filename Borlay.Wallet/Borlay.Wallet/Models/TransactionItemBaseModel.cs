using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class TransactionItemBaseModel : ModelBase
    {
        private string hash;
        private decimal balance;
        private DateTime dateTime;
        private string tag;

        public string Hash
        {
            get
            {
                return hash;
            }
            set
            {
                if (this.hash != value)
                {
                    this.hash = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public decimal Balance
        {
            get
            {
                return balance;
            }
            set
            {
                if (this.balance != value)
                {
                    this.balance = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime DateTime
        {
            get
            {
                return dateTime;
            }
            set
            {
                if (this.dateTime != value)
                {
                    this.dateTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Tag
        {
            get
            {
                return tag;
            }
            set
            {
                if (this.tag != value)
                {
                    this.tag = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
