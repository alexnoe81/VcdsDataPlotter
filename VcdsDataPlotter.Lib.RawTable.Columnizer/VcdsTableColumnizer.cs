using System.Globalization;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;
using VcdsDataPlotter.Lib.RawTableReader.Interface;

namespace VcdsDataPlotter.Lib.RawTable.Columnizer;

/*
* A VCDS data log is read using a filter chain:
*  - A file exposes discrete values with a time stamp, possibly several values aggregated in one column, like for NOx sensors or temperature sensors.
*    In older versions, VCDS mobiles put units into each cell
*  
*  - Convert strings in value columns to double:
*    - Most columns contain one single numeric value, so a simple parser converts raw strings into doubles
*    - A splitter decomposes aggregated source columns into single values, like single temperature sensor values
*    => at this point, we have data in the form of (time stamp, sensor value as double)
*
*  - Aggregators: Some semantic columns may need input from several other columns, like AdBlue mass flow
*    - can use total AdBlue consumption of vehicle since the begin of time as well as current AdBlue mass flow
*    - NOx sensors: we may be "NOx-sensors" in the form of " 42ppm / 5ppm", or we may have separate columns
*      => a virtual NOx sensor column must choose where to take its values from
*  
*  => at this point, we have semantic columns like "AdBlue consumption" or "Exhaust temperature sensor 3" 
*  => this graph should be generated automatically from input columns if we know what input columns look like
* 
*  - These semantic columns may undergo aggregation, like Total Distance = ∫ v(t) dt
*  - Configurable semantic columns may undergo aggregation, like 1 km rolling average of AdBlue consumption per 1.000 km 
*/
public class VcdsTableColumnizer
{
    private VcdsTableColumnizer(IRawTable rawTable)
    {
        this.rawTable = rawTable ?? throw new ArgumentNullException(nameof(rawTable));
        this.rawColumnMap = CreateDefaultMap();
    }

    private VcdsTableColumnizer(IRawTable rawTable, RawColumn2ValueColumnMap rawColumnMap)
    {
        this.rawTable = rawTable ?? throw new ArgumentNullException(nameof(rawTable));
        this.rawColumnMap = rawColumnMap ?? throw new ArgumentNullException(nameof(rawColumnMap));
    }

    public static VcdsTableColumnizer Open(IRawTable rawTable)
    {
        var result = new VcdsTableColumnizer(rawTable);
        result.Initialize();
        return result;
    }

    public static VcdsTableColumnizer Open(IRawTable rawTable, RawColumn2ValueColumnMap rawColumnMap)
    {
        var result = new VcdsTableColumnizer(rawTable, rawColumnMap);
        result.Initialize();
        return result;
    }

    private void Initialize()
    {
        if (isInitialized)
            return;

        // In a VCDS file, column 0 is the marker index, column 1/2 contain data 1, column 3/4 contain data 2 etc
        List<VcdsRawDataColumn> columns = new List<VcdsRawDataColumn>();
        for (int j = 1; j < rawTable.NumberOfColumns; j += 2)
        {
            if (rawTable.GetCellContent(5, j) == "STAMP")
            {
                try
                {
                    var newColumn = new VcdsRawDataColumn(rawTable, j, j + 1);
                    newColumn.Initialize();

                    columns.Add(newColumn);
                }
                catch (Exception error)
                {
                    // Failed to initialize column?

                }
            }
            else
            {

            }
        }

        rawDataColumns = columns.ToArray();

        // Now build columns that interpret raw columns, either by simply parsing double values, or by
        // extracting single values from columns that contain multiple columns
        BuildDiscreteValueColumns();

        // Extract some meta data from file
        var dayString = rawTable.GetCellContent(0, 1);
        var monthString = rawTable.GetCellContent(0, 2);
        var yearString = rawTable.GetCellContent(0, 3);
        var timeString = rawTable.GetCellContent(0, 4)?.Substring(0, 8);
        var hourString = timeString?.Substring(0, 2);
        var minuteString = timeString?.Substring(3, 2);
        var secondsString = timeString?.Substring(6, 2);
        var monthNames = CultureInfo.GetCultureInfo("en-US").DateTimeFormat.MonthNames;

        try
        {
            var day = int.Parse(dayString);
            var month = 1 + Array.FindIndex(monthNames, x => string.Compare(x, monthString, true) == 0);
            var year = int.Parse(yearString);
            var hour = int.Parse(hourString);
            var minute = int.Parse(minuteString);
            var seconds = int.Parse(secondsString);

            RecordingTimestamp = new DateTime(year, month, day, hour, minute, seconds, DateTimeKind.Unspecified);
        }
        catch (Exception ex)
        {
            throw new FormatException("Failed to parse recording time.", ex);
        }

        isInitialized = true;
    }

    private void BuildDiscreteValueColumns()
    {
        List<IDiscreteDataColumn> temp = new List<IDiscreteDataColumn>();

        if (rawColumnMap is not null)
        {
            foreach (var column in rawDataColumns)
            {
                if (rawColumnMap.TryGetMapping(channelId: column.ChannelId, out var mapping))
                {
                    temp.AddRange(mapping!.CreateColumns(column));
                }
            }

            discreteDataColumns = temp.ToArray();
        }
    }

    private void CheckInitialized()
    {
        if (!isInitialized)
            throw new InvalidOperationException("Object is not initialized.");
    }

    private static RawColumn2ValueColumnMap CreateDefaultMap()
    {
        RawColumn2ValueColumnMap map = new();
        map.Mappings =
        [
            new RawColumn2ValueColumnMapItem()
            {
                ChannelId = "IDE04090", Output =
                [
                    new RawColumn2ValueColumnMapItemItem() { TypeName = typeof(DiscreteExhaustTemperatureSensorColumn).FullName, Arguments = [1] },
                    new RawColumn2ValueColumnMapItemItem() { TypeName = typeof(DiscreteExhaustTemperatureSensorColumn).FullName, Arguments = [2] },
                    new RawColumn2ValueColumnMapItemItem() { TypeName = typeof(DiscreteExhaustTemperatureSensorColumn).FullName, Arguments = [3] },
                    new RawColumn2ValueColumnMapItemItem() { TypeName = typeof(DiscreteExhaustTemperatureSensorColumn).FullName, Arguments = [4] },
                ]
            },

            new RawColumn2ValueColumnMapItem()
            {
                ChannelId = "IDE04098", Output =
                [
                    new RawColumn2ValueColumnMapItemItem() { TypeName = typeof(DiscreteNoxSensorsColumn).FullName, Arguments = [1] },
                    new RawColumn2ValueColumnMapItemItem() { TypeName = typeof(DiscreteNoxSensorsColumn).FullName, Arguments = [2] }
                ]
            },

            new RawColumn2ValueColumnMapItem()
            {
                ChannelId = "*", Output =
                [
                    new RawColumn2ValueColumnMapItemItem() { TypeName = typeof(SimpleDiscreteNumericColumn).FullName, Arguments = new object[0] }
                ]
            }
        ];

        return map;
    }

    public DateTime RecordingTimestamp { get; private set; }

    public IRawDataColumn[] RawDataColumns => rawDataColumns;
    private VcdsRawDataColumn[] rawDataColumns;

    public IDiscreteDataColumn[] DiscreteDataColumns => discreteDataColumns;
    private IDiscreteDataColumn[] discreteDataColumns;

    private IRawTable rawTable;
    private bool isInitialized;
    private RawColumn2ValueColumnMap rawColumnMap;
}
