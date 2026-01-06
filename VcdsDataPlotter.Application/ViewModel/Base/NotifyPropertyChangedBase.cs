using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Gui.ViewModel.Base
{
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        protected NotifyPropertyChangedBase() { }

        protected void SetProperty<T>(ref T field, T newValue) => SetProperty(ref field, newValue, (Action<T, T>?)null);
        protected void SetProperty<T>(ref T field, T newValue, Action? onChanged) => SetProperty(ref field, newValue, (_, _) => onChanged?.Invoke());

        protected void SetProperty<T>(ref T field, T newValue, Action<T, T>? onChanged)
        {
            if (field is null && newValue is null)
                return;

            var oldValue = field;
            if (field is null ^ newValue is null)
            {
                field = newValue;
                onChanged?.Invoke(oldValue, newValue);
            }

            if (ReferenceEquals(field, newValue))
                return;

            field = newValue;
            onChanged?.Invoke(oldValue, newValue);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
