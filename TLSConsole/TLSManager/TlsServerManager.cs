using DHProtocolLibriary;

namespace TlsLibriary
{
    public class TlsServerManager : TlsManager
    {
        private DHServer _server = new DHServer();
       // private readonly Key _key = new Key();

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
            KeyOfEncrypt.EncryptKey = _server.CommonKey;
            KeyOfEncrypt.MakeKeyReadOnly();
            _server.ClearGlobalData();
        }
    }
}
