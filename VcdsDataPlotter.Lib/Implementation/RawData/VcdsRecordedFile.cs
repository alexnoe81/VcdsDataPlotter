using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.Interface;

namespace VcdsDataPlotter.Lib.Implementation.RawData;

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
public class VcdsRecordedFile
{
    private VcdsRecordedFile(IRawTable rawTable, RawColumn2ValueColumnMap rawColumnMap)
    {
        this.rawTable = rawTable ?? throw new ArgumentNullException(nameof(rawTable));
        this.rawColumnMap = rawColumnMap ?? throw new ArgumentNullException(nameof(rawColumnMap));
    }

    public static VcdsRecordedFile Open(IRawTable rawTable, RawColumn2ValueColumnMap rawColumnMap)
    {
        var result = new VcdsRecordedFile(rawTable, rawColumnMap);
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
        BuildReaders();

        isInitialized = true;
    }

    private void BuildReaders()
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

    public IRawDataColumn[] RawDataColumns => rawDataColumns;
    private VcdsRawDataColumn[] rawDataColumns;

    public IDiscreteDataColumn[] DiscreteDataColumns => discreteDataColumns;
    private IDiscreteDataColumn[] discreteDataColumns;

    private IRawTable rawTable;
    private bool isInitialized;
    private RawColumn2ValueColumnMap rawColumnMap;
}
