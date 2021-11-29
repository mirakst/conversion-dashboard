using System;
using System.Globalization;
using System.Windows.Data;

namespace DashboardFrontend.ValueConverters
{
    public class SqlNotEmptyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string sql && sql != string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
