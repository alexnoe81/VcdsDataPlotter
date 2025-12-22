using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.CalculatedColumns.Columns
{
    /// <summary>
    /// Base class for calculated columns. This base class can be used directly if the calculated column
    /// makes a simple transformation that does not require caching the result. Columns that want to calculate
    /// their output once and cache it should derive from InitializableCalculatedColumnBase.
    /// </summary>
    public abstract class CalculatedColumnBase
    {
        protected CalculatedColumnBase() { }
    }

    public abstract class InitializableCalculatedColumnBase : CalculatedColumnBase
    {
        protected void Initialize()
        {
            InternalInitialize();
            isInitialized = true;
        }

        protected abstract void InternalInitialize();

        protected void InitializeIfRequired()
        {
            if (!isInitialized)
                Initialize();
        }

        protected void CheckInitialized()
        {
            if (!isInitialized)
                throw new InvalidOperationException("Call 'Initialize()' before using this object.");
        }


        private bool isInitialized;
    }
}
