using Quester.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Quester.Controls
{
    public class InverseBooleanConverter : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ProjectPathFormatConverter : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");
            if (value == null)
                return String.Empty;

            string defaultProjectDir = IOHelper.GetDefaultProjectDir();

            if (((string)value).Equals(defaultProjectDir))
                return "Default directory is selected.";
            else
                return value;
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
