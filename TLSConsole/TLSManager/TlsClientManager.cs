using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using KeysLibriary;
using DHProtocolLibriary;

namespace TlsLibriary
{
    public class TlsClientManager
    {
        private DHClient _client;
        private readonly Key _key = new Key();

        /// <summary>
        /// Parse and set connect data from server
        /// </summary>
        /// <param name="message">message from server</param>
        public void ParseSetConnectMessage(string message)
        {
            List<string> stringValues = message.Split(':').ToList();
            SetConnectMessage(ParseConnectPhrase(stringValues[0]), ParseConnectPhrase(stringValues[1]), ParseConnectPhrase(stringValues[2]));
        }

        /// <summary>
        /// Public key for server
        /// </summary>
        /// <returns>message for server</returns>
        public string GetConnectAnswer()
        {
            return $"{{{_client.PublicKey}}}";
        }

        /// <summary>
        /// Encrypt message
        /// </summary>
        /// <param name="message">Text whic must be encrypt</param>]
        /// <param name="IV">Initialization vector (16 bytes)</param>
        /// <returns>Encrypted message(in byte[])</returns>
        public byte[] EncryptMessage(string message, byte[] IV)
        {
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
        /// <returns>Decrypted message</returns>
        public string DecryptMessage(byte[] encryptMessage, byte[] IV)
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

            return decryptMessage;
        }

        private void SetConnectMessage(string key,string divider,string generator)
        {
            _client = new DHClient(key, divider, generator);
            _key.EncryptKey = _client.CommonKey;
            _key.MakeKeyReadOnly();
            _client.ClearGlobalData();
        }

        private string ParseConnectPhrase(string phrase)
        {
            phrase = phrase.Replace("{", "");
            phrase = phrase.Replace("}", "");
            return phrase;
        }
    }
}
