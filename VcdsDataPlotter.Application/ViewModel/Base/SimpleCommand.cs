using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace VcdsDataPlotter.Gui.ViewModel.Base
{
    public class SimpleCommand : ICommand
    {
        public SimpleCommand(Action action) : this(_ => action()) { }
        public SimpleCommand(Action<object?> action) => this.action = action;
        public SimpleCommand(Action action, Func<bool> canExecute) : this(_ => action(), _ => canExecute()) { }

        public SimpleCommand(Action action, Func<bool> canExecute, INotifyPropertyChanged referenceViewModel, params string[] propertyNames)
            : this(action, canExecute)
        {
            this.referenceViewModel = referenceViewModel;
            AttachToReferenceViewModel();
        }

        public SimpleCommand(Action<object?> action, Func<object?, bool> canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public SimpleCommand(Action<object?> action, Func<object?, bool> canExecute, INotifyPropertyChanged referenceViewModel, params string[] propertyNames)
        {
            this.action = action;
            this.canExecute = canExecute;

            this.referenceViewModel = referenceViewModel;
            AttachToReferenceViewModel();
        }

        private void AttachToReferenceViewModel()
        {
            if (referenceViewModel is not null)
            {
                WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(
                    referenceViewModel, nameof(referenceViewModel.PropertyChanged), HandleReferencePropertyChanged);
            }
        }

        private void HandleReferencePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (this.referencePropertyNames is { Length : >0 })
            {
                if (this.referencePropertyNames.Any(x => x == e.PropertyName))
                {
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => (canExecute ?? (_ => true))(parameter);
        public void Execute(object? parameter) => action?.Invoke(parameter);

        private Action<object?> action;
        private Func<object?, bool>? canExecute;
        private INotifyPropertyChanged? referenceViewModel;
        private string[]? referencePropertyNames;
    }
}
