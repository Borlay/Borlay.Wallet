﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class WalletTabsModel
    {
        public WalletTabsModel()
        {
            TabItems = new ObservableCollection<TabItem>();
        }

        public ObservableCollection<TabItem> TabItems { get; private set; }
    }

    public class TabItem : ModelBase
    {
        private string name;
        private bool isSelected;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if(this.name != value)
                {
                    this.name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
