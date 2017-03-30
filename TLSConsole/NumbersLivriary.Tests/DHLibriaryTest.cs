using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;
using DHProtocolLibriary;

namespace NumbersLibriary.Tests
{
    [TestFixture]
    class DHLibriaryTest
    {
        [Test]
        public static void DHLibriary_EqualsKeys_ReturnTrue()
        {
            DHServer server = new DHServer();
            server.FirstPhase();
            DHClient client = new DHClient();
            client.FirstPhase(server.PublicKey,server.Divider,server.Generator);
            server.SecondPhase(client.PublicKey);
            Assert.AreEqual(server.CommonKey,client.CommonKey);
        }
    }
}
