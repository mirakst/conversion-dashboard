using System;
using System.Globalization;
using System.Windows.Controls;

namespace DashboardFrontend.ValidationRules
{
    /// <summary>
    /// Validation rule checking whether the input is within range
    /// </summary>
    public class WithinRangeRule : ValidationRule
    {
        /// <summary>
        /// Checks if the input data is within range.
        /// </summary>
        /// <returns>A validation result based on the success of the validation test.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int input = int.Parse((string)value);

            return input > 0
                ? new ValidationResult(true, null)
                : new ValidationResult(false, "Input must be a positive integer");
        }
    }
}