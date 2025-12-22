VCDS records columns which you might not just want to visualize, but on which you might want to perform calculations. 
Also, it is somewhat difficult to find columns which have a specific meaning, because either the ChannelId differs 
between vehicles, or because they are provided in a different form. 

We want a configuration that allows something like:

        public static ColumnSelector VehicleSpeed = ColumnSelectors.SelectFirst(
            InputColumnSpec.ChannelIdIs("IDE00075"),
            InputColumnSpec.TitleContains("Vehicle speed"));

and

    public class CalculatedColumnDefinitions
    {
        public static CalculatedColumnDefinition TraveledDistance = FormulaDefinitions.IntegralByTime.Over(KnownInputColumnSpecs.VehicleSpeed);
    }

We need the following building blocks:
- Select a single column by its ChannelId or channel title
  => the title is localized, so it won't help much
- Select the first column out of a list of columns that exist

__1. Idea 1__

A ColumnBuilder contains a configuration from which it can find or create a calculated column:

```
    public abstract class ColumnBuilder
    {
        public abstract IDiscreteDataColumn TryBuild(IEnumerable<IDiscreteDataColumn> sourceColumns);
    }
```
The resulting IDiscreteDataColumn represents that column that was configured before.
It can be one of the columns in `sourceColumns`, or it can be a calculated column.

A semantic column can then look like this:

```
   public class Columns
   {
        public static ColumnBuilder VehicleSpeed = new ColumnBuilder().SelectFirst(
            InputColumnSpec.ChannelIdIs("IDE00075"),
            InputColumnSpec.TitleContains("Vehicle speed"));
   }
```

Then, a calculated column can look like this:

    public class CalculatedColumnDefinitions
    {
        public static ColumnBuilder TraveledDistance = ColumnBuilder().Calculate
            IntegralByTime.Over(KnownInputColumnSpecs.VehicleSpeed);
    }

In this case
* `ColumnBuilder` must have a function `Calculate`, returning a `CalculatedColumnConfigurationBuilder`
* `CalculatedColumnConfigurationBuilder` must define a function `IntegralByTime`
* `IntegralByTime` must return a `ColumnIntegralBuilder` 
* `ColumnIntegralBuilder` must have an `Over` function, which must return a column build which, when materializing a column,
creates a IntegralByTimeColumn using the VehicleSpeed column as input.