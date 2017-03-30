using System;
using NUnit;
using NUnit.Framework;
using NumbersLibriary;
using System.Numerics;


namespace NumbersLibriary.Tests
{
    [TestFixture]
    public class NumbersLibriaryTest
    {
        [Test]
        [TestCase(997)]
        [TestCase(353)]
        [TestCase(457)]
        [TestCase(4651)]
        [TestCase(5003)]
        public void SurfaceTest_SimpleNumbers_ReturnTrue(int number)
        {
            Assert.IsTrue(NumberWorker.SurfaceTest(number, 6000));
        }

        [Test]
        [TestCase(998)]
        [TestCase(350)]
        [TestCase(460)]
        [TestCase(4653)]
        [TestCase(5007)]
        public void SurfaceTest_UnSimpleNumbers_ReturnFalse(int number)
        {
            Assert.IsFalse(NumberWorker.SurfaceTest(number, 6000));
        }

        [Test]
        [TestCase(5077)]
        [TestCase(3923)]
        [TestCase(4787)]
        [TestCase(2503)]
        [TestCase(5021)]
        public void RabinMillerTest_SimpleNumbers_ReturnTrue(int number)
        {
            Assert.IsTrue(NumberWorker.RabinMillerTest(number,4));
        }


    }
}
