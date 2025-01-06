using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VcdsDataPlotter.Gui.ViewModel.Base;

namespace VcdsDataPlotter.Gui.ViewModel
{
    public class MainWindowVM : ViewModelBase
    {
        public MainWindowVM()
        {
            CmdOpenVcdsCsvFile = new SimpleCommand(DoCmdOpenVcdsCsvFile);                
        }

        public ICommand CmdOpenVcdsCsvFile { get; private set; }
        
        public event EventHandler<EventArgs>? OnCmdOpenVcdsCsvFile;

        private void DoCmdOpenVcdsCsvFile() => OnCmdOpenVcdsCsvFile?.Invoke(this, EventArgs.Empty);
    }
}
