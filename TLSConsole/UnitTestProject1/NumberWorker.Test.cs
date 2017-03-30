using NUnit.Framework;

namespace TLS.UnitTests
{
    [TestFixture]
    public class TestNumberWorker
    {
        [Test]
        [TestCase(487)]
        [TestCase(383)]
        [TestCase(997)]
        public void SurfaceTest_SimpleNumbers_ReturnTrue(int number)
        {
            bool result = NumberWorker.SurfaceTest(number);
            Assert.IsTrue(result);
        }
    }
}
