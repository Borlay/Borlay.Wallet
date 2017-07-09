using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet
{
    public static class SecureExtensions
    {
        public static string GetString(this SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        public static SecureString ConvertToSecureString(string valueToSecureString)
        {
            if (valueToSecureString == null)
                throw new ArgumentNullException(nameof(valueToSecureString));

            var securePassword = new SecureString();

            foreach (char c in valueToSecureString)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }
    }
}
