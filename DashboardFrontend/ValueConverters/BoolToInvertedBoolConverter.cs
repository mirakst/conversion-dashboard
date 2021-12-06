using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DashboardFrontend.ValueConverters
{
    public class BoolToInvertedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                bool boolValue = (bool)value;
                return !boolValue;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack() of BoolToInvertedBoolConverter is not implemented");
        }
    }
}
