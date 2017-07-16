﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class AddressItemModel : ModelBase
    {
        private string address;
        private decimal balance;
        private decimal incomingBalance;
        private decimal outgoingBalance;

        public AddressItemModel(Action<AddressItemModel> sendAction)
        {
            this.ActionItems = new ObservableCollection<ButtonModel>();

            this.ActionItems.Add(new IconButtonModel((b) => sendAction(this), IconType.Sent, ColorType.Blue));
        }

        public void SetTransactions()
        {

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
