using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class ButtonModel : ModelBase
    {
        private string content;
        private IActionCommand buttonClick;

        public ButtonModel(Func<ButtonModel, Task> click)
        {
            this.ButtonClick = new ActionCommandAsync(o => click(this));
        }

        public ButtonModel(Action<ButtonModel> click)
        {
            this.ButtonClick = new ActionCommand(o => click(this));
        }
        public ButtonModel()
        {
        }

        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                if (this.content != value)
                {
                    this.content = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string tipText;
        public string TipText
        {
            get
            {
                return tipText;
            }
            set
            {
                if (this.tipText != value)
                {
                    this.tipText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public IActionCommand ButtonClick
        {
            get
            {
                return buttonClick;
            }
            set
            {
                if (this.buttonClick != value)
                {
                    this.buttonClick = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
