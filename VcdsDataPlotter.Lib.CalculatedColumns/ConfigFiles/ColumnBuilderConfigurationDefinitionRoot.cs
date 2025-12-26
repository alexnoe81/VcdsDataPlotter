using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ConfigFiles
{
    public class ColumnBuilderConfigurationDefinitionRoot
    {
        public static ColumnBuilderConfigurationDefinitionRoot Load(string filePath)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                return Load(fileStream);
            }
        }

        public static ColumnBuilderConfigurationDefinitionRoot Load(Stream source)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ColumnBuilderConfigurationDefinitionRoot));
            object result = serializer.Deserialize(source);

            return (ColumnBuilderConfigurationDefinitionRoot)result;
        }

        public ColumnBuilderConfigurationDefinition[] Columns { get; set; }
    }
}
