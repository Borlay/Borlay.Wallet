using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Borlay.Wallet.Models
{
    public class AddressItemModel : ModelBase
    {
        private string address;
        private decimal balance;
        private decimal incomingBalance;
        private decimal outgoingBalance;
        private readonly Func<string, string> addressWithCheckSumConverter;

        public AddressItemModel(Action<AddressItemModel> sendAction, Func<string, string> addressWithCheckSumConverter)
        {
            this.addressWithCheckSumConverter = addressWithCheckSumConverter;
            this.ActionItems = new ObservableCollection<ButtonModel>();

            this.ActionItems.Add(new IconButtonModel((b) => Clipboard.SetText(AddressWithChecksum), IconType.Copy, ColorType.Gray));
            this.ActionItems.Add(new IconButtonModel((b) => sendAction(this), IconType.Sent, ColorType.Blue));

            
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
                    this.AddressWithChecksum = addressWithCheckSumConverter(value);
                    this.address = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string addressWithChecksum;
        public string AddressWithChecksum
        {
            get
            {
                return addressWithChecksum;
            }
            private set
            {
                if (this.addressWithChecksum != value)
                {
                    this.addressWithChecksum = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private bool hasWithdrawal = false;
        public bool HasWithdrawal
        {
            get
            {
                return hasWithdrawal;
            }
            set
            {
                if (this.hasWithdrawal != value)
                {
                    this.hasWithdrawal = value;
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

        public decimal IncomingBalance
        {
            get
            {
                return incomingBalance;
            }
            set
            {
                if (this.incomingBalance != value)
                {
                    this.incomingBalance = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public decimal OutgoingBalance
        {
            get
            {
                return outgoingBalance;
            }
            set
            {
                if (this.outgoingBalance != value)
                {
                    this.outgoingBalance = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<ButtonModel> ActionItems { get; private set; }
    }
}
