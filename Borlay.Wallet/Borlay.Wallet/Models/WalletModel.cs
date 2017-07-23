using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public interface IView
    {
        object View { get; set; }
    }

    public class WalletModel : ModelBase, IView, IOpenDonation
    {
        public event Action<WalletModel> NewSend = (w) => { };
        private readonly IWalletManager walletManager;

        public WalletModel(IWalletManager walletManager, params TabItem[] menuItems)
            : this(walletManager, new BalanceStatsModel(), menuItems)
        {
        }

        public WalletModel(IWalletManager walletManager, BalanceStatsModel balanceStatsModel, params TabItem[] menuItems)
        {
            this.MenuItems = new ObservableCollection<TabItem>();
            this.BalanceStats = new BalanceStatsModel();
            this.walletManager = walletManager;
            this.SendCommand = new ActionCommand((s) => NewSend(this));

            this.ScanAddresses = new ScanAddressesModel(walletManager);
            this.SyncModels = new ObservableCollection<CancelSyncModel>();

            foreach (var item in menuItems)
                this.MenuItems.Add(item);
        }

        public ScanAddressesModel ScanAddresses { get; private set; }

        public BalanceStatsModel BalanceStats { get; private set; }

        public ObservableCollection<TabItem> MenuItems { get; private set; }

        public IActionCommand SendCommand { get; set; }

        public ObservableCollection<CancelSyncModel> SyncModels { get; set; }

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

        public void OpenDonation()
        {
            walletManager.OpenDonation();
        }
    }
}
