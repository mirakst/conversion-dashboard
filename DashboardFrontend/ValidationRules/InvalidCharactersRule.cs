using System.Globalization;
using System.Windows.Controls;

namespace DashboardFrontend.ValidationRules
{
    /// <summary>
    /// Validation rule checking whether the input contains illegal characters
    /// </summary>
    class InvalidCharactersRule : ValidationRule
    {
        public string InvalidCharacters {  get; set; }

        /// <summary>
        /// Used to set illegal characters for an input.
        /// </summary>
        /// <returns>A validation result based on the contents of the input.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string inputStr = value.ToString();
            InvalidCharacters.ToCharArray();

            foreach (char c in InvalidCharacters)
            {
                if (inputStr.Contains(c)) return new ValidationResult(false, $"Input cannot contain '{c}'!");
            }

            return new ValidationResult(true, null);
        }
    }
}
