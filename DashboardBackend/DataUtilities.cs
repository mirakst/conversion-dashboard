using DashboardBackend.Database;
using DashboardBackend.Database.Models;
using Model;
using System.Data.SqlTypes;
using static Model.LogMessage;
using static Model.ValidationTest;


namespace DashboardBackend
{
    public static class DataUtilities
    {
        //The class that handles the state database queries. Default is SQL.
        public static IDatabaseHandler DatabaseHandler { get; set; }


        //Set SQL minimum DateTime as default.
        public static DateTime SqlMinDateTime { get; } = SqlDateTime.MinValue.Value;


        /// <summary>
        /// Queries the state database for executions newer than minDate, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of executions, matching the supplied constraints.</returns>
        public static List<Execution> GetExecutions(DateTime minDate)
        {

            List<ExecutionEntry> queryResult = DatabaseHandler.QueryExecutions(minDate);
            List<Execution> result = new();

            foreach (ExecutionEntry item in queryResult)
            {
                int executionId = (int)item.ExecutionId.Value;
                DateTime created = item.Created.Value;
                Execution newExecution = new(executionId, created);
                result.Add(newExecution);
            }

            return result;
        }

        /// <summary>
        /// Queries the state database for executions newer than SqlMinDateTime, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <returns>A list of all executions in the state database</returns>
        public static List<Execution> GetExecutions() => GetExecutions(SqlMinDateTime);


        /// <summary>
        /// Queries the state database for validation tests newer than minDate, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of validation tests, matching the supplied constraints.</returns>
        public static List<ValidationTest> GetAfstemninger(DateTime minDate)
        {
            List<AfstemningEntry> queryResult = DatabaseHandler.QueryAfstemninger(minDate);
            List<ValidationTest> result = new();

            foreach (AfstemningEntry test in queryResult)
            {
                DateTime afstemtDato = test.Afstemtdato;
                string description = test.Description;
                ValidationStatus status = GetValidationStatus(test);
                string manager = test.Manager;
                ValidationTest newTest = new(afstemtDato, description, status, manager);
                result.Add(newTest);
            }

            return result;
        }

        /// <summary>
        /// Queries the state database for validation tests newer than SqlMinDateTime, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <returns>A list of all validation tests in the state database</returns>
        public static List<ValidationTest> GetAfstemninger() => GetAfstemninger(SqlMinDateTime);


        /// <summary>
        /// Queries the state database for log messages from a specific execution, newer than minDate,
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="ExecutionId">An execution ID constraint for the objects in the returned list.</param>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of log messages, matching the supplied constraints.</returns>
        public static List<LogMessage> GetLogMessages(int ExecutionId, DateTime minDate)
        {
            if (ExecutionId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ExecutionId));
            }

            List<LoggingEntry> queryResult = DatabaseHandler.QueryLogMessages(ExecutionId, minDate);
            List<LogMessage> result = new();

            foreach (LoggingEntry item in queryResult)
            {
                string content = item.LogMessage;
                LogMessageType type = GetLogMessageType(item);
                int contextId = (int)item.ContextId.Value;
                DateTime created = item.Created.Value;
                LogMessage message = new(content, type, contextId, created);
                result.Add(message);
            }

