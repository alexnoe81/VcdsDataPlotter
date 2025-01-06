using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace VcdsDataPlotter.Gui.Converter
{
    public class FilePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string valueString)
                return null;

            FileInfo fileInfo = new(valueString);

            if (parameter is string parameterString && parameterString.ToLowerInvariant() == "name")
                return fileInfo.Name;

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
