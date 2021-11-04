using DashboardBackend.Database.Models;
using Model;

namespace DashboardBackend
{
    public static class DataConversion
    {
        public static bool HealthReportConfigured { get; private set; } = false;
        public static LogMessage.LogMessageType GetLogMessageType(LoggingEntry entry)
        {
            if (entry.LogLevel.StartsWith("Afstemning"))
                return LogMessage.LogMessageType.VALIDATION;

            switch (entry.LogLevel)
            {
                case "INFO":
                    return LogMessage.LogMessageType.INFO;
                case "WARN":
                    return LogMessage.LogMessageType.WARNING;
                case "ERROR":
                    return LogMessage.LogMessageType.ERROR;
                case "FATAL":
                    return LogMessage.LogMessageType.FATAL;
                default:
                    throw new ArgumentException(entry.LogLevel + " is not a known log message type.");
            }
        }

        public static ValidationTest.ValidationStatus GetValidationStatus(AfstemningEntry entry)
        {
            switch (entry.Afstemresultat)
            {
                case "OK":
                    return ValidationTest.ValidationStatus.OK;
                case "DISABLED":
                    return ValidationTest.ValidationStatus.DISABLED;
                case "FAILED":
                    return ValidationTest.ValidationStatus.FAILED;
                case "FAIL MISMATCH":
                    return ValidationTest.ValidationStatus.FAIL_MISMATCH;
                default:
                    throw new ArgumentException(nameof(entry) + " is not a known validation test result.");
            }
        }

        public static HealthReport InitHealthReport(List<HealthReportEntry> entries)
        {
            HealthReport result;

            //INIT
            string HostName = entries.FindLast(e => e.ReportKey == "Hostname").ReportStringValue;
            string MonitorName = entries.FindLast(e => e.ReportKey == "Monitor Name").ReportStringValue;

            //CPU INIT
            string cpuName = entries.FindLast(e => e.ReportKey == "CPU Name").ReportStringValue;
            int cpuCores = (int)entries.FindLast(e => e.ReportKey == "PhysicalCores").ReportNumericValue;
            long cpuMaxFreq = (long)entries.FindLast(e => e.ReportKey == "CPU Max frequency").ReportNumericValue;
            Cpu cpu = new(cpuName, cpuCores, cpuMaxFreq);

            //MEMORY INIT
            long ramTotal = (long)entries.FindLast(e => e.ReportKey == "TOTAL").ReportNumericValue;
            Ram ram = new(ramTotal);

            //NETWORK INIT
            string networkName = entries.FindLast(e => e.ReportKey == "Interface 0: Name").ReportStringValue;
            string networkMacAddress = entries.FindLast(e => e.ReportKey == "Interface 0: MAC address").ReportStringValue;
            long networkSpeed = (long)entries.FindLast(e => e.ReportKey == "Interface 0: Speed").ReportNumericValue;
            Network network = new(networkName, networkMacAddress, networkSpeed);

            result = new HealthReport(HostName, MonitorName, cpu, network, ram);

            HealthReportConfigured = true;
            return result;
        }

        public static List<NetworkUsage> BuildNetworkUsage(List<HealthReportEntry> entries)
        {
            List<NetworkUsage> result = new();

            var logTimes = entries.Select(e => e.LogTime).Distinct().ToList();

            List<List<HealthReportEntry>> distinctReports = new();
            DateTime lastReadingTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

            foreach (var item in logTimes)
            {
                if (item.Value.Subtract(lastReadingTime).Duration() < TimeSpan.FromSeconds(1))
                {
                    List<HealthReportEntry> entryList = entries.FindAll(e => e.LogTime.Value.Subtract(lastReadingTime).Duration() < TimeSpan.FromSeconds(1) && e.LogTime.Value.Subtract(lastReadingTime).Duration() > TimeSpan.FromSeconds(0));
                    distinctReports.Add(distinctReports.Last().Concat(entryList).ToList());
                    distinctReports.RemoveAt(distinctReports.Count - 2);
                }
                else
                {
                    distinctReports.Add(entries.FindAll(e => e.LogTime == (DateTime)item));
                }
                lastReadingTime = item.Value;
            }

            foreach (var item in distinctReports)
            {
                //BYTES SEND
                long bytesSend = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Send").ReportNumericValue;
                long bytesSendDelta = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Send (Delta)").ReportNumericValue;
                long bytesSendSpeed = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Send (Speed)").ReportNumericValue;

                //BYTES RECEIVED
                long bytesReceived = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Received").ReportNumericValue;
                long bytesReceivedDelta = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Received (Delta)").ReportNumericValue;
                long bytesReceivedSpeed = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Received (Speed)").ReportNumericValue;

                result.Add(new NetworkUsage((int)item.First().ExecutionId, bytesSend, bytesSendDelta, bytesSendSpeed, bytesReceived, 
                                            bytesReceivedDelta, bytesReceivedSpeed, (DateTime)item.First().LogTime));
            }

            return result;
        }

    }
}
