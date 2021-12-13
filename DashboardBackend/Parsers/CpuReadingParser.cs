using DashboardBackend.Database.Models;
using Model;

namespace DashboardBackend.Parsers
{
    public class CpuReadingParser : IDataParser<HealthReportEntry, List<CpuLoad>>
    {
        public CpuReadingParser()
        {

        }

        public List<CpuLoad> Parse(List<HealthReportEntry> data)
        {
            List<CpuLoad> cpuReadings = new();
            foreach (var entry in data)
            {
                cpuReadings.Add(GetCpuReading(entry));
            }
            return cpuReadings;
        }

        /// <summary>
        /// Creates a list of CPU Readings for the system model, which is returned.
        /// </summary>
        /// <param name="item">A state database entry with cpu load readings</param>
        /// <returns>A CPU load reading.</returns>
        private static CpuLoad GetCpuReading(HealthReportEntry item)
        {
            int executionId = item.ExecutionId.Value;
            double reportNumValue = Convert.ToDouble(item.ReportNumericValue) / 100;
            DateTime logTime = item.LogTime.Value;
            CpuLoad cpuReading = new(executionId, reportNumValue, logTime);
            return cpuReading;
        }
    }
}
