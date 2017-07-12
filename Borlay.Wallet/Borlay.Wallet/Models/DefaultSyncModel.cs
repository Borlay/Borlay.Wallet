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

    public class CancelSyncModel : DefaultSyncModel
    {
        private readonly CancellationTokenSource cts;
        private readonly TaskCompletionSource<bool> tcs;

        public CancelSyncModel()
        {
            this.cts = new CancellationTokenSource();
            this.tcs = new TaskCompletionSource<bool>();
            cts.Token.Register(() => tcs.SetResult(true));
            this.CancelCommand = new ActionCommand(o => cts.Cancel());
        }

        public Task WaitAsync()
        {
            return tcs.Task;
        }

        public CancellationToken Token => cts.Token;
        public ICommand CancelCommand { get; private set; }
    }
}
