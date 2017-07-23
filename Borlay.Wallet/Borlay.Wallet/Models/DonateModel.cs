using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class DonateModel : ModelBase
    {
        public DonateModel(Action<DonateModel, string, decimal> sendAction, Action<DonateModel> cancelAction)
        {
            this.SendCommand = new ActionCommand((o) =>
            {
                var donateAddress = "YQQYYZPTDMAFSYIEQZVAZYVSCRRKONMACBDA9FFJKNLWVHUKKPBOBHAUPJWDNXXBUYTIVELYIJX9GPCTZOSXOMVSLS";
                if (value.HasValue && value.Value > 0)
                    sendAction(this, donateAddress, value.Value);
                else
                    ErrorText = "Value should be more than zero";
            });
            this.CancelCommand = new ActionCommand((o) => cancelAction(this));
        }

        private decimal? value;
        public decimal? Value
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

        private string errorText;
        public string ErrorText
        {
            get
            {
                return errorText;
            }
            set
            {
                if (this.errorText != value)
                {
                    errorText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public IActionCommand SendCommand { get; private set; }
        public IActionCommand CancelCommand { get; private set; }
    }
}
