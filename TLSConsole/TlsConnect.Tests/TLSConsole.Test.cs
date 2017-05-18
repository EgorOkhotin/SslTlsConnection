using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using RsaLibriary;
using TlsLibriary;

namespace TlsConnect.Tests
{
    [TestFixture]
    class TLSConsoleTest
    {
        [Test]
        public void TestSerialization()
        {
            string message = RandomString((new Random()).Next(10, 1000));
            TlsServerManager serverManager = new TlsServerManager();
            string connectMessage = serverManager.GetConnectMessage();
            TlsClientManager clientManager = new TlsClientManager();
            clientManager.ParseSetConnectMessage(connectMessage);
            var answer = serverManager.ParseConnectAnswer(clientManager.GetConnectAnswer());
            clientManager.ClearData();
            serverManager.SetClientPublicKey(answer);
            RsaManager rsaManager = new RsaManager();
            byte[] resultSerialize = Server.Program.SerializateMessage(message, serverManager, rsaManager);
            bool isSuccess;
            string resultString = Server.Program.DeserializateMessage(resultSerialize, serverManager, rsaManager, out isSuccess);
            Assert.AreEqual(message,resultString);

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

