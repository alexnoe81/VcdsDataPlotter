using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.Columns;
using VcdsDataPlotter.Lib.CalculatedColumns.Math;
using VcdsDataPlotter.Lib.RawTable.Columnizer;
using VcdsDataPlotter.Lib.RawTableReader;

namespace VcdsDataPlotter.Lib.Tests.TestCalculatedColumns
{
    [TestClass]
    public class Test_IntegralByTimeColumn
    {
        [TestMethod]
        public void Test_Simple()
        {
            string sampleFilePath = Path.Combine("TestData", "SimpleCsv", "VcsdLogWithFuelData.csv");

            using var fileStream = File.OpenRead(sampleFilePath);
            CsvTable reader = CsvTable.Open(new StreamReader(fileStream), ",");

            var vcdsFile = VcdsTableColumnizer.Open(reader);
            var fuelColumn = vcdsFile.DiscreteDataColumns.Where(x => x.Title.Contains("Fuel")).First();

            IntegralByTimeColumn testee = IntegralByTimeColumn.Create("Total fuel", "VIRT_FUEL", fuelColumn);
            
            foreach (var item in testee.EnumerateDataItems())
            {

            }
        }

    }
}
