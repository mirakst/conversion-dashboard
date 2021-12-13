using System;
using System.Windows.Data;

namespace DashboardFrontend.ValueConverters
{
    public class FilterPopupHeightConverter : IValueConverter
    {
        /// <summary>
        /// A converter for the height of the context id list of the log filter popup.
        /// </summary>
        /// <returns>0 as minimum height, height - 120 (the summed height of the other elements) if larger than 0.</returns>
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
