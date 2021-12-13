using Model;
using System;
using System.Globalization;
using System.Windows.Data;
using static Model.ValidationTest;

namespace DashboardFrontend.ValueConverters
{
    public class ValidationStatusToImageConverter : IValueConverter
    {
        /// <summary>
        /// A converter for validation result icons.
        /// </summary>
        /// <returns>An icon based on the status of the validation test.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ValidationTest test)
            {
                switch (test.Status)
                {
                    case ValidationStatus.Ok:
                        return "/Icons/ValidationOk.png";
                    case ValidationStatus.Failed:
                    case ValidationStatus.FailMismatch:
                        return "/Icons/ValidationFailed.png";
                    case ValidationStatus.Disabled:
                        return "/Icons/ValidationDisabled.png";
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
