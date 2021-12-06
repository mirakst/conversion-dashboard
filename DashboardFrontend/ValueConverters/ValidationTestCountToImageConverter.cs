using DashboardFrontend.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DashboardFrontend.ValueConverters
{
    public class ValidationTestCountToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ManagerObservable entry)
            {
                if (entry.FailedCount == 0 && entry.DisabledCount > 0)
                {
                    return "/Icons/ValidationDisabled.png";
                }
                else if (entry.FailedCount > 0)
                {
                    return "/Icons/ValidationFailed.png";
                }
                else
                {
                    return "/Icons/ValidationOk.png";
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
