using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Borlay.Wallet.Models
{
    public interface ISend
    {
        Task SendAsync();
    }

    public class NewSendModel : ModelBase
    {

        private string address;
        private decimal? value;
        private string messageTag;
        private string message;
        private readonly IView view;
        private object oldView;

        public ObservableCollection<AddressItemModel> Addresses { get; set; }

        public NewSendModel(IView view, AddressItemModel[] addresses, Action<NewSendModel> sendTransfer)
        {
            this.view = view;
            this.AddressValidation = null;
            this.Addresses = new ObservableCollection<AddressItemModel>(addresses);
            this.SendCommand = new ActionCommand(o =>
            {
                this.view.View = oldView;
                try
                {
                    sendTransfer(this);
                }
                catch(OperationCanceledException)
                {

                }
                catch(Exception e)
                {
                    this.view.View = this;
                    this.ErrorText = e.Message;
                }
                
            });
            this.CancelCommand = new ActionCommand(o => Cancel());
            this.AddressPasteButton = new IconButtonModel(o =>
            {
                var text= Clipboard.GetText();
                if(!string.IsNullOrWhiteSpace(text))
                    Address = text;
            }, IconType.Paste);
        }

        public void Open()
        {
            if (this.view.View is NewSendModel) return;
            oldView = this.view.View;
            this.view.View = this;
        }

        public void Cancel()
        {
            if (this.view.View != null && this.view.View.Equals(this))
                this.view.View = oldView;
        }

        public Func<string, string> AddressValidation { get; set; }

        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                //if(this.address != value)
                {
                    this.address = AddressValidation?.Invoke(value);
                    NotifyPropertyChanged();
                }
            }
        }

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

        public string MessageTag
        {
            get
            {
                return messageTag;
            }
            set
            {
                if (this.messageTag != value)
                {
                    messageTag = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                if (this.message != value)
                {
                    message = value;
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

        public IconButtonModel AddressPasteButton { get; private set; }

        public IActionCommand SendCommand { get; private set; }
        public IActionCommand CancelCommand { get; private set; }
    }
}
