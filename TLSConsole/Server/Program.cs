
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using RsaLibriary;
using TlsLibriary;

namespace Server
{
    public class Program
    {
        private const string ip = "127.0.0.1";
        private const int port = 8001;
        static void Main(string[] args)
        {
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            TlsServerManager serverManager = new TlsServerManager();
            RsaManager rsaManager = new RsaManager();

            serverSocket.Bind(point);
            try
            {
                Console.WriteLine("Server is listeting");
                serverSocket.Listen(1);

                using (var socketHandler = serverSocket.Accept())
                {
                    Console.WriteLine("Connected!");
                    byte[] buffer = new byte[256];
                    StringBuilder builder = new StringBuilder();

                    string serverHello = serverManager.GetConnectMessage();
                    buffer = Encoding.Unicode.GetBytes(serverHello);
                    socketHandler.Send(buffer);

                    do
                    {
                        int bytes = socketHandler.Receive(buffer);
                        builder.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                    } while (socketHandler.Available > 0);

                    string clientHello = builder.ToString();
                    serverManager.SetClientPublicKey(serverManager.ParseConnectAnswer(clientHello));

                    string message;
                    bool isSuccess;
                    while (true)
                    {
                        Console.Write($"{DateTime.Now} You: ");
                        message = Console.ReadLine();
                        socketHandler.Send(SerializateMessage(message, serverManager, rsaManager));

                        while (socketHandler.Available == 0) Thread.Sleep(100);

                        buffer = new byte[socketHandler.Available];
                        socketHandler.Receive(buffer);
                        string answer = DeserializateMessage(buffer, serverManager, rsaManager, out isSuccess);
                        if(isSuccess) Console.WriteLine($"{DateTime.Now} Client: {answer}");
                    }


                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
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
            hashSumCount = new byte[] {(byte) hashSum.Length};
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
                i++,j++)
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
