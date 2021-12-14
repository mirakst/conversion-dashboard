using Model;
using System;
using System.Globalization;
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
