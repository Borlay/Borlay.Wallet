using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Storage
{
    public static class Security
    {
        public static string EncryptPassword(string password)
        {
            byte[] salt = new byte[32];
            System.Security.Cryptography.RNGCryptoServiceProvider.Create().GetBytes(salt);

            return EncryptPassword(password, salt);
        }

        public static string EncryptPassword(string password, string saltText)
        {
            var salt = GetBytes(saltText);
            return EncryptPassword(password, salt);
        }

        public static string EncryptPassword(string password, byte[] salt)
        {
            // Convert the plain string pwd into bytes
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(password);
            // Append salt to pwd before hashing
            byte[] combinedBytes = new byte[plainTextBytes.Length + salt.Length];
            System.Buffer.BlockCopy(plainTextBytes, 0, combinedBytes, 0, plainTextBytes.Length);
            System.Buffer.BlockCopy(salt, 0, combinedBytes, plainTextBytes.Length, salt.Length);

            // Create hash for the pwd+salt
            System.Security.Cryptography.HashAlgorithm hashAlgo = new System.Security.Cryptography.SHA256Managed();
            byte[] hash = hashAlgo.ComputeHash(combinedBytes);

            var hashString = GetString(hash); 
            var saltString = GetString(salt); 

            var passwordHash = string.IsNullOrWhiteSpace(saltString) ? hashString : $"{hashString}.{saltString}";

            var hashBytes = GetBytes(hashString).ToArray();
            var hashString2 = GetString(hashBytes);

            if (hashString != hashString2)
                throw new SecurityException("Hashes are not equal. There is some bug.");

            return passwordHash;
        }

        public static byte[] GetBytes(string value)
        {
            List<byte> bytes = new List<byte>();

            if (string.IsNullOrWhiteSpace(value))
                return bytes.ToArray();

            while(value.Length > 1)
            {
                var b = value.Substring(0, 2);
                value = value.Substring(2);
                bytes.Add(Convert.ToByte(b, 16));
            }
            return bytes.ToArray();
        }

        public static string GetString(byte[] bytes)
        {
            var value = BitConverter.ToString(bytes).Replace("-", "");
            return value;
        }


        public static bool IsPasswordValid(string password, string hash)
        {
            var saltText = hash.Split('.')[1];
            var goodHash = EncryptPassword(password, saltText);

            return goodHash == hash;
        }

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;

                                var saltString = GetString(saltStringBytes);
                                var ivString = GetString(ivStringBytes);
                                var hashString = GetString(memoryStream.ToArray());
                                memoryStream.Close();
                                cryptoStream.Close();

                                return $"{hashString}.{ivString}.{saltString}";
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            var cipherTextSplited = cipherText.Split('.');
            var hashString = cipherTextSplited[0];
            var ivString = cipherTextSplited[1];
            var saltString = cipherTextSplited[2];

            var cipherTextBytes = GetBytes(hashString);
            var ivStringBytes = GetBytes(ivString);
            var saltStringBytes = GetBytes(saltString);

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}
