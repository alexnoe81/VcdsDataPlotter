using VcdsDataPlotter.Lib.Implementation;
using VcdsDataPlotter.Lib.RawTable.Columnizer;
using VcdsDataPlotter.Lib.RawTableReader;

namespace VcdsDataPlotter.Lib.Tests;

[TestClass]
public sealed class Test_VcdsRecordedTableColumn
{
    [TestMethod]
    public void Test_GetHeaders()
    {
        string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "VcsdLogWithHeader.csv");

        using var fileStream = File.OpenRead(sampleFilePath);
        CsvTable table = CsvTable.Open(new StreamReader(fileStream), ",");

        VcdsRawDataColumn column = new VcdsRawDataColumn(table, 5, 6);
        column.Initialize();

        Assert.AreEqual("IDE00100", column.ChannelId);
        Assert.AreEqual("Nm", column.RawUnit);
        Assert.AreEqual("Engine torque", column.Title);
    }

    [TestMethod]
    public void Test_GetData()
    {
        string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "VcsdLogWithHeader.csv");

        using var fileStream = File.OpenRead(sampleFilePath);
        CsvTable table = CsvTable.Open(new StreamReader(fileStream), ",");

        VcdsRawDataColumn column = new VcdsRawDataColumn(table, 5, 6);
        column.Initialize();

        var data = column.EnumerateDataItems().ToArray();
        
        Assert.AreEqual(0.08, data[0].TimeStamp.TotalSeconds, 0.0001);
        Assert.AreEqual(0.51, data[1].TimeStamp.TotalSeconds, 0.0001);
        Assert.AreEqual(0.93, data[2].TimeStamp.TotalSeconds, 0.0001);

        Assert.AreEqual("-500.0", data[0].RawData);
        Assert.AreEqual("-500.0", data[1].RawData);
        Assert.AreEqual("-500.0", data[2].RawData);
    }
}
