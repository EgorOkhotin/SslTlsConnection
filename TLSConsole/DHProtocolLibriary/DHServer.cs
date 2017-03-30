using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using NumbersLibriary;

namespace DHProtocolLibriary
{
    public sealed class DHServer
    {
        private static byte _generator;
        private static BigInteger _publicKey;
        private static BigInteger _privateKey;
        private static BigInteger _divider;
        private static BigInteger _commonKey;

        public void FirstPhase()
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

        public void SecondPhase(string number)
        {
            _commonKey = Calculate_commonKey(_divider, _privateKey, BigInteger.Parse(number));
        }

        public void ClearGlobalData()
        {
            _generator = 0;
            _publicKey = 0;
            _privateKey = 0;
            _divider = 0;
            _commonKey = 0;
        }

        public string PublicKey => _publicKey.ToString();
        public string Generator => _generator.ToString();
        public string Divider => _divider.ToString();

        public string CommonKey => _commonKey.ToString();

        private byte Create_generator(byte lowBorder, byte highBorder, Random rm)
        {
            bool test1, IsNotSimple=true;
            byte number =0;
            while (IsNotSimple)
            {
                number = (byte) rm.Next(lowBorder, highBorder);
                test1 = NumberWorker.SurfaceTest(number,number-1);
                if (test1 == true) IsNotSimple = false;
            }
            return number;
        }

        private BigInteger Create_divider(Random rm, int lowBorder, int highBorder)
        {
            return CreateSimpleBigInteger(rm,lowBorder,highBorder,300);
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
            bool IsNotSimple = true;
            bool test1;
            BigInteger number = 0;
            int highBorderNumbeerOfTest = 10000;
            while (IsNotSimple)
            {
                number = CreateBigInteger(rm, lowBorder, highBorder, rangeOfNumber);
                test1 = NumberWorker.SurfaceTest(number,highBorderNumbeerOfTest);
                if (test1 == true) IsNotSimple = false;
            }
            return number;
        }

    }
}
