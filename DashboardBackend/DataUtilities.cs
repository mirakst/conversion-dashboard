using System.Data.SqlTypes;
using DashboardBackend.Database.Models;
using DashboardBackend.Database;
using Model;
using static Model.LogMessage;
using static Model.ValidationTest;


namespace DashboardBackend
{
    public static class DataUtilities
    {
        //What is the use of this right now?
        public static bool HealthReportConfigured { get; private set; } = false;

        //The class that handles the state database queries. Default is SQL.
        public static IDatabaseHandler DatabaseHandler { get; set; }

        //Set SQL minimum DateTime as default.
        public static DateTime SqlMinDateTime { get; } = SqlDateTime.MinValue.Value;

        //Queries the state database for executions, then creates a list of them for the system model, which is returned.
        public static List<Execution> GetExecutions(DateTime minDate = default(DateTime))
        {
            minDate = minDate == default(DateTime) ? SqlMinDateTime : minDate;

            List<ExecutionEntry> queryResult = DatabaseHandler.QueryExecutions(minDate);

            List<Execution> result = new();

            foreach (ExecutionEntry item in queryResult)
            {
                Execution newExecution = new((int)item.ExecutionId, (DateTime)item.Created);
                result.Add(newExecution);
            }

            return result;
        }

        //Queries the state database for validation tests, then creates a list of them for the system model, which is returned.

        public static List<ValidationTest> GetAfstemninger(DateTime minDate = default(DateTime))
        {
            minDate = minDate == default(DateTime) ? SqlMinDateTime : minDate;

            List<AfstemningEntry> queryResult = DatabaseHandler.QueryAfstemninger(minDate);

            List<ValidationTest> result = new();

            foreach (AfstemningEntry test in queryResult)
            {
                var newTest = new ValidationTest(test.Afstemtdato, test.Description, DataUtilities.GetValidationStatus(test), test.Manager);
                result.Add(newTest);
            }

            return result;
        }

        //Queries the state database for log messages, then creates a list of them for the system model, which is returned.
        public static List<LogMessage> GetLogMessages(int ExecutionId, DateTime minDate = default(DateTime))
        {
            minDate = minDate == default(DateTime) ? SqlMinDateTime : minDate;

            List<LoggingEntry> queryResult = DatabaseHandler.QueryLogMessages(ExecutionId, minDate);

            List<LogMessage> result = new();

            foreach (LoggingEntry item in queryResult)
            {
                result.Add(new LogMessage(item.LogMessage, DataUtilities.GetLogMessageType(item), (int)item.ContextId, (DateTime)item.Created));
            }

            return result;
        }

        //Queries the state database for managers, then creates a list of them for the system model, which is returned.
        public static List<Manager> GetManagers(DateTime minDate = default(DateTime))
        {
            minDate = minDate == default(DateTime) ? SqlMinDateTime : minDate;

            List<ManagerEntry> queryResult = DatabaseHandler.QueryManagers();  

            List<Manager> result = new();

            foreach (var item in queryResult)
            {
                result.Add(new Manager((int)item.RowId, (int)item.ExecutionsId, item.ManagerName));
            }

            return result;
        }

        //Queries the state database for health report INIT entries, then creates a Health Report with system info for the system model, which is returned
        public static HealthReport GetHealthReport()
        {
            List<HealthReportEntry> queryResult = DatabaseHandler.QueryHealthReport();

            HealthReport result = BuildHealthReport(queryResult);

            return result;
        }

        //Queries the state database for health report NETWORK entries, then creates a list of them for the system model, which is returned.
        public static List<NetworkUsage> GetNetworkReadings(DateTime minDate = default(DateTime))
        {
            minDate = minDate == default(DateTime) ? SqlMinDateTime : minDate;

            List<HealthReportEntry> queryResult = DatabaseHandler.QueryNetworkReadings(minDate);

            List<NetworkUsage> result = BuildNetworkUsage(queryResult);

            return result;
        }

