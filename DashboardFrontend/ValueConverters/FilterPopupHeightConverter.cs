using System;
using System.Windows.Data;

namespace DashboardFrontend.ValueConverters
{
    public class FilterPopupHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double distance)
            {
                return distance - 120 > 0 ? distance - 120 : 0;
            }
            else return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack() of BoolToInvertedBoolConverter is not implemented");
        }
    }
}
