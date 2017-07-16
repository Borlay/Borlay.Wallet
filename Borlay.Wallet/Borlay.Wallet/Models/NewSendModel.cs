using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public interface ISend
    {
        Task SendAsync();
    }

    public class NewSendModel : ModelBase
    {

        private string address;
        private decimal value;
        private string messageTag;
        private string message;
        private readonly IView view;
        private object oldView;

        public ObservableCollection<AddressItemModel> Addresses { get; set; }

        public NewSendModel(IView view, AddressItemModel[] addresses, Func<NewSendModel, Task> sendTransfer)
        {
            this.view = view;
            this.Addresses = new ObservableCollection<AddressItemModel>(addresses);
            this.SendCommand = new ActionCommandAsync(async o =>
            {
                await sendTransfer(this);
                this.view.View = oldView;
            });
            this.CancelCommand = new ActionCommand(o => Cancel());
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

        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                if(this.address != value)
                {
                    this.address = value;
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

        public IActionCommand SendCommand { get; private set; }
        public IActionCommand CancelCommand { get; private set; }
    }
}
