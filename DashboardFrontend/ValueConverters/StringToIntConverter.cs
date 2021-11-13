using System;
using System.Globalization;
using System.Windows.Data;

namespace DashboardFrontend.ValueConverters
{
    /// <summary>
    /// Static resource for converting strings to ints and vice versa
    /// </summary>
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!int.TryParse(value.ToString(), out int result))
            {
                result = 0;
            }
            return result;
        }
    }
}
