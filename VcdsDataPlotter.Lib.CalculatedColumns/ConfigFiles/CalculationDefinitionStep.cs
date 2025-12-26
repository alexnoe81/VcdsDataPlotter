using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ConfigFiles;


[XmlInclude(typeof(Select))]
[XmlInclude(typeof(SelectFirst))]
[XmlInclude(typeof(MultiplyBy))]
[XmlInclude(typeof(DevideBy))]
[XmlInclude(typeof(IntegrateByTime))]
[XmlInclude(typeof(CalculateRunningAverage))]
[XmlInclude(typeof(DifferenceToFirstRow))]
[XmlInclude(typeof(ConvertUnit))]
[XmlInclude(typeof(ReassignUnit))]
public abstract class CalculationDefinitionStep
{
    public abstract ColumnBuilderConfiguration CreateConfiguration(ColumnBuilderConfiguration parent);
}
