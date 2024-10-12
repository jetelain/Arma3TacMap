using Arma3TacMapLibrary.Arma3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cTabWebAppTests
{
    [TestClass]
    public class Arma3TacMapLibraryTest
    {
        [TestMethod]
        public void ArmaSerializer_ParseMixedArray()
        {
            var value = ArmaSerializer.ParseMixedArray("[\"Hello \"\"world\"\" !\",123.456,null,789.123,\"Hello \"\"world\"\" !\", true, false]");
            Assert.AreEqual(7, value.Length);
            Assert.AreEqual("Hello \"world\" !", value[0]);
            Assert.AreEqual(123.456d, value[1]);
            Assert.AreEqual(null, value[2]);
            Assert.AreEqual(789.123d, value[3]);
            Assert.AreEqual("Hello \"world\" !", value[4]);
            Assert.AreEqual(true, value[5]);
            Assert.AreEqual(false, value[6]);
        }

        [TestMethod]
        public void ArmaSerializer_ParseDouble()
        {
            Assert.AreEqual(1.04858e+12, ArmaSerializer.ParseDouble("1.04858e+12"));
            Assert.AreEqual(1.04858e-12, ArmaSerializer.ParseDouble("1.04858e-12"));
        }
    }
}
