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

Whereas some columns are useful directly, other values that may need to be plotted must actually be calculated from other columns. 
For example, there is usually no column for EGR rate, but there are columns for exhaust mass flow and for EGR mass flow.

__1.3. Vehicle dependency__

Channel Ids and titles are highly vehicle dependent. If we want to give some semantic to columns,
we need to map technical channel ids to semantic channel ids. For example, a channel VIRT_VEHICLE_SPEED
could point to technical channel id IDE00075.


__1.3 Goal__

__1.3.1 Semantic columns__ 

It must be possible to create a configuration file that defines semantic columns. These semantic columns must have a specific semantic, 
like a column representing vehicle speed, no matter if it has different channel ids in different engines or different titles in different
languages.

We want a configuration that allows something like:

    var traveledDistanceColumnBuilder = ColumnBuilderConfiguration
        .Create("Traveled distance", "VIRT_DISTANCE")
        .Select(ColumnSpec.ChannelIdIs("IDE00075"))
        .IntegrateByTime()
        .ConvertUnit("m");

This kind of setup can be put into a configuration file relatively easily.


We need the following building blocks:
- Select a single column by its ChannelId or channel title
  => the title is localized, so it won't help much
- Select the first column out of a list of columns that exist

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

Example:

    var traveledDistanceColumnBuilder = ColumnBuilderConfiguration
        .Create("Traveled distance", "VIRT_DISTANCE")
        .Select(ColumnSpec.ChannelIdIs("IDE00075"))
        .IntegrateByTime()
        .ConvertUnit("m");

__2.2. Configuration files__

It must be possible to define a `ColumnBuilderConfiguration` in a configuration file. For example, if the same
piece of information is available under different ChannelIds in different vehicles, a configuration file might
specify a virtual column with a virtual ChannelId, which basicly defines which real ChannelIds zu try. This
is shown above with a virtual column with ChannelId `VIRT_DISTANCE`.

