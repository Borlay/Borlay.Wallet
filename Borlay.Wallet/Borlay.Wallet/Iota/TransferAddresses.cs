using Borlay.Iota.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Iota
{
    public class TransferAddresses
    {
        public string ReceiveAddress { get; set; }

        public string Remainder { get; set; }

        public AddressItem[] SendAddresses { get; set; }
    }
}
