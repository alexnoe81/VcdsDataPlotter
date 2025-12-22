using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders
{
    public class ColumnBuilderBuildingResult
    {
        /// <summary>
        /// True if the column was build successfully, false otherwise
        /// </summary>
        public bool Success => this.success;

        public ColumnBuilderBuildFailure Problem => this.problem;


        private ColumnBuilderBuildingResult(bool success)
        {
            this.success = success;
            this.problem = null;
        }

        internal static ColumnBuilderBuildingResult CreateError(ColumnBuilderBuildFailure problem)
        { 
            var result = new ColumnBuilderBuildingResult(false);
            
            result.problem = problem;
            return result;
        }

        public override string ToString()
        {
            if (success)
                return "Success";
            else
                return $"{problem}";
        }

        internal static ColumnBuilderBuildingResult SuccessfulResult = new ColumnBuilderBuildingResult(true);


        public static implicit operator bool(ColumnBuilderBuildingResult other) => other.success;


        private bool success;
        private ColumnBuilderBuildFailure problem;
    }
}
