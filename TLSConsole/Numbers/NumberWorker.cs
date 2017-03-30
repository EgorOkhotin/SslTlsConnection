using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace NumbersLibriary
{
    public class NumberWorker
    {
        public static bool SurfaceTest(BigInteger number, int highBorderNumberOfTest)
        {
            if (IsEven(number) == true) return false;
            bool result = false;
            for (int i = 2; (i <= highBorderNumberOfTest)&(i<number); i++)
            {
                if (BigInteger.Remainder(number, i) == 0) return result;
            }
            result = true;
            return result;
        }

        public static bool RabinMillerTest(BigInteger number, int countOfRounds)
        {
            if (IsEven(number) == true) return false;
            bool result = false;
            int s = 0; 
            int t = 0;
            BigInteger testingNumber = BigInteger.Add(number,-1);

            //testingNumber=(2^s)*t
            SetValues_s_t(testingNumber, ref t, ref s);
            // a!=0 ; a!=1(mod number) ; 1<a<m
            int k = 0;
            for (int i = 1; i <= countOfRounds; i++)
            {
                int a = Generate_a((int)number);
                if ((BigInteger.Remainder(number, a) == 0) & (BigInteger.Pow(a, t) != number)) k++;
            }
            if (k == 0) result = true;

            return result;
        }

        private static void SetValues_s_t(BigInteger testingNumber, ref int t, ref int s)
        {
            bool resultIsSet = false;
            int i = 3;
            while (resultIsSet == false & i < (int.MaxValue / 2) & i < testingNumber)
            {
                if (BigInteger.Remainder(testingNumber, i) == 0)
                {
                    var doubleS = BigInteger.Log(BigInteger.Divide(testingNumber, i), 2);
                    if (doubleS % 1 == 0)
                    {
                        s = (int)doubleS;
                        t = i;
                        resultIsSet = true;
                    }
                }

                i += 2;
            }
            if (resultIsSet == false) throw new Exception();
        }

        private static int Generate_a(int number)
        {
            Random rm = new Random();
            return rm.Next(0, number-1);

        }

        private static bool IsEven(BigInteger number)
        {
            bool result;

            if (BigInteger.Remainder(number, 2) == 0) result = true;
            else result = false;

            return result;
        }
    }
}