        //Queries the state database for health report CPU entries, then creates a list of them for the system model, which is returned.
        public static List<CpuLoad> GetCpuReadings(DateTime minDate = default(DateTime))
        {
            minDate = minDate == default(DateTime) ? SqlMinDateTime : minDate;

            List<HealthReportEntry> queryResult = DatabaseHandler.QueryCpuReadings(minDate);

            List<CpuLoad> result = new();

            foreach (var item in queryResult)
            {
                result.Add(new CpuLoad((int)item.ExecutionId, (long)item.ReportNumericValue, (DateTime)item.LogTime));
            }

            return result;
        }

        //Queries the state database for executions, then creates a list of them for the system model, which is returned.
        public static List<RamUsage> GetRamReadings(DateTime minDate = default(DateTime))
        {
            minDate = minDate == default(DateTime) ? SqlMinDateTime : minDate;

            List<HealthReportEntry> queryResult = DatabaseHandler.QueryRamReadings(minDate);
            List<RamUsage> result = new();

            foreach (var item in queryResult)
            {
                result.Add(new RamUsage((int)item.ExecutionId, (long)item.ReportNumericValue, (DateTime)item.LogTime));
            }

            return result;
        }

        //Returns the type of the log message parameter 'entry'
        public static LogMessageType GetLogMessageType(LoggingEntry entry)
        {
            if (entry.LogMessage.StartsWith("Afstemning"))
                return LogMessageType.VALIDATION;

            switch (entry.LogLevel)
            {
                case "INFO":
                    return LogMessageType.INFO;
                case "WARN":
                    return LogMessageType.WARNING;
                case "ERROR":
                    return LogMessageType.ERROR;
                case "FATAL":
                    return LogMessageType.FATAL;
                default:
                    throw new ArgumentException(entry.LogLevel + " is not a known log message type.");
            }
        }

        //Returns the status of the validation test parameter 'entry'
        public static ValidationStatus GetValidationStatus(AfstemningEntry entry)
        {
            switch (entry.Afstemresultat)
            {
                case "OK":
                    return ValidationStatus.OK;
                case "DISABLED":
                    return ValidationStatus.DISABLED;
                case "FAILED":
                    return ValidationStatus.FAILED;
                case "FAIL MISMATCH":
                    return ValidationStatus.FAIL_MISMATCH;
                default:
                    throw new ArgumentException(nameof(entry) + " is not a known validation test result.");
            }
        }

        //Builds the system model health report with CPU, Memory and Network, by use of entries with report_type ending on 'INIT'
        public static HealthReport BuildHealthReport(List<HealthReportEntry> entries)
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

        //Builds network usage readings. This is a big, complicated function due to the state database network logging
        //6 entries at once for the same update, sometimes with a tiny delay inbetween some of the entries.
        public static List<NetworkUsage> BuildNetworkUsage(List<HealthReportEntry> entries)
        {
            List<NetworkUsage> result = new();

            //List of distinct network logging timestamps.
            var logTimes = entries.Select(e => (DateTime)e.LogTime).Distinct().ToList();

            List<List<HealthReportEntry>> distinctReports = new();
            DateTime prevLogTime = new();

            //If logTime is less than 1 second from previous logTime, merge entries from the two separate logTimes.
            foreach (var item in logTimes)
            {
                if (item.Subtract(prevLogTime).Duration() < TimeSpan.FromSeconds(1))
                {
                    List<HealthReportEntry> entryList = entries.Where(e => e.LogTime >= prevLogTime)
                                                               .Where(e => e.LogTime < prevLogTime.AddSeconds(1)).ToList();

                    distinctReports.RemoveAt(distinctReports.Count - 1);
                    distinctReports.Add(entryList);
                }
                else
                {
                    distinctReports.Add(entries.FindAll(e => e.LogTime == (DateTime)item));
                }
                prevLogTime = item;
            }

            //Build system model network usage objects.
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
