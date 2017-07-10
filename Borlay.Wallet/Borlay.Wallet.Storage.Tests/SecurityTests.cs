using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Borlay.Wallet.Storage.Tests
{
    [TestClass]
    public class SecurityTests
    {
        [TestMethod]
        public void ToBytesAndBackTest()
        {
            byte[] bytes = new byte[32];
            System.Security.Cryptography.RNGCryptoServiceProvider.Create().GetBytes(bytes);

            string value = Security.GetString(bytes);
            var bytes2 = Security.GetBytes(value);
            var value2 = Security.GetString(bytes2);

            Assert.AreEqual(value, value2);
        }

        [TestMethod]
        public void EncryptAndValidatePasswordTest()
        {
            var password = Guid.NewGuid().ToString();

            var passwordHash = Security.EncryptPassword(password, "");
            Assert.IsFalse(passwordHash.Contains("."));

            var passwordHashDouble = Security.EncryptPassword(passwordHash);
            Assert.IsTrue(passwordHashDouble.Contains("."), "Hash doesn't contains dot");

            bool isPasswordValid = Security.IsPasswordValid(passwordHash, passwordHashDouble);

            Assert.IsTrue(isPasswordValid, "Password is not valid");
        }

        [TestMethod]
        public void EncryptAndShouldNotValidatePasswordTest()
        {
            var password = Guid.NewGuid().ToString();
            var password2 = Guid.NewGuid().ToString();

            var passwordHash = Security.EncryptPassword(password, "");
            Assert.IsFalse(passwordHash.Contains("."));

            var passwordHash2 = Security.EncryptPassword(password2, "");
            Assert.IsFalse(passwordHash.Contains("."));

            var passwordHashDouble = Security.EncryptPassword(passwordHash);
            Assert.IsTrue(passwordHashDouble.Contains("."), "Hash doesn't contains dot");

            bool isPasswordValid = Security.IsPasswordValid(passwordHash2, passwordHashDouble);

            Assert.IsFalse(isPasswordValid, "Password should not be valid");
        }

        [TestMethod]
        public void EncryptAndDecryptTest()
        {
            var text = Guid.NewGuid().ToString();
            var password = Guid.NewGuid().ToString();

            var encrypted = Security.Encrypt(text, password);
            var decrypted = Security.Decrypt(encrypted, password);

            Assert.AreEqual(text, decrypted);
        }
    }
}
