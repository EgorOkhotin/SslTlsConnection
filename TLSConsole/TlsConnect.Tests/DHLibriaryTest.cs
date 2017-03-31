using NUnit.Framework;
using DHProtocolLibriary;

namespace TlsConnect.Tests
{
    [TestFixture]
    class DHLibriaryTest
    {
        [Test]
        public static void DHLibriary_EqualsKeys_ReturnTrue()
        {
            DHServer server = new DHServer();
            DHClient client = new DHClient(server.PublicKey, server.Divider, server.Generator);
            server.CalculateKey(client.PublicKey);
            Assert.AreEqual(server.CommonKey,client.CommonKey);
        }
    }
}
