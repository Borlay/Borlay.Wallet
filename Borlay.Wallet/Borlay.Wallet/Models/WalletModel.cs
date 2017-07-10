﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class WalletModel : ModelBase
    {
        public WalletModel()
        {
            this.MenuItems = new ObservableCollection<TabItem>();
            this.MenuItems.Add(new TabItem() { Name = "Addresses", IsSelected = true });
            this.MenuItems.Add(new TabItem() { Name = "Transactions" });
        }


        public ObservableCollection<TabItem> MenuItems { get; private set; }
    }
}
