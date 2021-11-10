using System.Globalization;
using System.Text.RegularExpressions;

namespace DashboardBackend
{
    public static class LogMessageParser
    {
        public static void Parse(string message)
        {
            if (message.StartsWith("Starting manager:"))
            {
                // "Starting manager: [MANAGER_NAME],[SOME_ID]" => "[MANAGER_NAME]"
                string managerName = message.Split(' ', ',')[2];
                Console.WriteLine($"[INFO][MANAGER STARTED][{managerName}]");
            }
            else if (message.StartsWith("Total count:"))
            {
                string value = Regex.Match(message, @"\d+").Value;
                int rowsToProcess = int.Parse(value);
                Console.WriteLine($"[INFO][ROWS:{rowsToProcess}]");
            }
            else if (message.StartsWith("Manager execution done."))
            {
                Console.WriteLine("[INFO][MANAGER FINISHED]");
            }
            else if (message.StartsWith("Check -") || message.Contains("Afstemning"))
            {
                Console.WriteLine("[VALIDATION]");
            }
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
