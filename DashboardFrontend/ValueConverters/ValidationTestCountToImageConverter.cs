using Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DashboardFrontend.ValueConverters
{
    public class ValidationTestCountToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Manager entry)
            {
                if (entry.ValidationsFailed == 0 && entry.ValidationsDisabled > 0)
                {
                    return "/Icons/ValidationDisabled.png";
                }
                else if (entry.ValidationsFailed > 0)
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
