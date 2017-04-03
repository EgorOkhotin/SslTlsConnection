using System.Collections.Generic;
using System.Linq;
using DHProtocolLibriary;

namespace TlsLibriary
{
    public class TlsClientManager : TlsManager
    {
        private DHClient _client;

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

       

        private void SetConnectMessage(string key,string divider,string generator)
        {
            _client = new DHClient(key, divider, generator);
            KeyOfEncrypt.EncryptKey = _client.CommonKey;
            KeyOfEncrypt.MakeKeyReadOnly();
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
