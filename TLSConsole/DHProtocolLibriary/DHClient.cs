using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace DHProtocolLibriary
{
    public sealed class DHClient
    {
        private static byte _generator;
        private static BigInteger _publicKeyOtherSide;
        private static BigInteger _publicKey;
        private static BigInteger _privateKey;
        private static BigInteger _divider;
        private static BigInteger _commonKey;

        public void FirstPhase(string publicKeyOtherSide, string divider, string generator)
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

        public void ClearGlobalData()
        {
            _generator = 0;
            _publicKeyOtherSide = 0;
            _publicKey = 0;
            _privateKey = 0;
            _divider = 0;
            _commonKey = 0;
        }

        public string PublicKey => _publicKey.ToString();

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