            return result;
        }

        /// <summary>
        /// Queries the state database for log messages from a specific execution, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="ExecutionId">An execution ID constraint for the objects in the returned list.</param>
        /// <returns>A list of log messages, matching the supplied constraints.</returns>
        public static List<LogMessage> GetLogMessages(int ExecutionId) => GetLogMessages(ExecutionId, SqlMinDateTime);

        /// <summary>
        /// Queries the state database for log messages newer than minDate, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of log messages, matching the supplied constraints.</returns>
        public static List<LogMessage> GetLogMessages(DateTime minDate)
        {
            List<LoggingEntry> queryResult = DatabaseHandler.QueryLogMessages(minDate);
            List<LogMessage> result = new();

            foreach (LoggingEntry item in queryResult)
            {
                string content = item.LogMessage;
                LogMessageType type = GetLogMessageType(item);
                int contextId = (int)item.ContextId.Value;
                DateTime created = item.Created.Value;
                LogMessage message = new(content, type, contextId, created);
                result.Add(message);
            }

            return result;
        }


        /// <summary>
        /// Queries the state database for all log messages, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <returns>A list of all log messages from the state database</returns>
        public static List<LogMessage> GetLogMessages() => GetLogMessages(SqlMinDateTime);

        /// <summary>
        /// Queries the state database for all managers,
        /// then populates all executions with the managers that ran during their runtime.
        /// </summary>
        /// <param name="executions">A list of all executions from a conversion.</param>
        public static void AddManagers(List<Execution> executions)
        {
            List<Manager> managerList = GetManagers();

            foreach (var execution in executions)
            {
                List<Manager> executionManagers = managerList
                                                  .Where(e => e.ExecutionId > 0)
                                                  .Where(e => e.ExecutionId == execution.Id)
                                                  .Select(e => { e.Name = e.Name.ToLower(); return e; })
                                                  .ToList();

                execution.Managers = executionManagers;
                execution.SetUpDictionaries();
            }
        }

        /// <summary>
        /// Queries the state database for managers newer than minDate, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of Managers</returns>
        public static List<Manager> GetManagers(DateTime minDate)
        {
            List<LoggingContextEntry> queryResult = DatabaseHandler.QueryManagers();
            List<Manager> result = new();

            foreach (var item in queryResult)
            {
                int contextId = (int)item.ContextId;
                int executionId = (int)item.ExecutionId.Value;
                int index = item.Context.IndexOf(",");
                string mgrName = item.Context;
                if (index > 0)
                {
                    mgrName = item.Context.Substring(0, index);
                }
                Manager newManager = new(contextId, executionId, mgrName);
                result.Add(newManager);
            }

            return result;
        }

        /// <summary>
        /// Queries the state database for managers, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <returns>A list of all Managers from the state database</returns>
        public static List<Manager> GetManagers() => GetManagers(SqlMinDateTime);


        /// <summary>
        /// Queries the state database for health report performance entries, 
        /// and adds them to the health report.
        /// </summary>
        /// <param name="hr">The conversion's health report</param>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <exception cref="FormatException">Thrown if somehow the query failed and an unexpected entry is met.</exception>
        public static void AddHealthReportReadings(HealthReport hr, DateTime minDate)
        {
            List<HealthReportEntry> queryResult = DatabaseHandler.QueryPerformanceReadings(minDate);
            List<CpuLoad> cpuRes = new();
            List<RamUsage> ramRes = new();
            List<NetworkUsage> netRes = new();
            List<HealthReportEntry> cpuAndRamEntries = queryResult
                                                       .Where(e => e.ReportType != "NETWORK")
                                                       .ToList();
            List<HealthReportEntry> networkEntries = queryResult
                                                      .Where(e => e.ReportType == "NETWORK")
                                                      .ToList();

            foreach (HealthReportEntry item in cpuAndRamEntries)
            {
                switch (item.ReportType)
                {
                    case "CPU":
                        cpuRes.Add(GetCpuReading(item));
                        break;
                    case "MEMORY":
                        ramRes.Add(GetRamReading(item));
                        break;
                    default:
                        throw new FormatException(nameof(item));
                }
            }
            netRes = BuildNetworkUsage(networkEntries);

            hr.Cpu.Readings.AddRange(cpuRes);
            hr.Ram.Readings.AddRange(ramRes);
            hr.Network.Readings.AddRange(netRes);
        }

        /// <summary>
        /// Queries the state database for health report performance entries, 
        /// and adds them to the health report.
        /// </summary>
        /// <param name="hr">The conversion's health report</param>
        public static void AddHealthReportReadings(HealthReport hr) => AddHealthReportReadings(hr, SqlMinDateTime);

        /// <summary>
        /// Creates a list of CPU Readings for the system model, which is returned.
        /// </summary>
        /// <param name="item">A state database entry with cpu load readings</param>
        /// <returns>A CPU load reading.</returns>
        public static CpuLoad GetCpuReading(HealthReportEntry item)
        {
            int executionId = item.ExecutionId.Value;
            long reportNumValue = item.ReportNumericValue.Value;
            DateTime logTime = item.LogTime.Value;
            CpuLoad cpuReading = new(executionId, reportNumValue, logTime);
            return cpuReading;
        }

        /// <summary>
        /// Creates a list of RAM Readings for the system model, which is returned.
        /// </summary>
        /// <param name="item">A state database entry with cpu load readings</param>
        /// <returns>A RAM usage reading.</returns>
        public static RamUsage GetRamReading(HealthReportEntry item)
        {
            int executionId = (int)item.ExecutionId.Value;
            long reportNumValue = item.ReportNumericValue.Value;
            DateTime logTime = item.LogTime.Value;
            RamUsage ramReading = new(executionId, reportNumValue, logTime);
            return ramReading;
        }

        /// <summary>
        /// Returns the type of the log message parameter 'entry'.
        /// </summary>
        /// <param name="entry">A single entry from the [LOGGING] table in the state database.</param>
        /// <returns>A log message type besed on the enum in the log message class.</returns>
        /// <exception cref="ArgumentException">Thrown if the parameter passed is not a legal log message type.</exception>
        public static LogMessageType GetLogMessageType(LoggingEntry entry)
        {
            if (entry.LogMessage.StartsWith("Afstemning") || entry.LogMessage.StartsWith("Check -"))
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
                    throw new ArgumentException(entry.LogLevel +
                                                " is not a known log message type.");
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
                    throw new ArgumentException(nameof(entry) +
                                                " is not a known validation test result.");
            }
        }

        /// <summary>
        /// Builds the system model health report with CPU, Memory and Network, 
        /// by use of entries with report_type ending on 'INIT'.
        /// </summary>
        /// <param name="entries">A list of Health Report entries from the state database.</param>
        /// <returns>A Health Report initialized with system info.</returns>
        public static HealthReport BuildHealthReport()
        {
            List<HealthReportEntry> queryResult = DatabaseHandler.QueryHealthReport();
            HealthReport result;

            //INIT
            string HostName = queryResult.FindLast(e => e.ReportKey == "Hostname").ReportStringValue;
            string MonitorName = queryResult.FindLast(e => e.ReportKey == "Monitor Name").ReportStringValue;

            //CPU INIT
            string cpuName = queryResult.FindLast(e => e.ReportKey == "CPU Name").ReportStringValue;
            int cpuCores = (int)queryResult.FindLast(e => e.ReportKey == "PhysicalCores").ReportNumericValue;
            long cpuMaxFreq = (long)queryResult.FindLast(e => e.ReportKey == "CPU Max frequency").ReportNumericValue;
            Cpu cpu = new(cpuName, cpuCores, cpuMaxFreq);

            //MEMORY INIT
            long ramTotal = (long)queryResult.FindLast(e => e.ReportKey == "TOTAL").ReportNumericValue;
            Ram ram = new(ramTotal);

            //NETWORK INIT
            string networkName = queryResult.FindLast(e => e.ReportKey == "Interface 0: Name").ReportStringValue;
            string networkMacAddress = queryResult.FindLast(e => e.ReportKey == "Interface 0: MAC address").ReportStringValue;
            long networkSpeed = (long)queryResult.FindLast(e => e.ReportKey == "Interface 0: Speed").ReportNumericValue;
            Network network = new(networkName, networkMacAddress, networkSpeed);

            result = new HealthReport(HostName, MonitorName, cpu, network, ram);

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

            for (int i = 0; i < entryCount; i += 6)
            {
                distinctReports.Add(entries.Skip(i).Take(6).ToList());
            }

            //Build system model network usage objects.
            foreach (var item in distinctReports)
            {
                int executionId = item.First().ExecutionId.Value;
                DateTime logTime = item.First().LogTime.Value;
                //BYTES SEND
                long bytesSend = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Send").ReportNumericValue;
                long bytesSendDelta = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Send (Delta)").ReportNumericValue;
                long bytesSendSpeed = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Send (Speed)").ReportNumericValue;

                //BYTES RECEIVED
                long bytesReceived = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Received").ReportNumericValue;
                long bytesReceivedDelta = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Received (Delta)").ReportNumericValue;
                long bytesReceivedSpeed = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Received (Speed)").ReportNumericValue;

                NetworkUsage reading = new(executionId,
                                           bytesSend,
                                           bytesSendDelta,
                                           bytesSendSpeed,
                                           bytesReceived,
                                           bytesReceivedDelta,
                                           bytesReceivedSpeed,
                                           logTime);

                result.Add(reading);
            }

            return result;
        }

        /// <summary>
        /// Builds manager usage data by finding all entries connected with a specific manager,
        /// and storing the data from these entries in properties on the manager.
        /// </summary>
        /// <param name="manager">The manager to store the info on.</param>
        /// <param name="entries">A list of engine property entries from the state database.</param>
        public static void BuildManagerData(Manager manager, List<EnginePropertyEntry> entries)
        {
            List<EnginePropertyEntry> managerDataEntries = entries
                                                            .Where(e => 
                                                            {
                                                                string managerName = e.Manager;
                                                                int index = e.Manager.IndexOf(",");
                                                                if (index >= 0)
                                                                {
                                                                    managerName = e.Manager.Substring(0, index);
                                                                }
                                                                managerName = managerName.ToLower();
                                                                return managerName == manager.Name;
                                                            })
                                                            .ToList();

            manager.SetStartTime(managerDataEntries.FindLast(e => e.Key == "START_TIME").Value);
            manager.SetEndTime(managerDataEntries.FindLast(e => e.Key == "END_TIME").Value);
            manager.RowsRead = int.Parse(managerDataEntries.FindLast(e => e.Key == "Læste rækker").Value);
            manager.RowsWritten = int.Parse(managerDataEntries.FindLast(e => e.Key == "Skrevne rækker").Value);
        }

        /// <summary>
        /// Queries the state database for entries from the engine properties table,
        /// then populates all managers from the supplied execution with data connected to them.
        /// </summary>
        /// <param name="execution">The execution for which to gather manager data.</param>
        /// <param name="minDate">A DateTime constraint for the data gathered.</param>
        public static void AddManagerReadings(Execution execution, DateTime minDate)
        {
            List<EnginePropertyEntry> engineProperties = DatabaseHandler.QueryEngineProperties(minDate);
            foreach (var manager in execution.Managers)
            {
                BuildManagerData(manager, engineProperties);
            }
        }

        /// <summary>
        /// Queries the state database for entries from the engine properties table,
        /// then populates all managers from the supplied execution with data connected to them.
        /// </summary>
        /// <param name="execution">The execution for which to gather manager data.</param>
        public static void AddManagerReadings(Execution execution) => AddManagerReadings(execution, SqlMinDateTime);
    }
}
