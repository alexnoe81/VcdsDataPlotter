using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VcdsDataPlotter.Lib.ProjectDefinition;

namespace VcdsDataPlotter.Lib.Tests.TestProjectDefinition
{
    [TestClass]
    public class Test_AxisSpec
    {
        [TestMethod]
        public void Test_Serialization()
        {
            var testObj = new AxisSpec();
            testObj.Title = "Test title";
            testObj.TickIntervals = [5, 20];
            testObj.MinValue = new AxisEndpoint(10);
            testObj.MaxValue = AxisEndpoint.Auto;

            XmlSerializer serializer = new XmlSerializer(typeof(AxisSpec));
            StringBuilder stringBuilder = new StringBuilder();
            using StringWriter stringWriter = new StringWriter(stringBuilder);
            serializer.Serialize(stringWriter, testObj);
            var serialized = stringBuilder.ToString();

            StringReader stringReader = new StringReader(serialized);
            var deserialized = (AxisSpec?)serializer.Deserialize(stringReader);

            Assert.IsNotNull(deserialized);
            Assert.IsNotNull(deserialized.MinValue);
            Assert.IsNotNull(deserialized.MaxValue);
            Assert.IsTrue(deserialized.MaxValue.IsAuto);
            Assert.IsFalse(deserialized.MinValue.IsAuto);
            Assert.AreEqual(10, deserialized.MinValue.Value);
        }
    }
}
