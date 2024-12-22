using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.Interface;

namespace VcdsDataPlotter.Lib.Implementation.RawData
{
    /// <summary>
    /// A VcdsRawDataColumn aggregates a timestamp column and a data column to a logical column containing
    /// timestamped data. It can, however, not interpolate data, because its values might be any kind of string,
    /// such as 4 temperature sensor readings. A filter chain must be build to actually get parsable data.
    /// 
    /// Besides that, it extracts meta data like title and channel id.
    /// </summary>
    [DebuggerDisplay("ChannelId = {ChannelId} [Explanation = {Title}]")]
    public class VcdsRawDataColumn : IRawDataColumn
    {
        public VcdsRawDataColumn(IRawTable rawTable, int timeStampColumnIndex, int dataColumnIndex)
        {
            this.rawTable = rawTable ?? throw new ArgumentNullException(nameof(rawTable));
            this.timeStampColumnIndex = timeStampColumnIndex;
            this.dataColumnIndex = dataColumnIndex;
        }

        public void Initialize()
        {
            // Row 4 must be "TIME" / "Loc. xxxxx"
            // Row 5 must be "STAMP" / <Explanation of Loc. xxxxx"
            // Row 6 contains the unit. Long term goal: interpret unit string
            if (initialized)
                return;

            ChannelId = rawTable.GetCellContent(4, dataColumnIndex)?.Replace("Loc.", "").Trim();
            Title = rawTable.GetCellContent(5, dataColumnIndex);
            RawUnit = rawTable.GetCellContent(6, dataColumnIndex);

            // Find the first row in which the time stamp is a time stamp
            for (int j = 0; j < 20; j++)
            {
                string? rawTimestamp = rawTable.GetCellContent(j, timeStampColumnIndex);
                if (rawTimestamp is null && j >= 10)
                    break;

                if (double.TryParse(rawTimestamp, DefaultCulture, out var timeStamp))
                {
                    firstDataRow = j;
                    break;
                }
            }

            if (firstDataRow is null)
            {
                throw new InvalidOperationException("Did not find any relevant time stamp.");
            }

            initialized = true;
        }

        public IEnumerable<RawDataItem> EnumerateDataItems()
        {
            CheckInitialized();

            int currentRow = firstDataRow!.Value;
            while (true)
            {
                try
                {
                    // There were older versions of VCDS mobile which sometimes dropped values in a way so that
                    // there were fewer columns in a row than there were supposed to be. 
                    //
                    // TODO: Add an additional filter that removes rows with bad column count instead of doing this
                    // here, because if one column is missing in the middle, values coming after the missing value
                    // are attributed to the wrong column header.
                    if (currentRow >= rawTable.NumberOfRows)
                        break;

                    var rawTimestamp = rawTable.GetCellContent(currentRow, timeStampColumnIndex);
                    var rawData = rawTable.GetCellContent(currentRow, dataColumnIndex);

                    if (rawTimestamp is null || rawData is null)
                        continue;

                    // There may be "N/A" items
                    if (!double.TryParse(rawTimestamp, DefaultCulture, out var timeStamp))
                        continue;

                    yield return new RawDataItem(TimeSpan.FromSeconds(timeStamp), rawData);
                }
                finally
                {
                    currentRow++;
                }
            }
        }

        public string? Title { get; private set; }
        public string? ChannelId { get; private set; }
        public string? RawUnit { get; private set; }

        private void CheckInitialized()
        {
            if (!initialized)
                throw new InvalidOperationException("Object is not initialized.");
        }

        private int? firstDataRow;
        private bool initialized;
        private int timeStampColumnIndex;
        private int dataColumnIndex;
        private IRawTable rawTable;
        private static CultureInfo DefaultCulture = CultureInfo.InvariantCulture;
    }
}
