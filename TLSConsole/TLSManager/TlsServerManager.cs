using System.IO;
using System.Security.Cryptography;
using System.Text;
using DHProtocolLibriary;
using KeysLibryary;

namespace TlsLibriary
{
    public class TlsServerManager
    {
        private readonly DHServer _server = new DHServer();
        private readonly Key _key = new Key();

        public string GetConnectMessage()
        {
            _server.FirstPhase();
            return $"{{{_server.PublicKey}}}:{{{_server.Divider}}}:{{{_server.Generator}}}";
        }

        public void SetConnectAnswer(string answer)
        {
            _server.SecondPhase(answer);
        }

        public string ParseConnectAnswer(string answer)
        {
            answer = answer.Replace("{", "");
            answer = answer.Replace("}", "");
            return answer;
        }

        public void SetKeyOfEncrypt()
        {
            _key.EncryptKey = _server.CommonKey;
            _key.MakeKeyReadOnly();
            _server.ClearGlobalData();
        }

        public byte[] EncryptMessage(string message,ref byte[] IV)
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
    }
}
