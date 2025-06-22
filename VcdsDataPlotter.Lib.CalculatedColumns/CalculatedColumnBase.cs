using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.CalculatedColumns
{


    public abstract class CalculatedColumnBase
    {
        protected CalculatedColumnBase() { }

       



    }

    public abstract class InitializableCalculatedColumnBase
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
