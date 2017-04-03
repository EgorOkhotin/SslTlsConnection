using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using KeysLibriary;

namespace TlsLibriary
{
    public class TlsManager
    {
        private readonly Key _key = new Key();

        internal Key KeyOfEncrypt => _key;

        /// <summary>
        /// Encrypt message
        /// </summary>
        /// <param name="message">Text whic must be encrypt</param>]
        /// <param name="IV">Initialization vector (16 bytes)</param>
        /// <param name="hashSumMessage">Hash sum of message</param>
        /// <returns>Encrypted message(in byte[])</returns>
        public byte[] EncryptMessage(string message, out byte[] IV, out byte[] hashSumMessage)
        {
            hashSumMessage = ComputeHashSum(message);
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
        /// Encrypt message
        /// </summary>
        /// <param name="message">Text whic must be encrypt</param>]
        /// <param name="IV">Initialization vector (16 bytes)</param>
        /// <returns>Encrypted message(in byte[])</returns>
        public byte[] EncryptMessage(string message, out byte[] IV)
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
            var result = ComputeHashSum(decryptMessage);

            if (result.SequenceEqual(hashSumMessage)) isSuccess = true;
            else isSuccess = false;

            return decryptMessage;
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

        public static byte[] ComputeHashSum(string message)
        {
            return (new SHA1Managed()).ComputeHash(Encoding.Unicode.GetBytes(message));
        }
    }
}
