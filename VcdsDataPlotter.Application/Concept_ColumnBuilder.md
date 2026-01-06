__1. Introduction__

__1.1. Input data from VCDS__

VCDS records CSV files which have, besides a couple of headlines, columns pairs which have a time stamp column and a value column.
Some of these columns contain single values, like current fuel consumption in liters per hours, whereas others contain several values 
that need to be decomposed, like one column containing the values of four temperature sensors.

Each value column contains a channel id, a channel title, and also a unit:

Tuesday,23,December,2025,13:30:13:40001-VCID:2A066900CC76ED4AF07-807E,VCDS Version: Release 25.3.2 (x64),Data version: 20250822 DS365.0
05L 906 022 F,ADVMB,R4 2.0l TDI   H30 0987,
,,G004,F0,G008,F0,G016,F0,G261,F0,G332,F0,G348,F0,G475,F0,G837,F0,G960,F0,G1031,F0,G1046,F0,G1108,F0,

Marker,TIME,Loc. IDE00021,TIME,Loc. IDE00075,TIME,Loc. IDE00100
,STAMP,Engine speed,STAMP,Vehicle speed,STAMP,Engine torque
,, /min,, km/h,, Nm
,0.44,0,0.47,0,0.51,-500.0
,0.96,0,0.98,0,1.02,-500.0
,1.47,0,1.50,0,1.54,-500.0
...

__1.2. Data to visualize__

Some columns are useful directly, other values that may need to be plotted must be calculated from other columns. 
For example, there is usually no column for EGR rate, but there are columns for exhaust mass flow and for EGR mass flow.

__1.3. Vehicle dependency__

Channel Ids and titles are highly vehicle dependent, even if we do not take into account UI languages of VCDS. If we want
to give some meaning to columns, we need to map technical channel ids to semantic channel ids. 

For example, vehicle speed is usually IDE00075, but calculated actual low-pressure EGR mass flow is IDE09886 in EA288 engines,
but IDE07086 in EA288 evo engines.

__1.4 Semantic and calculated columns__

__1.4.1 Semantic columns__ 

It must be possible to create a configuration file that defines semantic columns. These semantic columns must have a specific semantic, 
like a column representing vehicle speed, no matter if it has different channel ids in different engines or different titles in different
languages.

__1.4.2 Calculated Columns__

Using semantic columns, we can defined often-needed calculated columns, such as:

    s = ∫ v(t) dt

or even

                 ∫ egr_massflow(t) dt
    EGR-rate = ------------------------
               ∫ exhaust_massflow(t) dt


__2. Idea__

__2.1. Introduction__

Basicly, we can create something that is configured like Serilog:
A ColumnBuilderConfiguration contains a configuration which is built step by step specifying which data to use,
which calculations to perform, and which conversions to perform:

```
    /// <summary>
    /// A ColumnBuilderConfiguration is used to create a description of a column that can be materialized
    /// from a list of input solumns
    /// </summary>
    public partial class ColumnBuilderConfiguration
    {
        public static ColumnBuilderConfiguration Create(string title, string channelId)
        ...
        public ColumnBuilderConfiguration Select(ColumnSpec spec)
        ...
        public ColumnBuilderConfiguration MultiplyBy(double other)
        ...
        public ColumnBuilderConfiguration MultiplyBy(ColumnBuilderConfiguration other)
        ...
        public virtual ColumnBuilderBuildingResult TryBuild(IList<IDiscreteDataColumn> sourceColumns, out IDiscreteDataColumn? result)
        ...
        public ColumnBuilderConfiguration ConvertUnit(string targetUnit)
        ...
    }
```
When calling TryBuild(...), the resulting IDiscreteDataColumn provides access to the configured data.

Then, we can do something like:

    var traveledDistanceColumnBuilder = ColumnBuilderConfiguration
        .Create("Traveled distance", "VIRT_VEHICLE_SPEED")
        .Select(ColumnSpec.ChannelIdIs("IDE00075"));

returning the traveled distance in meters. Or:

    var traveledDistanceColumnBuilder = ColumnBuilderConfiguration
        .Create("Traveled distance", "VIRT_TRAVELED_DISTANCE")
        .Select(ColumnSpec.ChannelIdIs("VIRT_VEHICLE_SPEED"))
        .IntegrateByTime()
        .ConvertUnit("m");

This kind of setup can be put into a configuration file relatively easily.


__2.2. Configuration files__

It must be possible to define a `ColumnBuilderConfiguration` in a configuration file.


__3. Implementation__

__3.1. Table reader__

The interface `IRawTable` represents a table that can be accessed by row and column. The class `CsvTable` reads a CSV formatted text file.

`VcdsTableColumnizer` reads an IRawTable, seperates head lines from data, and creates actual data columns from it.
If columns are known to contain several values in one string, it splits them.
=> result: `IDiscreteDataColumn`

__3.2. Calculated columns__

Everything is build upon `IDiscreteDataColumn`. Calculated columns are built using a `ColumnBuilderConfiguration`.
A `ColumnBuilderConfiguration` describes how to construct a column, such as: select column with id xyz, and then
integrate it by time.

__3.3. Defining `ColumnBuilderConfiguration` in configuration files__

Since `ColumnBuilderConfiguration` instances know their hierarchy and have a Source or Parent property, they cannot
be serialized and deserialized directly, at least not without making the resulting JSON or XML string look badly readable.

To avoid this, there is a `ColumnBuilderConfigurationDefinition`, which is contained inside a
`ColumnBuilderConfigurationDefinitionRoot`. These objects are XML serializeable, and are also human-readable enough
to actually read or write such files. After deserialization, the function `ColumnBuilderConfigurationDefinition.Build()` 
builds a `ColumnBuilderConfiguration`, as described in 3.2.

For example, the definition for "traveled distance" looks like this:

    <Column channelId="VIRT_TRAVELED_DISTANCE" title="Traveled distance">
      <Instruction xsi:type="Select">
        <ChannelId>VIRT_VEHICLE_SPEED</ChannelId>
      </Instruction>
      <Instruction xsi:type="IntegrateByTime" />
      <Instruction xsi:type="ConvertUnit">
        <TargetUnit>m</TargetUnit>
      </Instruction>
    </Column>

where VIRT_VEHICLE_SPEED is defined as:

    <Column channelId="VIRT_VEHICLE_SPEED" title="Vehicle speed">
      <Instruction xsi:type="Select">
        <ChannelId>IDE00075</ChannelId>
      </Instruction>
    </Column>