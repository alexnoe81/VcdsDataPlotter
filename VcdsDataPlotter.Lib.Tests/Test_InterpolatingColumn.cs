using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.Implementation;
using VcdsDataPlotter.Lib.Implementation.CalculatedColumns;
using VcdsDataPlotter.Lib.Interface;

namespace VcdsDataPlotter.Lib.Tests
{
    [TestClass]
    public class Test_InterpolatingColumn
    {
        [TestMethod]
        public void Test_InterpolatingColumn_OneValue()
        {
            // Test with one value
            DummyRawColumn rawColumn = new DummyRawColumn();
            rawColumn.Data.Add(new RawDataItem(TimeSpan.FromSeconds(0.2), "17.3"));

            SimpleDiscreteNumericColumn simpleDiscreteColumn = new SimpleDiscreteNumericColumn(rawColumn);

            IInterpolatingColumn column = InterpolatingColumn.Create(simpleDiscreteColumn);

            Assert.AreEqual(17.3, (double)column.GetValue(TimeSpan.FromSeconds(0)), 0.0001);
            Assert.AreEqual(17.3, (double)column.GetValue(TimeSpan.FromSeconds(0.2)), 0.0001);
            Assert.AreEqual(17.3, (double)column.GetValue(TimeSpan.FromSeconds(0.5)), 0.0001);
        }

        [TestMethod]
        public void Test_InterpolatingColumn_TwoValues()
        {
            // Test with one value
            DummyRawColumn rawColumn = new DummyRawColumn();
            rawColumn.Data.Add(new RawDataItem(TimeSpan.FromSeconds(1.0), "20"));
            rawColumn.Data.Add(new RawDataItem(TimeSpan.FromSeconds(3.0), "30"));

            SimpleDiscreteNumericColumn simpleDiscreteColumn = new SimpleDiscreteNumericColumn(rawColumn);

            IInterpolatingColumn column = InterpolatingColumn.Create(simpleDiscreteColumn);

            Assert.AreEqual(20, (double)column.GetValue(TimeSpan.FromSeconds(0.5)), 0.0001);
            Assert.AreEqual(20, (double)column.GetValue(TimeSpan.FromSeconds(1.0)), 0.0001);
            Assert.AreEqual(22.5, (double)column.GetValue(TimeSpan.FromSeconds(1.5)), 0.0001);
            Assert.AreEqual(30, (double)column.GetValue(TimeSpan.FromSeconds(3.5)), 0.0001);
        }


        private class DummyRawColumn : IRawDataColumn
        {
            public string ChannelId => "1234";
            public string Title => "Some title";
            public string? RawUnit => "Some unit";

            public IEnumerable<RawDataItem> EnumerateDataItems()
            {
                foreach (var item in Data)
                    yield return item;
            }

            public List<RawDataItem> Data = new List<RawDataItem>();
        }
    }
}
