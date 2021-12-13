using DashboardBackend.Database.Models;
using Model;

namespace DashboardBackend.Parsers
{
    public class RamReadingParser : IDataParser<HealthReportEntry, List<RamLoad>>
    {
        public RamReadingParser()
        {

        }

        public List<RamLoad> Parse(List<HealthReportEntry> data)
        {
            List<RamLoad> ramReadings = new();
            foreach (var entry in data)
            {
                ramReadings.Add(GetRamReading(entry));
            }
            return ramReadings;
        }

        /// <summary>
        /// Creates a list of RAM Readings for the system model, which is returned.
        /// </summary>
        /// <param name="item">A state database entry with cpu load readings</param>
        /// <returns>A RAM usage reading.</returns>
        private static RamLoad GetRamReading(HealthReportEntry item)
        {
            int executionId = item.ExecutionId.Value;
            long available = item.ReportNumericValue.Value;
            DateTime logTime = item.LogTime.Value;
            RamLoad ramReading = new(executionId, available, logTime);
            return ramReading;
        }
    }
}
