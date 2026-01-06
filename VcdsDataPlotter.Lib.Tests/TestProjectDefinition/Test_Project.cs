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
    public class Test_Project
    {
        [TestMethod]
        public void Test_Serialization()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Project));
            StringBuilder stringBuilder = new StringBuilder();
            using StringWriter stringWriter = new StringWriter(stringBuilder);

            Project testObj = new Project();
            Document testDoc = new Document();
            testObj.Documents = [testDoc];

            testDoc.DataFilePath = new RelativePath(KnownBasePaths.ProjectFile, "abc.csv");
            testDoc.X1 = new AxisSpec()
            {
                Title = "Time [s]",
                MinValue = new AxisEndpoint(0),
                MaxValue = new AxisEndpoint(50),
                TickIntervals = [1, 5]

            };
            testDoc.Y1 = new AxisSpec()
            {
                Title = "Measured stuff [kg/h]",
                MinValue = new AxisEndpoint(-20),
                MaxValue = new AxisEndpoint(100),
                TickIntervals = [5, 20]
            };

            testDoc.CustomColumns = [ new CalculatedColumns.ConfigFiles.ColumnBuilderConfigurationDefinition()
            {
                Title = "abc",
                ChannelId = "VIRT_ABC",
                Steps =
                [
                    new CalculatedColumns.ConfigFiles.Select() { ChannelId = "IDE00075" }
                ]
            }];
            testDoc.SelectedColumns = ["VIRT_ABC", "  IDE00075   "];

            serializer.Serialize(stringWriter, testObj);
            var serialized = stringBuilder.ToString();

            StringReader stringReader = new StringReader(serialized);
            var deserializedProject = (Project?)serializer.Deserialize(stringReader);
            var deserializedDocument = deserializedProject.Documents[0];

            // Compare deserialized with testObj
            Assert.IsNotNull(deserializedDocument);
            Assert.IsNotNull(deserializedDocument.X1);
            Assert.IsNotNull(deserializedDocument.Y1);
            Assert.IsNull(deserializedDocument.X2);
            Assert.IsNull(deserializedDocument.Y2);

            Assert.AreEqual(testDoc.X1.Title, deserializedDocument.X1.Title);
            Assert.AreEqual(testDoc.X1.MinValue.Value, deserializedDocument.X1.MinValue.Value);
            Assert.AreEqual(testDoc.X1.MaxValue.Value, deserializedDocument.X1.MaxValue.Value);
            Assert.AreEqual(testDoc.Y1.Title, deserializedDocument.Y1.Title);
            Assert.AreEqual(testDoc.Y1.MinValue.Value, deserializedDocument.Y1.MinValue.Value);
            Assert.AreEqual(testDoc.Y1.MaxValue.Value, deserializedDocument.Y1.MaxValue.Value);
        }
    }
}
