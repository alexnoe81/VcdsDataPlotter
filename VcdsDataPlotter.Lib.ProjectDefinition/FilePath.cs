using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VcdsDataPlotter.Lib.ProjectDefinition
{
    [XmlInclude(typeof(RelativePath))]
    [XmlInclude(typeof(AbsolutePath))]
    public abstract class FilePath
    {
        protected FilePath() { }
        protected FilePath(string path) => Path = path ?? throw new ArgumentNullException(nameof(path));

        public string Path { get; set; }
    }

    /// <summary>
    /// Represents a file path that can be relative to another non-fixed path, sich as 
    /// "relative to directory with default config files"
    /// </summary>
    public class RelativePath : FilePath
    {
        public RelativePath() { }
        public RelativePath(KnownBasePaths basePathKind, string path) : base(path) => BasePathKind = basePathKind;                
        public KnownBasePaths BasePathKind { get; set; }
        public override string ToString() => $"[{BasePathKind}]\\{Path}";
    }

    public class AbsolutePath : FilePath
    {
        public AbsolutePath() { }
        public AbsolutePath(string path) : base(path) { }
        public override string ToString() => Path;
    }

    public enum KnownBasePaths
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// cfg directory next to application exe file
        /// </summary>
        DefaultConfigFiles,

        /// <summary>
        /// This may be used if different definition files for semantic or calculated columns
        /// are used for a specific recorded file
        /// </summary>
        DataSource,

        ProjectFile,
    }
}
