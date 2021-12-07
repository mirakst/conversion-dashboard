using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DashboardFrontend.ValueConverters
{
    public class WindowStateToWingdingsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (WindowState)value switch
            {
                WindowState.Normal => 1,
                WindowState.Maximized => 2,
                _ => 1
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
