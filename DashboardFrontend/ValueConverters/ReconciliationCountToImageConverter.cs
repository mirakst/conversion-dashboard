using DashboardFrontend.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DashboardFrontend.ValueConverters
{
    public class ReconciliationCountToImageConverter : IValueConverter
    {
        /// <summary>
        /// A converter for Reconciliation icons.
        /// </summary>
        /// <returns>An icon based on whether a collection of Reconciliations is disabled, failed or OK.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ManagerObservable entry)
            {
                if (entry.FailedCount == 0 && entry.DisabledCount > 0)
                {
                    return "/Icons/ReconciliationDisabled.png";
                }
                else if (entry.FailedCount > 0)
                {
                    return "/Icons/ReconciliationFailed.png";
                }
                else
                {
                    return "/Icons/ReconciliationOk.png";
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
