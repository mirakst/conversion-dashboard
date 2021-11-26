using Model;
using System;
using System.Globalization;
using System.Windows.Data;
using static Model.ValidationTest;

namespace DashboardFrontend.ValueConverters
{
    public class ValidationStatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ValidationTest test)
            {
                switch (test.Status)
                {
                    case ValidationStatus.Ok:
                        return "/Icons/Validation.png";
                    case ValidationStatus.Failed:
                    case ValidationStatus.FailMismatch:
                        return "/Icons/Fatal.png";
                    case ValidationStatus.Disabled:
                        return "/Icons/Warning.png";
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
