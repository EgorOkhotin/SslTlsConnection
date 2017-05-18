using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using RsaLibriary;
using TlsLibriary;

namespace Client
{
    class Program
    {
        private const string ip = "127.0.0.1";
        private const int port = 8001;
        static void Main(string[] args)
        {
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            TlsClientManager clientManager = new TlsClientManager();
            RsaManager rsaManager = new RsaManager();

            try
            {
                clientSocket.Connect(point);

                StringBuilder builder = new StringBuilder();
                byte[] buffer = new byte[256];

                do
                {
                    int bytes = clientSocket.Receive(buffer);
                    builder.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                } while (clientSocket.Available > 0);
                clientManager.ParseSetConnectMessage(builder.ToString());
                builder.Clear();

                clientSocket.Send(Encoding.Unicode.GetBytes(clientManager.GetConnectAnswer()));

                while (true)
                {
                    while (clientSocket.Available == 0) Thread.Sleep(100);

                    buffer = new byte[clientSocket.Available];
                    clientSocket.Receive(buffer);
                    bool isSuccess;
                    string message = DeserializateMessage(buffer, clientManager, rsaManager, out isSuccess);
                    if(isSuccess) Console.WriteLine($"{DateTime.Now} Server: {message}");

                    Console.Write($"{DateTime.Now} You: ");
                    string answer = Console.ReadLine();
                    clientSocket.Send(SerializateMessage(answer, clientManager, rsaManager));
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }
        public static byte[] SerializateMessage(string message, TlsManager tlsManager, RsaManager rsaManager)
        {
            byte[] IV,
                hashSum,
                digitalSignature,
                hashSumCount,
                messageBytes;
            string publicRSAKey = rsaManager.PublicKey;
            message = $"|rsaKeyStart|{publicRSAKey}|rsaKeyEnd|{message}";

            messageBytes = tlsManager.EncryptMessage(message, out IV, out hashSum);
            hashSumCount = new byte[] { (byte)hashSum.Length };
            digitalSignature = rsaManager.GetDigitalSignature(hashSum);

            return IV.Concat(hashSumCount.Concat(hashSum.Concat(digitalSignature.Concat(messageBytes)))).ToArray();
        }

        public static string DeserializateMessage(byte[] message, TlsManager tlsManager, RsaManager rsaManager, out bool isAllSuccess)
        {
            isAllSuccess = false;
            byte[] IV = new byte[16],
                   hashSum,
                   digitalSignature = new byte[256],
                   messageBytes;

            for (int i = 0; i < IV.Length; i++) IV[i] = message[i];

            hashSum = new byte[message[16]];

            for (int i = IV.Length + 1, j = 0; j < hashSum.Length; i++, j++)
                hashSum[j] = message[i];

            for (int i = IV.Length + 1 + hashSum.Length, j = 0; j < digitalSignature.Length; i++, j++)
                digitalSignature[j] = message[i];

            messageBytes = new byte[message.Length - IV.Length - 1 - hashSum.Length - digitalSignature.Length];

            for (int i = IV.Length + 1 + hashSum.Length + digitalSignature.Length, j = 0;
                j < messageBytes.Length;
                i++, j++)
                messageBytes[j] = message[i];

            bool isSuccess;
            string decryptMessage = tlsManager.DecryptMessage(messageBytes, IV, hashSum, out isSuccess);
            string rsaNotParsedKey = decryptMessage.Substring(decryptMessage.IndexOf("|rsaKeyStart|"),
                decryptMessage.LastIndexOf("|rsaKeyEnd|") + "|rsaKeyEnd|".Length);
            string rsaKey = rsaNotParsedKey.Replace("|rsaKeyStart|", "");
            rsaKey = rsaKey.Replace("|rsaKeyEnd|", "");
            bool isValidDigitalSignature = rsaManager.IsValidDigitalSignature(hashSum, digitalSignature, rsaKey);

            decryptMessage = decryptMessage.Replace(rsaNotParsedKey, "");

            if ((isSuccess == true) & (isValidDigitalSignature == true)) isAllSuccess = true;

            return decryptMessage;
        }
    }
}
