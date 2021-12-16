using Model;
using System;
using System.Globalization;
using System.Windows.Data;
using static Model.Reconciliation;

namespace DashboardFrontend.ValueConverters
{
    public class ReconciliationStatusToImageConverter : IValueConverter
    {
        /// <summary>
        /// A converter for Reconciliation icons.
        /// </summary>
        /// <returns>An icon based on the status of the Reconciliation.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Reconciliation test)
            {
                switch (test.Status)
                {
                    case ReconciliationStatus.Ok:
                        return "/Icons/ReconciliationOk.png";
                    case ReconciliationStatus.Failed:
                    case ReconciliationStatus.FailMismatch:
                        return "/Icons/ReconciliationFailed.png";
                    case ReconciliationStatus.Disabled:
                        return "/Icons/ReconciliationDisabled.png";
                    default:
                        break;
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
