using Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DashboardFrontend.ValueConverters
{
    public class IsReconciliationBoolConverter : IValueConverter
    {

        /// <returns>True if value is a Reconciliation.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Reconciliation;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
