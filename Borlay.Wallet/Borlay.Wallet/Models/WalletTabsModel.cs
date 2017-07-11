using System;
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

    public interface ISelectedChanged
    {
        event Action<object, bool> SelectedChanged;
    }

    public class TabItem : ModelBase, ISelectedChanged
    {
        public event Action<object, bool> SelectedChanged = (o, a) => { };

        private string name;
        private bool isSelected;

        public TabItem(Action<TabItem> selected)
        {
            this.Selected = selected;
        }

        public TabItem()
        {
        }

        public Action<TabItem> Selected { get; set; }

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
                    if (value)
                        Selected?.Invoke(this);
                    NotifyPropertyChanged();

                    SelectedChanged(this, value);
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
