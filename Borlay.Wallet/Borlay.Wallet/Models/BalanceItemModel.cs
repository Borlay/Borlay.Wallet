using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class BalanceItemModel : ModelBase
    {
        private string currency;
        private decimal value;
        private decimal rate;
        private bool current;

        public string Currency
        {
            get
            {
                return currency;
            }
            set
            {
                if (this.currency != value)
                {
                    this.currency = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public decimal Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public decimal Rate
        {
            get
            {
                return rate;
            }
            set
            {
                if (this.rate != value)
                {
                    this.rate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool Current
        {
            get
            {
                return current;
            }
            set
            {
                if (this.current != value)
                {
                    this.current = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //public string Sufix { get; set; }
    }
}
