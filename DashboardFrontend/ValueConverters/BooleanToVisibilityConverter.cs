using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DashboardFrontend.ValueConverters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// A converter for the visibility of UI elements.
        /// </summary>
        /// <returns>Visible if value is true, otherwise false.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
