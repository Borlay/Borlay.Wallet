using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public interface IScanAddresses
    {
        Task ScanAddressesAsync(IUpdateProgress updateProgress, bool force, CancellationToken cancellationToken);
    }

    public class ScanAddressesModel : ModelBase
    {
        private readonly ButtonModel scanButton;
        private readonly IScanAddresses scanAddresses;

        public ScanAddressesModel(IScanAddresses scanAddresses)
        {
            this.scanAddresses = scanAddresses;
            this.scanButton = new TextButtonModel(b => ScanAddressesAsync(true)) { Content = "Scan" };
            this.View = scanButton;
        }

        public async Task ScanAddressesAsync(bool force)
        {
            var scanProgress = new ScanAddressesProgressModel();
            this.View = scanProgress;
            try
            {
                await scanAddresses.ScanAddressesAsync(scanProgress, force, scanProgress.Token);
            }
            finally
            {
                this.View = scanButton;
            }
        }

        private object view;
        public object View
        {
            get
            {
                return view;
            }
            set
            {
                view = value;
                NotifyPropertyChanged();
            }
        }
    }
}
