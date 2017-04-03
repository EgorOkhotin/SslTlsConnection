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
        [TestCase("Hello World!")]
        [TestCase("This is Test")]
        [TestCase("This is Sparta")]
        public void RsaManager_EqualStrings_ReturnTrue(string message)
        {
            RsaManager rsaManager1 = new RsaManager();
            RsaManager rsaManager2 = new RsaManager();
            var encryptMessage = rsaManager1.EncryptMessage(message, false, rsaManager2.PublicKey);
            var result = rsaManager2.DecryptMessage(encryptMessage,false);
            Assert.AreEqual(message,result);
        }

        [Test]
        [TestCase("Hello World!")]
        [TestCase("This is Test")]
        [TestCase("This is Sparta")]
        public void RsaManager_EqualHash_ReturnTrue(string message)
        {
            SHA1Managed managed = new SHA1Managed();

            var hashSum = managed.ComputeHash(Encoding.Unicode.GetBytes(message));
            RsaManager manager1 = new RsaManager();
            RsaManager manager2 = new RsaManager();
            var digitalSignature = manager1.GetDigitalSignature(hashSum);
            Assert.IsTrue(manager2.IsValidDigitalSignature(hashSum, digitalSignature, manager1.PublicKey));
        }
    }
}
