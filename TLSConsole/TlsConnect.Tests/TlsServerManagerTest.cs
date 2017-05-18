using System;
using System.Text;
using NUnit.Framework;
using TlsLibriary;

namespace TlsConnect.Tests
{
    [TestFixture]
    class TlsServerManagerTest
    {
        [Test]
        public void ParseConnectManager_GetNumber_ReturnTrue()
        {
            string answer = (new Random()).Next(0,int.MaxValue).ToString();
            TlsServerManager manager = new TlsServerManager();
            Assert.AreEqual(answer,manager.ParseConnectAnswer(answer));
        }


    }

    [TestFixture]
    class TlsCompleteTest
    {
        [Test]
        public void CompleteTest_EncryptAndDecryptMessage_ReturnTrue()
        {
            string message = RandomString((new Random()).Next(10, 1000));
            TlsServerManager serverManager = new TlsServerManager();
            string connectMessage = serverManager.GetConnectMessage();
            TlsClientManager clientManager = new TlsClientManager();
            clientManager.ParseSetConnectMessage(connectMessage);
            var answer = serverManager.ParseConnectAnswer(clientManager.GetConnectAnswer());
            serverManager.SetClientPublicKey(answer);
            var IV = new byte[16];
            byte[] hashSum = null;
            bool isSuccess;
            var encryptManag = serverManager.EncryptMessage(message, out IV, out hashSum);
            string result = clientManager.DecryptMessage(encryptManag, IV, hashSum, out isSuccess);
            Assert.IsTrue(isSuccess);
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
