using Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DashboardFrontend.ValueConverters
{
    public class IsValidationTestBoolConverter : IValueConverter
    {

        /// <returns>True if value is a validation test.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is ValidationTest;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
