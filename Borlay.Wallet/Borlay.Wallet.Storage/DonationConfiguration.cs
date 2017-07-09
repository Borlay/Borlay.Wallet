using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Storage
{
    public class DonationConfiguration
    {
        public Guid UserGuid { get; set; }

        public string Message { get; set; }

        public WalletType WalletType { get; set; }

        public DateTime DateTime { get; set; }
    }

    public class DonationCollectionConfiguration
    {

    }
}
