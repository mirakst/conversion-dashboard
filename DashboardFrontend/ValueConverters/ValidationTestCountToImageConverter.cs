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
            if (value is ValidationReportViewModel.ManagerObservable entry)
            {
                if (entry.FailedCount == 0 && entry.DisabledCount > 0)
                {
                    return "/Icons/Warning.png";
                }
                else if (entry.FailedCount > 0)
                {
                    return "/Icons/Fatal.png";
                }
                else
                {
                    return "/Icons/Validation.png";
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
