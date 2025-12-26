using System;
using System.Xml.Serialization;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ConfigFiles
{
    /// <summary>
    /// A ColumnBuilderConfigurationDefinition is a serializable definition that specifies how to build a 
    /// ColumnBuilderConfiguration. This class is intended to be used to store column definitions in a
    /// configuration file.
    /// </summary>
    /// <remarks>
    /// Somehow use this:
    /// https://stackoverflow.com/questions/1533335/how-to-exclude-null-properties-when-using-xmlserializer
    /// </remarks>
    [XmlType("Column")]
    public class ColumnBuilderConfigurationDefinition
    {
        [XmlAttribute("channelId")]
        public string? ChannelId { get; set; }

        [XmlAttribute("title")]
        public string? Title { get; set; }

        // We don't use CalculationDefinition here because we don't want that layer in the XML file
        [XmlElement("Instruction")]
        public CalculationDefinitionStep[] Steps { get; set; }

        /// <summary>
        /// Creates a ColumnBuilderConfiguration from the current object.
        /// </summary>
        /// <returns></returns>
        public ColumnBuilderConfiguration Build()
        {
            if (ChannelId is null)
                throw new InvalidOperationException("ChannelId must be specified.");
            if (Title is null)
                throw new InvalidOperationException("Title must be specified.");
            if (Steps is null)
                throw new InvalidOperationException("Steps must be specified.");
            if (Steps.Count() < 1)
                throw new InvalidOperationException("There must be at least 1 step.");

            var newConfiguraton = ColumnBuilderConfiguration.Create(Title, ChannelId);

            // Convert Steps to transformations. Steps contains all calculation steps in execution order,
            // meaninig that each step must invoke a function like Add or Multiply on the precedingly created
            // ColumnBuilderConfiguration.
            foreach (var nextStep in Steps)
            {
                newConfiguraton = nextStep.CreateConfiguration(newConfiguraton);
            }

            return newConfiguraton;
        }
    }

    /// <summary>
    /// This is very similar, but without ChannelId or Title, and is used to define a SelectFirst
    /// clause.
    /// </summary>
    public class CalculationDefinition
    {
        [XmlElement("Instruction")]
        public CalculationDefinitionStep[] Steps { get; set; }

        public ColumnBuilderConfiguration Build()
        {
            if (Steps is null)
                throw new InvalidOperationException("Steps must be specified.");
            if (Steps.Count() < 1)
                throw new InvalidOperationException("There must be at least 1 step.");

            var newConfiguraton =  new ColumnBuilderConfiguration();

            // Convert Steps to transformations. Steps contains all calculation steps in execution order,
            // meaninig that each step must invoke a function like Add or Multiply on the precedingly created
            // ColumnBuilderConfiguration.
            foreach (var nextStep in Steps)
            {
                newConfiguraton = nextStep.CreateConfiguration(newConfiguraton);
            }

            return newConfiguraton;
        }
    }
}
