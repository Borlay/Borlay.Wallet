using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Borlay.Wallet.Models
{
    public class ScanAddressesProgressModel : CancelSyncModel, IUpdateProgress
    {
        public void UpdateProgress(int current, int total)
        {
            base.Text = $"Scanning {current}/{total}";
        }
    }

    public interface IUpdateProgress
    {
        void UpdateProgress(int current, int total);
    }
}
