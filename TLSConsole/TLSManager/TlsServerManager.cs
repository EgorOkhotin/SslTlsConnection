using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DHProtocolLibriary;
using KeysLibriary;

namespace TlsLibriary
{
    public class TlsServerManager
    {
        private DHServer _server = new DHServer();
        private readonly Key _key = new Key();

        /// <summary>
        /// Message for client of server
        /// </summary>
        /// <returns>message for client</returns>
        public string GetConnectMessage()
        {
            return $"{{{_server.PublicKey}}}:{{{_server.Divider}}}:{{{_server.Generator}}}";
        }

        /// <summary>
        /// Set value of answer from client
        /// </summary>
        /// <param name="answer"></param>
        public void SetClientPublicKey(string answer)
        {
            _server.CalculateKey(answer);
        }

        /// <summary>
        /// Parse answer from client
        /// </summary>
        /// <param name="answer">Aswer of client on connect message</param>
        /// <returns>parsed answer</returns>
        public virtual string ParseConnectAnswer(string answer)
        {
            answer = answer.Replace("{", "");
            answer = answer.Replace("}", "");
            return answer;
        }

        /// <summary>
        /// Set key for encryption
        /// </summary>
        public void SetKeyOfEncrypt()
        {
            _key.EncryptKey = _server.CommonKey;
            _key.MakeKeyReadOnly();
            _server.ClearGlobalData();
        }

        /// <summary>
        /// Encrypt message
        /// </summary>
        /// <param name="message">Text whic must be encrypt</param>]
        /// <param name="IV">Initialization vector (16 bytes)</param>
        /// <param name="hashSumMessage">Hash sum of message</param>
        /// <returns>Encrypted message(in byte[])</returns>
        public byte[] EncryptMessage(string message, out byte[] IV, out byte[] hashSumMessage)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            hashSumMessage = (new SHA1Managed()).ComputeHash(data);

            byte[] encryptedMessage;
            using (AesManaged manager = new AesManaged())
            {
                var bytesKey = Encoding.Unicode.GetBytes(_key.EncryptKey);
                byte[] keyInBytes = new byte[32];
                for (int i = 0; i < keyInBytes.Length; i++) keyInBytes[i] = bytesKey[i];
                manager.Key = keyInBytes;
                manager.GenerateIV();
                IV = manager.IV;

                ICryptoTransform encryptor = manager.CreateEncryptor();
                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream cryptStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptStream))
                        {
                            writer.Write(message);
                        }
                    }
                    encryptedMessage = stream.ToArray();
                }
            }
            return encryptedMessage;
        }

        /// <summary>
        /// Decrypt message
        /// </summary>
        /// <param name="encryptMessage">text whic must be decrypt</param>
        /// <param name="IV">Initialization vector(16 bytes)</param>
        /// <param name="hashSumMessage">Hash sum of message</param>
        /// <param name="isSuccess">(hash sum message) == (decrypt hash sum message)</param>
        /// <returns>Decrypted message</returns>
        public string DecryptMessage(byte[] encryptMessage, byte[] IV, byte[] hashSumMessage, out bool isSuccess)
        {
            string decryptMessage;
            using (AesManaged manager = new AesManaged())
            {
                var bytesKey = Encoding.Unicode.GetBytes(_key.EncryptKey);
                byte[] keyInBytes = new byte[32];
                for (int i = 0; i < keyInBytes.Length; i++) keyInBytes[i] = bytesKey[i];

                manager.Key = keyInBytes;
                manager.IV = IV;

                ICryptoTransform decrypt = manager.CreateDecryptor();
                using (MemoryStream stream = new MemoryStream(encryptMessage))
                {
                    using (CryptoStream decryptStream = new CryptoStream(stream, decrypt, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(decryptStream))
                        {
                           decryptMessage = reader.ReadToEnd();
                        }
                    }
                }
            }
            byte[] data = Encoding.Unicode.GetBytes(decryptMessage);
            var result = (new SHA1Managed()).ComputeHash(data);

            if (result.SequenceEqual(hashSumMessage)) isSuccess = true;
            else isSuccess = false;

            return decryptMessage;
        }
    }
}
