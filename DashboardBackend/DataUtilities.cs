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


        /// <summary>
        /// Queries the state database for executions, then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of executions, matching the supplied constraints.</returns>
        public static List<Execution> GetExecutions(DateTime minDate)
        {

            List<ExecutionEntry> queryResult = DatabaseHandler.QueryExecutions(minDate);

            List<Execution> result = new();

            foreach (ExecutionEntry item in queryResult)
            {
                Execution newExecution = new((int)item.ExecutionId, (DateTime)item.Created);
                result.Add(newExecution);
            }

            return result;
        }

        public static List<Execution> GetExecutions() => GetExecutions(SqlMinDateTime);


        /// <summary>
        /// Queries the state database for validation tests, then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of validation tests, matching the supplied constraints.</returns>
        public static List<ValidationTest> GetAfstemninger(DateTime minDate)
        {
            List<AfstemningEntry> queryResult = DatabaseHandler.QueryAfstemninger(minDate);

            List<ValidationTest> result = new();

            foreach (AfstemningEntry test in queryResult)
            {
                var newTest = new ValidationTest(test.Afstemtdato, test.Description, DataUtilities.GetValidationStatus(test), test.Manager);
                result.Add(newTest);
            }

            return result;
        }

        public static List<ValidationTest> GetAfstemninger() => GetAfstemninger(SqlMinDateTime);


        /// <summary>
        /// Queries the state database for log messages, then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="ExecutionId">An execution ID constraint for the objects in the returned list.</param>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of log messages, matching the supplied constraints.</returns>
        public static List<LogMessage> GetLogMessages(int ExecutionId, DateTime minDate)
        {
            List<LoggingEntry> queryResult;
            if (ExecutionId > 0)
            {
                queryResult = DatabaseHandler.QueryLogMessages(ExecutionId, minDate);
            } 
            else
            {
                queryResult = DatabaseHandler.QueryLogMessages(minDate);
            }

            List<LogMessage> result = new();

            foreach (LoggingEntry item in queryResult)
            {
                result.Add(new LogMessage(item.LogMessage, DataUtilities.GetLogMessageType(item), (int)item.ContextId, (DateTime)item.Created));
            }

            return result;
        }

        public static List<LogMessage> GetLogMessages(int ExecutionId) => GetLogMessages(ExecutionId, SqlMinDateTime);
        public static List<LogMessage> GetLogMessages(DateTime minDate) => GetLogMessages(0, minDate);
        public static List<LogMessage> GetLogMessages() => GetLogMessages(0, SqlMinDateTime);


        /// <summary>
        /// Queries the state database for managers, then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of Managers</returns>
        public static List<Manager> GetManagers(DateTime minDate)
        {
            List<ManagerEntry> queryResult = DatabaseHandler.QueryManagers();  

            List<Manager> result = new();

            foreach (var item in queryResult)
            {
                result.Add(new Manager((int)item.RowId, (int)item.ExecutionsId, item.ManagerName));
            }

            return result;
        }

        public static List<Manager> GetManagers() => GetManagers(SqlMinDateTime);


        /// <summary>
        /// Queries the state database for health report INIT entries, then creates a Health Report with system info for the system model, which is returned
        /// </summary>
        /// <returns>A Health Report initialized with system info.</returns>
        public static HealthReport GetHealthReport()
        {
            List<HealthReportEntry> queryResult = DatabaseHandler.QueryHealthReport();

            HealthReport result = BuildHealthReport(queryResult);

            return result;
        }


        /// <summary>
        /// Queries the state database for health report NETWORK entries, then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of Network usage readings.</returns>
        public static List<NetworkUsage> GetNetworkReadings(DateTime minDate)
        {
            List<HealthReportEntry> queryResult = DatabaseHandler.QueryNetworkReadings(minDate);

            List<NetworkUsage> result = BuildNetworkUsage(queryResult);

            return result;
        }

        public static List<NetworkUsage> GetNetworkReadings() => GetNetworkReadings(SqlMinDateTime);


        /// <summary>
        /// Queries the state database for health report CPU entries, then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of CPU load readings.</returns>
        public static List<CpuLoad> GetCpuReadings(DateTime minDate)
        {
            List<HealthReportEntry> queryResult = DatabaseHandler.QueryCpuReadings(minDate);

            List<CpuLoad> result = new();

            foreach (var item in queryResult)
            {
                result.Add(new CpuLoad((int)item.ExecutionId, (long)item.ReportNumericValue, (DateTime)item.LogTime));
            }

            return result;
        }

        public static List<CpuLoad> GetCpuReadings() => GetCpuReadings(SqlMinDateTime);


        /// <summary>
        /// Queries the state database for executions, then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of ram usage readings.</returns>
        public static List<RamUsage> GetRamReadings(DateTime minDate)
        {
            List<HealthReportEntry> queryResult = DatabaseHandler.QueryRamReadings(minDate);
            List<RamUsage> result = new();

            foreach (var item in queryResult)
            {
                result.Add(new RamUsage((int)item.ExecutionId, (long)item.ReportNumericValue, (DateTime)item.LogTime));
            }

            return result;
        }

        public static List<RamUsage> GetRamReadings() => GetRamReadings(SqlMinDateTime);


        /// <summary>
        /// Returns the type of the log message parameter 'entry'.
        /// </summary>
        /// <param name="entry">A single entry from the [LOGGING] table in the state database.</param>
        /// <returns>A log message type besed on the enum in the log message class.</returns>
        /// <exception cref="ArgumentException">Thrown if the parameter passed is not a legal log message type.</exception>
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

        /// <summary>
        /// Returns the status of the validation test parameter 'entry'.
        /// </summary>
        /// <param name="entry">A single entry from the [AFSTEMNING] table in the state database.</param>
        /// <returns>A validation status based on the enum in the validation class.</returns>
        /// <exception cref="ArgumentException">Thrown if the parameter passed is not a legal validation status.</exception>
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

        /// <summary>
        /// Builds the system model health report with CPU, Memory and Network, by use of entries with report_type ending on 'INIT'.
        /// </summary>
        /// <param name="entries">A list of Health Report entries from the state database.</param>
        /// <returns>A Health Report initialized with system info.</returns>
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

        /// <summary>
        /// Builds network usage readings by coupling network entries 6 at a time.
        /// </summary>
        /// <param name="entries">A list of network usage entries from the state database.</param>
        /// <returns>A coupled list of network usage entries.</returns>
        public static List<NetworkUsage> BuildNetworkUsage(List<HealthReportEntry> entries)
        {
            List<NetworkUsage> result = new();
            List<List<HealthReportEntry>> distinctReports = new();

            int entryCount = entries.Count;

            for (int i = 0; i < entries.Count; i+= 6)
            {
                distinctReports.Add(entries.Skip(i).Take(6).ToList());
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
