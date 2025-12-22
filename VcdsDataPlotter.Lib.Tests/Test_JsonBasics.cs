using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.Tests
{
    /*
    [TestClass]
    public class Test_JsonBasics
    {
        [TestMethod]
        public void TestSimple()
        {
            JsonSerializer js = new JsonSerializer();
            js.ConstructorHandling = ConstructorHandling.Default;
            js.Formatting = Formatting.Indented;

            SourceColumnReference obj = new SourceColumnReference();
            obj.ColumnSpec = new ColumnSpecById("IDE00042");

            StringBuilder sb = new StringBuilder();
            js.Serialize(new StringWriter(sb), obj);

            string result = sb.ToString();

         
            var deserialized = js.Deserialize(new StringReader(result), typeof(SourceColumnReference));

        }
    }

    public class SomeStringifyableStuff
    {
        public SomeStringifyableStuff(string s)
        {
            Value = s;
        }

        public String Value { get; set; }
    }

    public class SomeComplexStuff
    {
        public SomeComplexStuff()
        {
        }

        public SomeStringifyableStuff Value { get; set; }
    }

    public class SourceColumnReference
    {
        public ColumnSpec ColumnSpec { get; set; }
    }





    /// <summary>
    /// A ColumnSpec defines search criteria for a column
    /// </summary>
    public class ColumnSpec
    {
    }

    /// <summary>
    /// A ColumnSpecById finds a column with a measurement channel id
    /// </summary>
    public class ColumnSpecById : ColumnSpec
    {
        public ColumnSpecById(string columnId) => ColumnId = columnId ?? throw new ArgumentNullException(nameof(columnId));
        public string ColumnId { get; set; }
    }

    /// <summary>
    /// A ColumnSpecByChoice finds the first column that exists
    /// </summary>
    public class ColumnSpecByChoice : ColumnSpec
    {
        public ColumnSpec[] Columns { get; set; }
    }*/
}
