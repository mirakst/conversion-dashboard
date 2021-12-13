using DashboardBackend.Database.Models;
using Model;

namespace DashboardBackend.Parsers
{
    public class NetworkReadingParser : IDataParser<HealthReportEntry, List<NetworkUsage>>
    {
        public List<NetworkUsage> Parse(List<HealthReportEntry> data)
        {
            List<NetworkUsage> networkReadings = new();
            var distinctSets = GetDistinctSets(data);
            foreach (var set in distinctSets)
            {
                networkReadings.Add(GetNetworkReading(set));
            }
            return networkReadings;
        }

        /// <summary>
        /// Builds network usage readings by coupling network entries 6 at a time.
        /// </summary>
        /// <param name="data">A list of network usage entries from the state database.</param>
        /// <returns>A coupled list of network usage entries.</returns>
        private List<List<HealthReportEntry>> GetDistinctSets(List<HealthReportEntry> data)
        {
            List<List<HealthReportEntry>> result = new();
            for (int i = 0; i < data.Count; i += 6)
            {
                result.Add(data.Skip(i).Take(6).ToList());
            }
            return result;
        }

        private NetworkUsage GetNetworkReading(List<HealthReportEntry> set)
        {
            int? execId = set.Find(i => i.ExecutionId.HasValue)?.ExecutionId.Value;
            DateTime? logTime = set.Find(i => i.LogTime.HasValue)?.LogTime.Value;
            if (!execId.HasValue)
            {
                throw new FormatException(nameof(execId));
            }
            if (!logTime.HasValue)
            {
                throw new FormatException(nameof(logTime));
            }
            long bytesSend = set.Find(e => e.ReportKey == "Interface 0: Bytes Send")?.ReportNumericValue ?? 0;
            long bytesSendDelta = set.Find(e => e.ReportKey == "Interface 0: Bytes Send (Delta)")?.ReportNumericValue ?? 0;
            long bytesSendSpeed = set.Find(e => e.ReportKey == "Interface 0: Bytes Send (Speed)")?.ReportNumericValue ?? 0;
            long bytesReceived = set.Find(e => e.ReportKey == "Interface 0: Bytes Received")?.ReportNumericValue ?? 0;
            long bytesReceivedDelta = set.Find(e => e.ReportKey == "Interface 0: Bytes Received (Delta)")?.ReportNumericValue ?? 0;
            long bytesReceivedSpeed = set.Find(e => e.ReportKey == "Interface 0: Bytes Received (Speed)")?.ReportNumericValue ?? 0;
            return new NetworkUsage(execId.Value, bytesSend, bytesSendDelta, bytesSendSpeed, bytesReceived, bytesReceivedDelta, bytesReceivedSpeed, logTime.Value);
        }
    }
}
