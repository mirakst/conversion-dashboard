using System;
using System.Globalization;
using System.Windows.Controls;

namespace DashboardFrontend.ValidationRules
{
    /// <summary>
    /// Validation rule checking whether the input is of the correct data type
    /// </summary>
    public class ValidTypeRule : ValidationRule
    {
        public string ValidDataType { get; set; }

        /// <summary>
        /// Checks if the input data type matches the valid data types.
        /// </summary>
        /// <returns>A validation result based on the success of the validation test.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string inputStr = value.ToString();

            switch (ValidDataType) //Add cases for the allowed datatypes when needed
            {
                case "Int16":
                    bool result = int.TryParse(inputStr, out int intVal);
                    return result ? new ValidationResult(true, null) : new ValidationResult(false, $"Input must be of type: Int16");
            }
            throw new ArgumentException("The specified data type is either not implemented or it is spelled incorrectly in the assignment");
        }
    }
}
