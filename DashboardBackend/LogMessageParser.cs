using System.Globalization;
using System.Text.RegularExpressions;

namespace DashboardBackend
{
    public static class LogMessageParser
    {
        /// <summary>
        /// Scans the input log message and performs actions based on the interpretation of its content.
        /// </summary>
        /// <param name="message">The log message to parse</param>
        public static void Parse(string message)
        {
            // Indicates that a manager has begun its execution.
            if (message.StartsWith("Starting manager:"))
            {
                // "Starting manager: [MANAGER_NAME],[SOME_ID]" => "[MANAGER_NAME]"
                string managerName = message.Split(' ', ',')[2];
                Console.WriteLine($"[INFO][MANAGER STARTED][{managerName}]");
            }
            // Indicates that a Manager's number of rows to process has been calculated during its post-run script.
            else if (message.StartsWith("Total count:"))
            {
                string value = Regex.Match(message, @"\d+\.?\d*").Value;
                int rowsToProcess = int.Parse(value, NumberStyles.AllowThousands);
                Console.WriteLine($"[INFO][ROWS:{rowsToProcess}]");
            }
            // Indicates that a manager has finished its execution.
            else if (message.StartsWith("Manager execution done."))
            {
                Console.WriteLine("[INFO][MANAGER FINISHED]");
            }
            // Indicates that the log message should be classified as a validation (for filtering).
            else if (message.StartsWith("Check -") || message.Contains("Afstemning"))
            {
                Console.WriteLine("[VALIDATION]");
            }
            // Indicates that the message contains an SQL statement cost which may be relevant for calculating manager scores.
            else if (Regex.IsMatch(message, @"^\[.+\] SQL statement has a cost of [\d+.]+, and a total of [\d+.]+ full table scans.$"))
            {
                MatchCollection matches = Regex.Matches(message, @"(?<= )\d+\.?\d*");
                // Since the statement cost is printed in en-US formatting, it must be parsed as such.
                float statementCost = float.Parse(matches[0].Value, new CultureInfo("en-US"));
                int fullTableScans = int.Parse(matches[1].Value);
                Console.WriteLine($"[INFO][COST: {statementCost}][SCANS: {fullTableScans}]");
            }
        }
    }
}
