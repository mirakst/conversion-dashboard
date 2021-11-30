using DashboardFrontend.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DashboardFrontend.ValueConverters
{
    public class NotFoundToNaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int rowNum = (int)value;
            return rowNum < 0 ? "N/A" : rowNum;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}