using DashboardBackend.Database;
using DashboardBackend.Database.Models;
using Model;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using static Model.LogMessage;
using static Model.ValidationTest;
using static Model.Manager;
using System.Diagnostics;

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

            return (from item in queryResult
                    let executionId = (int)item.ExecutionId.Value
                    let created = item.Created.Value
                    select new Execution(executionId, created))
                    .ToList();
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

            return (from item in queryResult
                    let date = item.Afstemtdato
                    let name = item.Description
                    let managerName = item.Manager
                    let status = GetValidationStatus(item)
                    let srcCount = item.Srcantal
                    let dstCount = item.Dstantal
                    let toolkitId = item.ToolkitId
                    let srcSql = item.SrcSql
                    let dstSql = item.DstSql
                    select new ValidationTest(date, name, status, managerName, srcCount, dstCount, toolkitId, srcSql, dstSql))
                    .ToList();
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
        /// <param name="executionId">An execution ID constraint for the objects in the returned list.</param>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of log messages, matching the supplied constraints.</returns>
        public static List<LogMessage> GetLogMessages(int executionId, DateTime minDate)
        {
            if (executionId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(executionId));
            }

            List<LoggingEntry> queryResult = DatabaseHandler.QueryLogMessages(executionId, minDate);

            return (from item in queryResult
                    let content = item.LogMessage
                    let type = GetLogMessageType(item, content)
                    let contextId = (int)item.ContextId.Value
                    let msgExecutionId = (int)item.ExecutionId.Value
                    let created = item.Created.Value
                    select new LogMessage(content, type, contextId, msgExecutionId, created))
                    .ToList();
        }

        /// <summary>
        /// Queries the state database for log messages from a specific execution, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="executionId">An execution ID constraint for the objects in the returned list.</param>
        /// <returns>A list of log messages, matching the supplied constraints.</returns>
        public static List<LogMessage> GetLogMessages(int executionId) => GetLogMessages(executionId, SqlMinDateTime);

        /// <summary>
        /// Queries the state database for log messages newer than minDate, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of log messages, matching the supplied constraints.</returns>
        public static List<LogMessage> GetLogMessages(DateTime minDate)
        {
            List<LoggingEntry> queryResult = DatabaseHandler.QueryLogMessages(minDate);

            return (from item in queryResult
                    let content = Regex.Replace(item.LogMessage, @"\u001b\[\d*;?\d+m", "")
                    let type = GetLogMessageType(item, content)
                    let contextId = (int)item.ContextId.Value
                    let executionId = (int)item.ExecutionId.Value
                    let created = item.Created.Value
                    select new LogMessage(content, type, contextId, executionId, created))
                    .ToList();
        }


        /// <summary>
        /// Queries the state database for all log messages, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <returns>A list of all log messages from the state database</returns>
        public static List<LogMessage> GetLogMessages() => GetLogMessages(SqlMinDateTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allManagers"></param>
        /// <returns></returns>
        public static Dictionary<int, List<Manager>> GetManagers(List<Manager> allManagers)
        {
            // return a dictionary where the keys are execution ID's and the values are lists of managers in each execution
            Dictionary<int, List<Manager>> executionIdManagerDict = new();

            // get list of all managers in all executions (may contain duplicates)
            List<LoggingContextEntry> loggingContextEntries = DatabaseHandler.QueryManagers();
            foreach(var entry in loggingContextEntries)
            {
                int contextId = (int)entry.ContextId;
                int executionId = (int)entry.ExecutionId.Value;
                string mgrName = entry.Context.Split(',')[0];

                // ensure that managers are only created once (avoid duplicates for each execution), but they must be added to each execution
                Manager manager = allManagers.Find(m => m.ContextId == contextId && m.Name == mgrName);
                if (manager == null)
                {
                    manager = new(contextId, executionId, mgrName);
                    allManagers.Add(manager);
                }
                else
                {
                    manager.ExecutionId = executionId;
                }

                if (!executionIdManagerDict.ContainsKey(executionId))
                {
                    executionIdManagerDict.Add(executionId, new());
                }
                if (!executionIdManagerDict[executionId].Contains(manager))
                {
                    executionIdManagerDict[executionId].Add(manager);
                }
            }
            // at this point we have a dictionary which maps lists of managers to execution ID's

            // Get additional details (status, rows read/written, start/end time) from Manager_Tracking table and add them to the managers that have already been created
            List<ManagerTracking> queryResult = DatabaseHandler.QueryManagerTracking();
            foreach (int executionId in executionIdManagerDict.Keys)
            {
                AddTrackingDataToManagers(executionIdManagerDict[executionId], queryResult);
            }

            return executionIdManagerDict;
        }

        // If the manager does not have data in the Manager_Tracking database, nothing is added to it
        public static void AddTrackingDataToManagers(List<Manager> managers, List<ManagerTracking> trackingInfo)
        {
            foreach(var item in trackingInfo)
            {
                string newManagerName = item.Mgr.Split(',')[0];
                Manager manager = managers.Find(m => m.Name == newManagerName.ToUpper());
                if (manager is not null && !manager.HasReadAllDataFromLog)
                {
                    manager.Name = newManagerName;
                    manager.Status = GetManagerStatus(item);
                    manager.RowsRead = item.Performancecountrowsread;
                    manager.RowsWritten = item.Performancecountrowswritten;
                    manager.StartTime = item.Starttime;
                    manager.EndTime = item.Endtime;
                    manager.Runtime = null;
                    if (manager.StartTime.HasValue && manager.EndTime.HasValue)
                    {
                        manager.Runtime = manager.EndTime.Value.Subtract(manager.StartTime.Value);
                    }
                }
            }
        }

        private static ManagerStatus GetManagerStatus(ManagerTracking manager)
        {
            return manager.Status switch
            {
                "OK" => ManagerStatus.Ok,
                "READY" => ManagerStatus.Ready,
                _ => ManagerStatus.Ready,
            };
        }


        /// <summary>
        /// Queries the state database for health report performance entries, 
        /// and adds them to the health report.
        /// </summary>
        /// <param name="hr">The conversion's health report</param>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <exception cref="FormatException">Thrown if somehow the query failed and an unexpected entry is met.</exception>
        public static void AddHealthReportReadings(HealthReport hr, DateTime minDate)
        {
            hr.LastModified = DateTime.Now;
            List<HealthReportEntry> queryResult = DatabaseHandler.QueryPerformanceReadings(minDate);
            List<CpuLoad> cpuRes = new();
            List<RamLoad> ramRes = new();
            List<NetworkUsage> netRes;
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
                        ramRes.Add(GetRamReading(hr.Ram.Total, item));
                        break;
                    default:
                        throw new FormatException(nameof(item));
                }
            }
            netRes = BuildNetworkUsage(networkEntries);

            hr.Cpu.Readings = cpuRes;
            hr.Ram.Readings = ramRes;
            hr.Network.Readings = netRes;
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
            double reportNumValue = Convert.ToDouble(item.ReportNumericValue) / 100;
            DateTime logTime = item.LogTime.Value;
            CpuLoad cpuReading = new(executionId, reportNumValue, logTime);
            return cpuReading;
        }

        /// <summary>
        /// Creates a list of RAM Readings for the system model, which is returned.
        /// </summary>
        /// <param name="item">A state database entry with cpu load readings</param>
        /// <returns>A RAM usage reading.</returns>
        public static RamLoad GetRamReading(long? totalRam, HealthReportEntry item)
        {
            int executionId = (int)item.ExecutionId.Value;
            long reportNumValue = item.ReportNumericValue.Value;
            double load = 1 - Convert.ToDouble(reportNumValue) / Convert.ToDouble(totalRam);
            long available = item.ReportNumericValue.Value;
            DateTime logTime = item.LogTime.Value;
            RamLoad ramReading = new(executionId, load, available, logTime);
            return ramReading;
        }

        /// <summary>
        /// Returns the type of the log message parameter 'entry'.
        /// </summary>
        /// <param name="entry">A single entry from the [LOGGING] table in the state database.</param>
        /// <returns>A log message type besed on the enum in the log message class.</returns>
        /// <exception cref="ArgumentException">Thrown if the parameter passed is not a legal log message type.</exception>
        public static LogMessageType GetLogMessageType(LoggingEntry entry, string content)
        {
            LogMessageType type;
            switch (entry.LogLevel)
            {
                case "INFO":
                    type = LogMessageType.Info;
                    break;
                case "WARN":
                    type = LogMessageType.Warning;
                    break;
                case "ERROR":
                    type = LogMessageType.Error;
                    break;
                case "FATAL":
                    type = LogMessageType.Fatal;
                    break;
                default:
                    type = LogMessageType.None;
                    break;
            }

            if (content.StartsWith("Afstemning") || content.StartsWith("Check -"))
            {
                if (type.HasFlag(LogMessageType.Error))
                {
                    type |= LogMessageType.Validation;
                }
                else
                {
                    type = LogMessageType.Validation;
                }
            }

            return type;
        }

        /// <summary>
        /// Returns the status of the validation test parameter 'entry'.
        /// </summary>
        /// <param name="entry">A single entry from the [AFSTEMNING] table in the state database.</param>
        /// <returns>A validation status based on the enum in the validation class.</returns>
        /// <exception cref="ArgumentException">Thrown if the parameter passed is not a legal validation status.</exception>
        public static ValidationStatus GetValidationStatus(AfstemningEntry entry)
        {
            return entry.Afstemresultat switch
            {
                "OK" => ValidationStatus.Ok,
                "DISABLED" => ValidationStatus.Disabled,
                "FAILED" => ValidationStatus.Failed,
                "FAIL MISMATCH" => ValidationStatus.FailMismatch,
                _ => throw new ArgumentException(nameof(entry) + " is not a known validation test result.")
            };
        }

        /// <summary>
        /// Builds the system model health report with CPU, Memory and Network, 
        /// by use of entries with report_type ending on 'INIT'.
        /// </summary>
        /// <param name="entries">A list of Health Report entries from the state database.</param>
        /// <returns>A Health Report initialized with system info.</returns>
        public static void BuildHealthReport(HealthReport hr)
        {
            List<HealthReportEntry> queryResult = DatabaseHandler.QueryHealthReport();

            //INIT
            string hostName = queryResult.FindLast(e => e.ReportKey == "Hostname")?.ReportStringValue;
            string monitorName = queryResult.FindLast(e => e.ReportKey == "Monitor Name")?.ReportStringValue;

            //CPU INIT
            string cpuName = queryResult.FindLast(e => e.ReportKey == "CPU Name")?.ReportStringValue;
            int? cpuCores = (int?) queryResult.FindLast(e => e.ReportKey == "PhysicalCores")?.ReportNumericValue;
            long? cpuMaxFreq = queryResult.FindLast(e => e.ReportKey == "CPU Max frequency")?.ReportNumericValue;

            Cpu cpu = new(cpuName, cpuCores, cpuMaxFreq);

            //MEMORY INIT
            long? ramTotal = queryResult.FindLast(e => e.ReportKey == "TOTAL")?.ReportNumericValue;
            Ram ram = new(ramTotal);

            //NETWORK INIT
            string networkName = queryResult.FindLast(e => e.ReportKey == "Interface 0: Name")?.ReportStringValue;
            string networkMacAddress = queryResult.FindLast(e => e.ReportKey == "Interface 0: MAC address")?.ReportStringValue;
            long? networkSpeed = queryResult.FindLast(e => e.ReportKey == "Interface 0: Speed")?.ReportNumericValue;
            Network network = new(networkName, networkMacAddress, networkSpeed);

            hr.Build(hostName, monitorName, cpu, network, ram);
        }

        /// <summary>
        /// Builds network usage readings by coupling network entries 6 at a time.
        /// </summary>
        /// <param name="entries">A list of network usage entries from the state database.</param>
        /// <returns>A coupled list of network usage entries.</returns>
        public static List<NetworkUsage> BuildNetworkUsage(List<HealthReportEntry> entries)
        {
            List<List<HealthReportEntry>> distinctReports = new();
            int entryCount = entries.Count;

            for (int i = 0; i < entryCount; i += 6)
            {
                distinctReports.Add(entries.Skip(i).Take(6).ToList());
            }

            //Build system model network usage objects.
            return (from item in distinctReports
                    let executionId = item.First().ExecutionId.Value
                    let logTime = item.First().LogTime.Value
                    let bytesSend = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Send").ReportNumericValue
                    let bytesSendDelta = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Send (Delta)").ReportNumericValue
                    let bytesSendSpeed = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Send (Speed)").ReportNumericValue
                    let bytesReceived = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Received").ReportNumericValue
                    let bytesReceivedDelta = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Received (Delta)").ReportNumericValue
                    let bytesReceivedSpeed = (long)item.Find(e => e.ReportKey == "Interface 0: Bytes Received (Speed)").ReportNumericValue
                    select new NetworkUsage(executionId, bytesSend, bytesSendDelta, bytesSendSpeed, bytesReceived, bytesReceivedDelta, bytesReceivedSpeed, logTime))
                    .ToList();
        }
    }
}
