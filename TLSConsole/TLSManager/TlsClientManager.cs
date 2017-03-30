using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using KeysLibryary;
using DHProtocolLibriary;

namespace TlsLibriary
{
    public class TlsClientManager
    {
        private readonly DHClient _client = new DHClient();
        private readonly Key _key = new Key();

        public void ParseSetConnectMessage(string message)
        {
            List<string> stringValues = message.Split(':').ToList();
            SetConnectMessage(ParseConnectPhrase(stringValues[0]), ParseConnectPhrase(stringValues[1]), ParseConnectPhrase(stringValues[2]));
        }

        public string GetConnectAnswer()
        {
            return $"{{{_client.PublicKey}}}";
        }

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
            _client.FirstPhase(key, divider, generator);
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
