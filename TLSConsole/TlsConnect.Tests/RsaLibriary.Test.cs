using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RsaLibriary;

namespace TlsConnect.Tests
{
    [TestFixture]
    class RsaLibriary
    {
        [Test]
        public void RsaManager_EqualStrings_ReturnTrue()
        {
            string message = RandomString((new Random()).Next(10, 100));

            RsaManager rsaManager1 = new RsaManager();
            RsaManager rsaManager2 = new RsaManager();
            var encryptMessage = rsaManager1.EncryptMessage(message, false, rsaManager2.PublicKey);
            var result = rsaManager2.DecryptMessage(encryptMessage,false);
            Assert.AreEqual(message,result);
        }

        [Test]
        public void RsaManager_EqualHash_ReturnTrue()
        {
            string message = RandomString((new Random()).Next(10, 1000));

            SHA1Managed managed = new SHA1Managed();

            var hashSum = managed.ComputeHash(Encoding.Unicode.GetBytes(message));
            RsaManager manager1 = new RsaManager();
            RsaManager manager2 = new RsaManager();
            var digitalSignature = manager1.GetDigitalSignature(hashSum);
            Assert.IsTrue(manager2.IsValidDigitalSignature(hashSum, digitalSignature, manager1.PublicKey));
        }

        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}
