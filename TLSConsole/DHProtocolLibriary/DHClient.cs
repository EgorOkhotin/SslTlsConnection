using System;
using System.Numerics;

namespace DHProtocolLibriary
{
    public sealed class DHClient
    {
        private byte _generator;
        private BigInteger _publicKeyOtherSide;
        private BigInteger _publicKey;
        private BigInteger _privateKey;
        private BigInteger _divider;
        private BigInteger _commonKey;

        /// <summary>
        /// New object
        /// </summary>
        /// <param name="publicKeyOtherSide">Public key from server</param>
        /// <param name="divider">Divider of function</param>
        /// <param name="generator">Generator of function</param>
        public DHClient(string publicKeyOtherSide, string divider, string generator)
        {
            _generator = Convert.ToByte(generator);
            _publicKeyOtherSide = BigInteger.Parse(publicKeyOtherSide);
            _divider = BigInteger.Parse(divider);

            Random rm = new Random();
            int lowBorder = 2;
            int highBorder = 2147483647;
            _privateKey = Create_privateKey(rm, lowBorder, highBorder);
            _publicKey = Calculate_publicKey(_divider, _privateKey, _generator);

            _commonKey = Calculate_commonKey(_divider, _privateKey, _publicKeyOtherSide);
        }

        /// <summary>
        /// Clear al data in object
        /// </summary>
        public void ClearGlobalData()
        {
            _generator = 0;
            _publicKeyOtherSide = 0;
            _publicKey = 0;
            _privateKey = 0;
            _divider = 0;
            _commonKey = 0;
        }

        /// <summary>
        /// Public key of client
        /// </summary>
        public string PublicKey => _publicKey.ToString();

        /// <summary>
        /// Common key for encrypt and decrypt
        /// </summary>
        public string CommonKey => _commonKey.ToString();

        private BigInteger Create_privateKey(Random rm, int lowBorder, int highBorder)
        {
            int rangeOfNumber = 100;
            return CreateBigInteger(rm, lowBorder, highBorder, rangeOfNumber);
        }

        private BigInteger CreateBigInteger(Random rm, int lowBorder, int highBorder, int rangeOfNumber)
        {
            string stringViewOfBigIntegerNumber = "";
            while (stringViewOfBigIntegerNumber.Length < rangeOfNumber)
            {
                stringViewOfBigIntegerNumber += rm.Next(lowBorder, highBorder).ToString();
            }
            return BigInteger.Parse(stringViewOfBigIntegerNumber);
        }

        private BigInteger Calculate_publicKey(BigInteger divider, BigInteger privateKey, byte generator)
        {
            return BigInteger.ModPow(generator, privateKey, divider);
        }

        private BigInteger Calculate_commonKey(BigInteger divider, BigInteger privateKey, BigInteger publicKeyOtherSide)
        {
            return BigInteger.ModPow(publicKeyOtherSide, privateKey, divider);
        }
    }
}
