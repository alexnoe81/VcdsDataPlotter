﻿using VcdsDataPlotter.Lib.Implementation;
using VcdsDataPlotter.Lib.RawTableReader;

namespace VcdsDataPlotter.Lib.Tests;

/// <summary>
/// TODO: Add test cases that do not only count columns and rows, but that test that the content is read correctly
/// </summary>
[TestClass]
public sealed class Test_CsvTable
{
    [TestMethod]
    public void Test_ReadEmptyFile()
    {
        string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "Empty.csv");

        using var fileStream = File.OpenRead(sampleFilePath);
        CsvTable reader = CsvTable.Open(new StreamReader(fileStream), ",");

        Assert.AreEqual(0, reader.NumberOfRows);
        Assert.AreEqual(0, reader.NumberOfColumns);
    }

    [TestMethod]
    public void Test_ReadOneSimpleLine()
    {
        string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "OneSimpleLine.csv");

        using var fileStream = File.OpenRead(sampleFilePath);
        CsvTable reader = CsvTable.Open(new StreamReader(fileStream), ",");

        Assert.AreEqual(1, reader.NumberOfRows);
        Assert.AreEqual(5, reader.NumberOfColumns);
    }

    [TestMethod]
    public void Test_ReadSimpleFile()
    {
        string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "Sample_1.csv");

        using var fileStream = File.OpenRead(sampleFilePath);
        CsvTable reader = CsvTable.Open(new StreamReader(fileStream), ",");

        Assert.AreEqual(15, reader.NumberOfRows);
        Assert.AreEqual(26, reader.NumberOfColumns);
    }
}
