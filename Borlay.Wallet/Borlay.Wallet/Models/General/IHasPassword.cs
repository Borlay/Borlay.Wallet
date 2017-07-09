using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models.General
{
    public interface IHasPassword
    {
        SecureString Password { get; set; }

        int PasswordLength { get; set; }
    }
}
