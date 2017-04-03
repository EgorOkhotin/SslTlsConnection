using System.Security.Cryptography;
using System.Text;

namespace RsaLibriary
{
    public class RsaManager
    {
        private RSACryptoServiceProvider _rsa = new RSACryptoServiceProvider(2048);

        /// <summary>
        /// Your public key for encryption
        /// </summary>
        public string PublicKey { get; }

        /// <summary>
        /// New manager
        /// </summary>
        public RsaManager()
        {
            PublicKey = _rsa.ToXmlString(false);
        }

        /// <summary>
        /// Encrypt message
        /// </summary>
        /// <param name="message">Test which must be encrypted</param>
        /// <param name="fOAEP">Support OAEP or not</param>
        /// <param name="publicKey">Public key for encryption</param>
        /// <returns>Encrypted data</returns>
        public byte[] EncryptMessage(string message, bool fOAEP, string publicKey)
        {
            _rsa.FromXmlString(publicKey);
            return _rsa.Encrypt(Encoding.Unicode.GetBytes(message), fOAEP);
        }

        /// <summary>
        /// Decrypt message
        /// </summary>
        /// <param name="message">Message which must be encrypted</param>
        /// <param name="fOAEP">Support OAEP or not</param>
        /// <returns>Decrypted message</returns>
        public string DecryptMessage(byte[] message, bool fOAEP)
        {
            return Encoding.Unicode.GetString(_rsa.Decrypt(message, fOAEP));
        }

        /// <summary>
        /// Get your digital signature
        /// </summary>
        /// <param name="hashSumOfMessage">Hash sum of sending message</param>
        /// <returns>Digital signature</returns>
        public byte[] GetDigitalSignature(byte[] hashSumOfMessage)
        {
            RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(_rsa);
            formatter.SetHashAlgorithm("SHA1");
            return formatter.CreateSignature(hashSumOfMessage);
        }

        /// <summary>
        /// Verify digital signature
        /// </summary>
        /// <param name="hashSumOfMessage">Hash sum of checking message</param>
        /// <param name="digitalSignature">Digital signature</param>
        /// <param name="publicKey">Public key from other side</param>
        /// <returns>Is valid</returns>
        public bool IsValidDigitalSignature(byte[] hashSumOfMessage, byte[] digitalSignature, string publicKey)
        {
            using(RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
            {
                provider.FromXmlString(publicKey);
                RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(provider);
                deformatter.SetHashAlgorithm("SHA1");
                return deformatter.VerifySignature(hashSumOfMessage, digitalSignature);
            }
        }
    }
}
