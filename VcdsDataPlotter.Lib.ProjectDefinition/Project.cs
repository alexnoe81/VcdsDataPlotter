using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VcdsDataPlotter.Lib.CalculatedColumns.ConfigFiles;

namespace VcdsDataPlotter.Lib.ProjectDefinition
{

    [XmlType("Project")]
    public class Project
    {
        /// <summary>
        /// Theoretically, a project can contain several documents, even though for the beginning, it won't.
        /// </summary>
        public Document[] Documents { get; set; }
    }

    /// <summary>
    /// This class defines settings of one savable and loadable document inside a projects, such as
    /// - which CSV file was loaded
    /// - which definition of semantic and calculated columns was used
    /// - which settings were used for axes, e.g. min-max or label
    /// - which additional columns were added to the project, like running averages of any column
    /// - which columns were selected for plotting
    /// </summary>
    [XmlType("Document")]
    public class Document
    {
        /// <summary>
        /// Path of the CSV file, preferably relative to the project file
        /// </summary>
        public FilePath? DataFilePath { get; set; }

        /// <summary>
        /// Path of the semantic columns definition file, preferably either relative to the cfg directory,
        /// or relative to the project file
        /// </summary>
        public FilePath? SemanticColumnsFilePath { get; set; }

        /// <summary>
        /// Path of the calculated columns definition file, preferably either relative to the cfg directory,
        /// or relative to the project file
        /// </summary>
        public FilePath? CalculatedColumnsFilePath { get; set; }

        [XmlIgnore]
        public string[]? SelectedColumns { get; set; }
        [XmlElement("SelectedColumns")]
        public string SelectedColumnsString
        {
            get => string.Join(";", (SelectedColumns ?? Array.Empty<string>()).Select(x => x.Trim()));
            set
            {
                if (value is null)
                {
                    SelectedColumns = null;
                }
                else 
                {
                    SelectedColumns = value.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                }
            }
        }


        [XmlArray("CustomColumns")]
        [XmlArrayItem("CustomColumn")]
        public ColumnBuilderConfigurationDefinition[]? CustomColumns { get; set; }

        /// <summary>
        /// Defines the primary X axis (bottom)
        /// </summary>
        public AxisSpec? X1 { get; set; }

        /// <summary>
        /// Defines the primary Y axis (left)
        /// </summary>
        public AxisSpec? Y1 { get; set; }

        /// <summary>
        /// Defines the secondary X axis (top). This may be null.
        /// </summary>
        public AxisSpec? X2 { get; set; }

        /// <summary>
        /// Defines the secondary Y axis (right). This may be null.
        /// </summary>
        public AxisSpec? Y2 { get; set; }
    }
}
