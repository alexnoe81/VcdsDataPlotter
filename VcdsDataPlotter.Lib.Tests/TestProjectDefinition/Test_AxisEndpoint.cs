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
    public class Test_AxisEndpoint
    {
        [TestMethod]
        public void Test_Compare_Auto()
        {
            Assert.IsTrue(AxisEndpoint.Auto.Equals(AxisEndpoint.Auto));
            Assert.IsTrue(AxisEndpoint.Auto.Equals(new AxisEndpoint() { IsAuto = true }));
            Assert.IsFalse(AxisEndpoint.Auto.Equals(new AxisEndpoint() { IsAuto = false }));

            Assert.IsTrue(new AxisEndpoint() { Value = 5 }.Equals(new AxisEndpoint() { Value = 5 }));
            Assert.IsFalse(new AxisEndpoint() { Value = 5 }.Equals(new AxisEndpoint() { Value = 4 }));
            Assert.IsFalse(new AxisEndpoint() { Value = 5 }.Equals(AxisEndpoint.Auto));
        }

        [TestMethod]
        public void Test_XmlSerialization_Auto()
        {
            var testObj = AxisEndpoint.Auto;

            XmlSerializer serializer = new XmlSerializer(typeof(AxisEndpoint));
            StringBuilder stringBuilder = new StringBuilder();
            using StringWriter stringWriter = new StringWriter(stringBuilder);
            serializer.Serialize(stringWriter, testObj);
            var serialized = stringBuilder.ToString();

            StringReader stringReader = new StringReader(serialized);
            var deserialized = (AxisEndpoint)serializer.Deserialize(stringReader);

            Assert.AreEqual(deserialized, testObj);
        }

        [TestMethod]
        public void Test_XmlSerialization_ManualValue()
        {
            var testObj = new AxisEndpoint(42);

            XmlSerializer serializer = new XmlSerializer(typeof(AxisEndpoint));
            StringBuilder stringBuilder = new StringBuilder();
            using StringWriter stringWriter = new StringWriter(stringBuilder);
            serializer.Serialize(stringWriter, testObj);
            var serialized = stringBuilder.ToString();

            StringReader stringReader = new StringReader(serialized);
            var deserialized = (AxisEndpoint)serializer.Deserialize(stringReader);

            Assert.AreEqual(deserialized, testObj);
        }
    }
}
