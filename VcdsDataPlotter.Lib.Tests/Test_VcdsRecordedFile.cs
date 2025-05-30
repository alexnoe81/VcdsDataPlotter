﻿using VcdsDataPlotter.Lib.RawTable.Columnizer;
using VcdsDataPlotter.Lib.RawTableReader;

namespace VcdsDataPlotter.Lib.Tests;

[TestClass]
public sealed class Test_VcdsRecordedFile
{
    [TestMethod]
    public void Test_ReadEmptyFile()
    {
        string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "VcsdLogWithHeader.csv");

        using var fileStream = File.OpenRead(sampleFilePath);
        using var reader = new StreamReader(fileStream);
        CsvTable table = CsvTable.Open(reader, ",");

        var channelMap = ChannelMaps.CreateDefaultMap();
        VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);
        Assert.AreEqual(12, vcdsRecordedFile.RawDataColumns.Length);
        Assert.AreEqual(13, vcdsRecordedFile.DiscreteDataColumns.Length);

        Assert.AreEqual(new DateTime(2024, 11, 4, 16, 22, 12), vcdsRecordedFile.RecordingTimestamp);
    } 
}
