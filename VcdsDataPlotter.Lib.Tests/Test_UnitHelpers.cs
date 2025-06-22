using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns;
using VcdsDataPlotter.Lib.Physics;

namespace VcdsDataPlotter.Lib.Tests
{
    [TestClass]
    public class Test_UnitHelpers
    {
        [TestMethod]
        public void Test_IsConvertible()
        {
            Assert.IsTrue(UnitHelpers.IsConvertible("g", "g"));
            Assert.IsTrue(UnitHelpers.IsConvertible("mg", "g"));
            Assert.IsTrue(UnitHelpers.IsConvertible("g", "mg"));
            Assert.IsTrue(UnitHelpers.IsConvertible("mg", "kg"));
            Assert.IsTrue(UnitHelpers.IsConvertible("kg", "mg"));

            Assert.IsTrue(UnitHelpers.IsConvertible("s", "min"));
            Assert.IsTrue(UnitHelpers.IsConvertible("s", "h"));
            Assert.IsTrue(UnitHelpers.IsConvertible("min", "s"));
            Assert.IsTrue(UnitHelpers.IsConvertible("h", "s"));

            Assert.IsTrue(UnitHelpers.IsConvertible("km/h", "m/s"));

            Assert.IsTrue(UnitHelpers.IsConvertible("mg/s", "kg/h"));
            Assert.IsTrue(UnitHelpers.IsConvertible("kg/h", "mg/s"));

            Assert.IsFalse(UnitHelpers.IsConvertible("min", "kg"));
            Assert.IsFalse(UnitHelpers.IsConvertible("kg", "min"));
        }

        [TestMethod]
        public void Test_Simple()
        {
            Assert.AreEqual(1.0, UnitHelpers.GetConversionFactor("g", "g"));
            Assert.AreEqual(1.0, UnitHelpers.GetConversionFactor("s", "s"));

            Assert.AreEqual(1000.0, UnitHelpers.GetConversionFactor("kg", "g"));
            Assert.AreEqual(1.0/1000.0, UnitHelpers.GetConversionFactor("g", "kg"));

            Assert.AreEqual(1000000.0, UnitHelpers.GetConversionFactor("kg", "mg"));
            Assert.AreEqual(1.0/1000000.0, UnitHelpers.GetConversionFactor("mg", "kg"));

            Assert.AreEqual(3_600.0 / 1_000_000.0, UnitHelpers.GetConversionFactor("mg/s", "kg/h"));
            Assert.AreEqual(1_000_000.0 / 3_600.0, UnitHelpers.GetConversionFactor("kg/h", "mg/s"));

            Assert.AreEqual(1 / 3.6, UnitHelpers.GetConversionFactor("km/h", "m/s"));
        }

        [TestMethod]
        public void Test_BadConversions()
        {
            try
            {
                Assert.AreEqual(1.0, UnitHelpers.GetConversionFactor("kg", "min"));
                Assert.Fail("Expected exception not thrown.");
            }
            catch (ArgumentException)
            {

            }
        }
    }
}
