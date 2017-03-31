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
            var encryptManag = serverManager.EncryptMessage(message, ref IV);
            string result = clientManager.DecryptMessage(encryptManag, IV);
            Assert.AreEqual(message,result);
        }
    }
}
