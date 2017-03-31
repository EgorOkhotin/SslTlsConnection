using NUnit.Framework;
using TlsLibriary;

namespace TlsConnect.Tests
{
    [TestFixture]
    class TlsServerManagerTest
    {
        [Test]
        [TestCase("{123790}")]
        public void ParseConnectManager_GetNumber_ReturnTrue(string answer)
        {
            TlsServerManager manager = new TlsServerManager();
            Assert.AreEqual("123790",manager.ParseConnectAnswer(answer));
        }


    }

    [TestFixture]
    class TlsCompleteTest
    {
        [Test]
        [TestCase("Helloworld")]
        public void CompleteTest_EncryptAndDecryptMessage_ReturnTrue(string message)
        {
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
    }
}
