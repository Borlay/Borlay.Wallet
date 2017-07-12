using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Borlay.Wallet.Models
{
    public interface IText
    {
        string Text { get; set; }
    }

    public class DefaultSyncModel : ModelBase, IText
    {
        private string text;

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
