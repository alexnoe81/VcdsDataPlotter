using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnSpecs;
using VcdsDataPlotter.Lib.RawTable.Columnizer;
using VcdsDataPlotter.Lib.RawTableReader;

namespace VcdsDataPlotter.Lib.Tests
{
    [TestClass]
    public class Test_ColumnBuilderConfiguration
    {
        [TestMethod]
        public void Test_Select_FindFirst()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "LogWithSpeed-2022-11-23.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            using var reader = new StreamReader(fileStream);
            CsvTable table = CsvTable.Open(reader, ",");

            var channelMap = ChannelMaps.CreateDefaultMap();
            VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);

            // Looking for IDE00075 will succeed, so the typo in Vehicl speed doesn't matter
            var vehicleSpeedColumnBuilder = new ColumnBuilderConfiguration().SelectFirst(
                ColumnSpec.ChannelIdIs("IDE00075"),
                ColumnSpec.TitleContains("Vehicl speed"));

            var detailedResult = vehicleSpeedColumnBuilder.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var speedColumn);
            Assert.IsTrue(detailedResult.Success);
            Assert.AreEqual(speedColumn!.ChannelId, "IDE00075");
        }

        public void Test_SelectFirst_FindSecond()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "LogWithSpeed-2022-11-23.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            using var reader = new StreamReader(fileStream);
            CsvTable table = CsvTable.Open(reader, ",");

            var channelMap = ChannelMaps.CreateDefaultMap();
            VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);

            // Looking for IDE00076 will fail, but Vehicle speed will succeed 
            var vehicleSpeedColumnBuilder = new ColumnBuilderConfiguration().SelectFirst(
                ColumnSpec.ChannelIdIs("IDE00076"),
                ColumnSpec.TitleContains("Vehicle speed"));

            var detailedResult = vehicleSpeedColumnBuilder.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var speedColumn);
            Assert.IsTrue(detailedResult.Success);
            Assert.AreEqual(speedColumn!.ChannelId, "IDE00075");
        }

        [TestMethod]
        public void Test_SelectFirst2_FindSecond()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "LogWithSpeed-2022-11-23.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            using var reader = new StreamReader(fileStream);
            CsvTable table = CsvTable.Open(reader, ",");

            var channelMap = ChannelMaps.CreateDefaultMap();
            VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);

            // Test the overload of SelectFirst which takes a sequence of ColumnBuilderConfiguration and
            // uses the first one that can be materialized.
            var vehicleSpeedColumnBuilder = new ColumnBuilderConfiguration().SelectFirst(
                new ColumnBuilderConfiguration().Select(ColumnSpec.ChannelIdIs("IDE00076")),
                new ColumnBuilderConfiguration().Select(ColumnSpec.TitleContains("Vehicle speed")));

            var detailedResult = vehicleSpeedColumnBuilder.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var speedColumn);
            Assert.IsTrue(detailedResult.Success);
            Assert.AreEqual(speedColumn!.ChannelId, "IDE00075");
        }

        [TestMethod]
        public void Test_Select_DonotFind()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "LogWithSpeed-2022-11-23.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            using var reader = new StreamReader(fileStream);
            CsvTable table = CsvTable.Open(reader, ",");

            var channelMap = ChannelMaps.CreateDefaultMap();
            VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);

            var vehicleSpeedColumnBuilder = new ColumnBuilderConfiguration().SelectFirst(
                ColumnSpec.ChannelIdIs("IDE00076"),
                ColumnSpec.TitleContains("Vehicl speed"));

            var detailedResult = vehicleSpeedColumnBuilder.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var speedColumn);
            Assert.IsFalse(detailedResult.Success);
        }

        [TestMethod]
        public void Test_DifferenceToFirstRow()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "LogWithDefConsumption-2025-11-20.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            using var reader = new StreamReader(fileStream);
            CsvTable table = CsvTable.Open(reader, ",");

            var channelMap = ChannelMaps.CreateDefaultMap();
            VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);

            var defColumnBuilder = ColumnBuilderConfiguration.Create("Total DEF consumed", "VIRT_DEF_CONSUMED")
                .Select(ColumnSpec.ChannelIdIs("IDE16115"))
                .Calculate.DifferenceToFirstRow;

            var detailedResult = defColumnBuilder.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var defColumn);
            Assert.IsTrue(detailedResult.Success);

            var dataPoints = defColumn!.EnumerateDataItems().ToList();
            Assert.AreEqual(0, dataPoints[0].RawData);
            Assert.AreEqual(0, dataPoints[1].RawData);
            Assert.AreEqual(24.576, dataPoints.Last().RawData, 1e-6);
        }

        [TestMethod]
        public void Test_BuildIntegral_With_Distance_OK()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "LogWithSpeed-2022-11-23.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            using var reader = new StreamReader(fileStream);
            CsvTable table = CsvTable.Open(reader, ",");

            var channelMap = ChannelMaps.CreateDefaultMap();
            VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);

            var distanceBuilder = ColumnBuilderConfiguration.Create("Distance", "VIRT_DISTANCE")
                .Calculate.IntegralByTime.Over(new ColumnBuilderConfiguration().SelectFirst(
                    ColumnSpec.ChannelIdIs("IDE00075"),
                    ColumnSpec.TitleContains("Vehicl speed")));

            var detailedResult = distanceBuilder.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var distanceColumn);
            Assert.IsTrue(detailedResult.Success);
            Assert.AreEqual("km", distanceColumn!.Unit);
            
            var dataItems = distanceColumn.EnumerateDataItems().ToList();
            
            // We know that the total distance in this file was around 44 km
            Assert.AreEqual(44, dataItems.Last().RawData, 1);
        }

        [TestMethod]
        public void Test_BuildIntegral_With_Distance_WithUnitReassignment_OK()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "LogWithSpeed-2022-11-23.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            using var reader = new StreamReader(fileStream);
            CsvTable table = CsvTable.Open(reader, ",");

            var channelMap = ChannelMaps.CreateDefaultMap();
            VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);

            var distanceBuilder = ColumnBuilderConfiguration.Create("Distance", "VIRT_DISTANCE")
                .Calculate.IntegralByTime.Over(new ColumnBuilderConfiguration().SelectFirst(
                    ColumnSpec.ChannelIdIs("IDE00075"),
                    ColumnSpec.TitleContains("Vehicle speed")))
                .AssignUnit("abc");

            // TODO: We need to test that no calculation is going on
            var detailedResult = distanceBuilder.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var distanceColumn);
            Assert.IsTrue(detailedResult.Success);
            Assert.AreEqual("abc", distanceColumn!.Unit);
            Assert.AreEqual("Distance", distanceColumn!.Title);
            Assert.AreEqual("VIRT_DISTANCE", distanceColumn!.ChannelId);
        }

        [TestMethod]
        public void Test_BuildIntegral_With_Distance_WithUnitConversion_OK_1()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "LogWithSpeed-2022-11-23.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            using var reader = new StreamReader(fileStream);
            CsvTable table = CsvTable.Open(reader, ",");

            var channelMap = ChannelMaps.CreateDefaultMap();
            VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);

            var distanceBuilder = ColumnBuilderConfiguration.Create("Distance", "VIRT_DISTANCE")
                .Calculate.IntegralByTime.Over(new ColumnBuilderConfiguration().SelectFirst(
                    ColumnSpec.ChannelIdIs("IDE00075"),
                    ColumnSpec.TitleContains("Vehicle speed")))
                .ConvertUnit("m");

            var detailedResult = distanceBuilder.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var distanceColumn);
            Assert.IsTrue(detailedResult.Success);
            Assert.AreEqual("m", distanceColumn!.Unit);
            
            // We know that the total distance in this file was around 44 km
            var dataItems = distanceColumn.EnumerateDataItems().ToList();
            Assert.AreEqual(44000, dataItems.Last().RawData, 1000);

            var s = dataItems[42].ToString();
        }

        [TestMethod]
        public void Test_BuildIntegral_With_Distance_WithUnitConversion_OK_2()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "LogWithSpeed-2022-11-23.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            using var reader = new StreamReader(fileStream);
            CsvTable table = CsvTable.Open(reader, ",");

            var channelMap = ChannelMaps.CreateDefaultMap();
            VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);

            var distanceBuilder = ColumnBuilderConfiguration.Create("Distance", "VIRT_DISTANCE")
                .SelectFirst(
                    ColumnSpec.ChannelIdIs("IDE00075"),
                    ColumnSpec.TitleContains("Vehicle speed"))
                .IntegrateByTime()
                .ConvertUnit("m");

            var detailedResult = distanceBuilder.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var distanceColumn);
            Assert.IsTrue(detailedResult.Success);
            Assert.AreEqual("m", distanceColumn!.Unit);

            // We know that the total distance in this file was around 44 km
            var dataItems = distanceColumn.EnumerateDataItems().ToList();
            Assert.AreEqual(44000, dataItems.Last().RawData, 1000);

            var s = dataItems[42].ToString();
        }

        [TestMethod]
        public void Test_BuildIntegral_With_Distance_WithUnitConversion_BadSource()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "VcsdLogWithHeader.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            using var reader = new StreamReader(fileStream);
            CsvTable table = CsvTable.Open(reader, ",");

            var channelMap = ChannelMaps.CreateDefaultMap();
            VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);

            var distanceBuilder = ColumnBuilderConfiguration.Create("Distance", "VIRT_DISTANCE")
                .Calculate.IntegralByTime.Over(new ColumnBuilderConfiguration().SelectFirst(
                    ColumnSpec.ChannelIdIs("IDE00076"),
                    ColumnSpec.TitleContains("Vehicl speed")))
                .ConvertUnit("m");

            var detailedResult = distanceBuilder.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var distanceColumn);
            Assert.IsFalse(detailedResult.Success);
            Assert.AreEqual("Failed to build source column.", detailedResult.Problem.Error);
            Assert.AreEqual("Failed to resolve source column.", detailedResult.Problem.Reasons[0].Error);
            Assert.AreEqual("None of the provided ColumnSpecs matched any of the provided source columns.", detailedResult.Problem.Reasons[0].Reasons[0].Error);
        }

        [TestMethod]
        public void Test_ChangeOverTime()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "LogWithEGR-2024-02-26-1.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            using var reader = new StreamReader(fileStream);
            CsvTable table = CsvTable.Open(reader, ",");

            var channelMap = ChannelMaps.CreateDefaultMap();
            VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);

            var egrMassFlowBuiler = ColumnBuilderConfiguration.Create("Total LP EGR mass flow, average over 30 s", "VIRT_LPEGR_MASS_FLOW")
                .Calculate.IntegralByTime.Over(new ColumnBuilderConfiguration().Select(ColumnSpec.ChannelIdIs("IDE07086")))
                .Calculate.RunningChange.Over(TimeSpan.FromSeconds(30))
                .ConvertUnit("kg/h");

            var detailedResult = egrMassFlowBuiler.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var egrColumn);
            Assert.IsTrue(detailedResult.Success);
            Assert.AreEqual("kg/h", egrColumn!.Unit);
            var dataPoints = egrColumn.EnumerateDataItems().ToList(); 
            
            // Technically, we'd need to verify the calculated result with manufactured data
        }

        [TestMethod]
        public void Test_LinearTransformation()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "VcsdLogWithFuelData.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            using var reader = new StreamReader(fileStream);
            CsvTable table = CsvTable.Open(reader, ",");

            var channelMap = ChannelMaps.CreateDefaultMap();
            VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);

            var fuelInParrotBuilder = ColumnBuilderConfiguration.Create("Fuel in parrots", "VIRT_FUEL_IN_PARROTS")
                .Select(ColumnSpec.ChannelIdIs("IDE00371"))
                .TransformLinear(7, 3.5);

            var detailedResult = fuelInParrotBuilder.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var fuelColumn);
            Assert.IsTrue(detailedResult.Success);
            Assert.AreEqual("l/h", fuelColumn!.Unit);
            var dataPoints = fuelColumn.EnumerateDataItems().ToList();

            Assert.AreEqual(0.70 * 7 + 3.5, dataPoints[0].RawData);
            Assert.AreEqual(0.80 * 7 + 3.5, dataPoints[1].RawData);
            Assert.AreEqual(0.75 * 7 + 3.5, dataPoints[2].RawData);
        }

        [TestMethod]
        public void Test_BuildIntegral_With_Distance_WithWrongUnitConversion()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "VcsdLogWithHeader.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            using var reader = new StreamReader(fileStream);
            CsvTable table = CsvTable.Open(reader, ",");

            var channelMap = ChannelMaps.CreateDefaultMap();
            VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);

            var distanceBuilder = ColumnBuilderConfiguration.Create("Distance", "VIRT_DISTANCE")
                .Calculate.IntegralByTime.Over(new ColumnBuilderConfiguration().SelectFirst(
                    ColumnSpec.ChannelIdIs("IDE00075"),
                    ColumnSpec.TitleContains("Vehicle speed")))
                .ConvertUnit("s");

            var detailedResult = distanceBuilder.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var distanceColumn);
            Assert.IsFalse(detailedResult.Success);
            Assert.AreEqual("Unit 'km' of column 'VIRT_DISTANCE/Distance' cannot be converted to target unit 's'.", detailedResult.Problem.Error);
        }

        [TestMethod]
        public void Test_MultiplyColumns()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "LogWithEGR-2024-02-26-1.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            using var reader = new StreamReader(fileStream);
            CsvTable table = CsvTable.Open(reader, ",");

            var channelMap = ChannelMaps.CreateDefaultMap();
            VcdsTableColumnizer vcdsRecordedFile = VcdsTableColumnizer.Open(table, channelMap);

            var NOxMassFlowBuilder = ColumnBuilderConfiguration
                .Create("NOx 1 mass flow", "VIRT_NOX1_MASS_FLOW")
                .Select(ColumnSpec.ChannelIdIs("IDE04098/1"))
                .MultiplyBy(new ColumnBuilderConfiguration().Select(ColumnSpec.ChannelIdIs("ENG247045")))
                .MultiplyBy(46d / 29000000)
                .ConvertUnit("mg/s");

            var detailedResult = NOxMassFlowBuilder.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var noxMassFlowColumn);
            var t = NOxMassFlowBuilder.ToString();
            var dataPoints = noxMassFlowColumn.EnumerateDataItems().ToList();

            var NOxMassBuilder = ColumnBuilderConfiguration.Create("NOx 1 mass", "VIRT_NOX1_MASS")
                .Calculate.IntegralByTime.Over(NOxMassFlowBuilder);
            detailedResult = NOxMassBuilder.TryBuild(vcdsRecordedFile.DiscreteDataColumns, out var noxMassColumn);

            var NOxMass_dataPoints = noxMassColumn.EnumerateDataItems().ToList();
        }
    }
}
