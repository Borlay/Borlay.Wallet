using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Borlay.Wallet.Models
{
    public class TransactionItemBaseModel : ModelBase
    {
        private string hash;
        private string address;
        private decimal balance;
        private DateTime dateTime;
        private string transactionTag;

        public int Index { get; set; }

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

        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                if (this.address != value)
                {
                    this.address = value;
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

        public string TransactionTag
        {
            get
            {
                return transactionTag;
            }
            set
            {
                if (this.transactionTag != value)
                {
                    this.transactionTag = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool isOwn;
        public bool IsOwn
        {
            get
            {
                return isOwn;
            }
            set
            {
                if (this.isOwn != value)
                {
                    this.isOwn = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool isConfirmed;
        public virtual bool IsConfirmed
        {
            get
            {
                return isConfirmed;
            }
            set
            {
                if (this.isConfirmed != value)
                {
                    this.isConfirmed = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
