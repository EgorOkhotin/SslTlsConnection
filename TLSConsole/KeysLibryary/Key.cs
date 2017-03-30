using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace KeysLibryary
{
    internal class Key
    {
        private static SecureString _encryptKey;

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

        public void MakeKeyReadOnly()
        {
            _encryptKey.MakeReadOnly();
        }

    }
}
