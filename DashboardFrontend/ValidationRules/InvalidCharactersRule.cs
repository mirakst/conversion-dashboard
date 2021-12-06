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
