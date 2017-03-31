using System;
using System.Numerics;
using NumbersLibriary;

namespace DHProtocolLibriary
{
    public sealed class DHServer
    {
        private byte _generator;
        private BigInteger _publicKey;
        private BigInteger _privateKey;
        private BigInteger _divider;
        private BigInteger _commonKey;

        /// <summary>
        /// New object
        /// </summary>
        public DHServer()
        {
            int lowBorder = 2, highBorder = 9;
            Random rm = new Random();
            _generator = Create_generator((byte)lowBorder, (byte)highBorder, rm);

            lowBorder = 2;
            highBorder = 2147483647;
            _privateKey = Create_privateKey(rm, lowBorder, highBorder);
            _divider = Create_divider(rm, lowBorder, highBorder);

            _publicKey = Calculate_publicKey(_divider, _privateKey, _generator);
        }

        /// <summary>
        /// Calculate common key
        /// </summary>
        /// <param name="number">Public key from client</param>
        public void CalculateKey(string number)
        {
            _commonKey = Calculate_commonKey(_divider, _privateKey, BigInteger.Parse(number));
        }

        /// <summary>
        /// Clear all data in object
        /// </summary>
        public void ClearGlobalData()
        {
            _generator = 0;
            _publicKey = 0;
            _privateKey = 0;
            _divider = 0;
            _commonKey = 0;
        }

        /// <summary>
        /// Public key of server
        /// </summary>
        public string PublicKey => _publicKey.ToString();

        /// <summary>
        /// Generator of function
        /// </summary>
        public string Generator => _generator.ToString();

        /// <summary>
        /// Divider of function
        /// </summary>
        public string Divider => _divider.ToString();

        /// <summary>
        /// Common key for encrypt and decrypt
        /// </summary>
        public string CommonKey => _commonKey.ToString();

        private byte Create_generator(byte lowBorder, byte highBorder, Random rm)
        {
            bool isNotSimple = true;
            byte number = 0;
            while (isNotSimple)
            {
                number = (byte)rm.Next(lowBorder, highBorder);
                var test1 = NumberWorker.SurfaceTest(number, number - 1);
                if (test1 == true) isNotSimple = false;
            }
            return number;
        }

        private BigInteger Create_divider(Random rm, int lowBorder, int highBorder)
        {
            return CreateSimpleBigInteger(rm, lowBorder, highBorder, 300);
        }

        private BigInteger Create_privateKey(Random rm, int lowBorder, int highBorder)
        {
            return CreateBigInteger(rm, lowBorder, highBorder, 100);
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

        private BigInteger CreateSimpleBigInteger(Random rm, int lowBorder, int highBorder, int rangeOfNumber)
        {
            bool isNotSimple = true;
            BigInteger number = 0;
            int highBorderNumbeerOfTest = 10000;
            while (isNotSimple)
            {
                number = CreateBigInteger(rm, lowBorder, highBorder, rangeOfNumber);
                var test1 = NumberWorker.SurfaceTest(number, highBorderNumbeerOfTest);
                if (test1 == true) isNotSimple = false;
            }
            return number;
        }

    }
}
