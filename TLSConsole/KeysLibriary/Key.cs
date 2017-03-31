using System.Security;

namespace KeysLibriary
{
    internal class Key
    {
        private static SecureString _encryptKey;

        /// <summary>
        ///Key for encrypt and decrypt
        /// </summary>
        public string EncryptKey
        {
            get { return _encryptKey.ToString(); }
            set
            {
                _encryptKey = new SecureString();
                foreach (char ch in value)
                {
                    _encryptKey.AppendChar(ch);
                }
            }
        }

        /// <summary>
        /// Make key only for read
        /// </summary>
        public void MakeKeyReadOnly()
        {
            _encryptKey.MakeReadOnly();
        }

    }
}
