using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DashboardFrontend.ValueConverters
{
    public class WindowStateToWingdingsConverter : IValueConverter
    {
        /// <summary>
        /// A converter for the maximize/restore window button.
        /// </summary>
        /// <returns>A character for the wingdings font, based on the state of the window.</returns>
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
