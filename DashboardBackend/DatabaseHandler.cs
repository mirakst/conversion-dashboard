using System.Text.RegularExpressions;
using System.Data.SqlTypes;
using DashboardBackend.Database;
using DashboardBackend.Database.Models;
using Model;

namespace DashboardBackend
{
    /// <summary>
    /// Provides methods to retrieve fully parsed objects from the state database through the specified <see cref="IDatabase"/> interface.
    /// </summary>
    public class DatabaseHandler : IDatabaseHandler
    {
        public DatabaseHandler()
        {

        }

        public DatabaseHandler(IDatabase database)
        {
            Database = database;
        }

        //The class that handles the state database queries. Default is SQL.
        public IDatabase Database { get; set; }
        protected DateTime SqlMinDateTime => SqlDateTime.MinValue.Value;

        /// <summary>
        /// Queries the state database for executions newer than minDate, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of executions, matching the supplied constraints.</returns>
        public IList<Execution> GetExecutionsSince(DateTime minDate)
        {
            List<ExecutionEntry> queryResult = Database.QueryExecutions(minDate);

            return (from item in queryResult
                    let executionId = (int)item.ExecutionId.Value
                    let created = item.Created.Value
                    select new Execution(executionId, created))
                    .ToList();
        }

        /// <summary>
        /// Queries the state database for validation tests newer than minDate, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of validation tests, matching the supplied constraints.</returns>
        public IList<ValidationTest> GetValidationsSince(DateTime minDate)
        {
            List<AfstemningEntry> queryResult = Database.QueryAfstemninger(minDate);

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
        /// Queries the state database for log messages newer than minDate, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of log messages, matching the supplied constraints.</returns>
        public IList<LogMessage> GetLogMessagesSince(DateTime minDate)
        {
            List<LoggingEntry> queryResult = Database.QueryLogMessages(minDate);

            return (from item in queryResult
                    let content = Regex.Replace(item.LogMessage, @"\u001b\[\d*;?\d+m", "")
                    let type = GetLogMessageType(item, content)
                    let contextId = (int)item.ContextId.Value
                    let executionId = (int)item.ExecutionId.Value
                    let created = item.Created.Value
                    select new LogMessage(content, type, contextId, executionId, created))
                    .ToList();
        }

        public IList<LogMessage> GetLogMessagesFromExecutionSince(DateTime minDate, int executionId)
        {
            return GetLogMessagesSince(minDate).Where(m => m.ExecutionId == executionId).ToList();
        }

        /// <summary>
        /// Queries the state database for managers added since the specified minimum date.
        /// </summary>
        /// <remarks>The ENGINE_PROPERTIES table is used since it contains all managers and their values, and it is periodically updated.</remarks>
        /// <param name="minDate"></param>
        /// <param name="allManagers"></param>
        public int GetAndUpdateManagers(DateTime minDate, List<Manager> allManagers)
        {
            List<EnginePropertyEntry> engineEntries = Database.QueryEngineProperties(minDate);
            int addedManagers = 0;

            // Necessary cleanup (removes ',rnd_-XXXX' from manager names)
            foreach (var entry in engineEntries)
            {
                string name = entry.Manager.Split(',')[0];
                entry.Manager = name;
            }

            // For each entry: Find the associated manager and add the value to it. 
            // If the manager exists but already has all values set, it must be the same manager in a new execution.
            // In this case, we create the manager again, but for the other execution (since it may receive a different context ID).
            foreach (var entry in engineEntries)
            {
                // If manager was created by the log first (context id is set), find the first manager that is missing a property.
                // The entries are parsed sequentially, so once all values for a manager has been set, the next time its name pops up will be for a new execution where the values are not yet set.
                Manager logManager = allManagers.Find(m => m.Name == entry.Manager && m.ContextId != 0 && m.IsMissingValues);
                if (logManager != null)
                {
                    AddEnginePropertiesToManager(logManager, entry);
                }
                else
                {
                    // Find all managers created from ENGINE_PROPERTIES (context ID=0)
                    Manager engManager = allManagers.Find(m => m.Name == entry.Manager && m.ContextId == 0 && m.IsMissingValues);
                    if (engManager != null)
                    {
                        AddEnginePropertiesToManager(engManager, entry);
                    }
                    // If no manager was found at this point, it has neither been created from ENGINE_PROPERTIES or LOGGING - so we will create it!
                    else
                    {
                        Manager manager = new()
                        {
                            Name = entry.Manager,
                        };
                        AddEnginePropertiesToManager(manager, entry);
                        allManagers.Add(manager);
                        addedManagers++;
                    }
                }
            }
            return addedManagers;
        }

        /// <summary>
        /// Parses the value of the specified EnginePropertyEntry and adds it to the given manager.
        /// </summary>
        /// <param name="manager">The manager object associated with the entry.</param>
        /// <param name="entry">The EnginePropertyEntry to get data from.</param>
        private void AddEnginePropertiesToManager(Manager manager, EnginePropertyEntry entry)
        {
            switch (entry.Key)
            {
                case "START_TIME":
                    if (DateTime.TryParse(entry.Value, out DateTime startTime))
                    {
                        manager.StartTime = startTime;
                        manager.Status = ManagerStatus.Running;
                        if (manager.EndTime.HasValue)
                        {
                            manager.Runtime = manager.EndTime.Value.Subtract(startTime);
                            manager.Status = ManagerStatus.Ok;
                        }
                    }
                    break;
                case "END_TIME":
                    if (DateTime.TryParse(entry.Value, out DateTime endTime))
                    {
                        manager.EndTime = endTime;
                        if (manager.StartTime.HasValue)
                        {
                            manager.Runtime = endTime.Subtract(manager.StartTime.Value);
                            manager.Status = ManagerStatus.Ok;
                        }
                    }
                    break;
                case "Læste rækker":
                    if (int.TryParse(entry.Value, out int rowsRead))
                    {
                        manager.RowsRead = rowsRead;
                    }
                    break;
                case "Skrevne rækker":
                    if (int.TryParse(entry.Value, out int rowsWritten))
                    {
                        manager.RowsWritten = rowsWritten;
                    }
                    break;

                default:
                    break;
            }
        }

        public IList<Manager> GetManagersSince(DateTime minDate)
        {
            return new List<Manager>();
        }

        public bool TryUpdateManagerProperties(IList<Manager> managers, DateTime lastUpdate)
        {
            return true;
        }

        public int GetEstimatedManagerCount(int executionId)
        {
            return Database.QueryLoggingContext(executionId).Count;
        }

        /// <summary>
        /// Queries the state database for health report performance entries, 
        /// and adds them to the health report.
        /// </summary>
        /// <param name="hr">The conversion's health report</param>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <exception cref="FormatException">Thrown if somehow the query failed and an unexpected entry is met.</exception>
        public int AddHealthReportReadings(HealthReport hr, DateTime minDate)
        {
            hr.LastModified = DateTime.Now;
            List<HealthReportEntry> queryResult = Database.QueryPerformanceReadings(minDate);
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
            netRes = BuildNetworkUsage(networkEntries).ToList();

            //hr.Cpu.Readings = cpuRes;
            //hr.Ram.Readings = ramRes;
            //hr.Network.Readings = netRes;
            return queryResult.Count;
        }

        /// <summary>
        /// Queries the state database for health report performance entries, 
        /// and adds them to the health report.
        /// </summary>
        /// <param name="hr">The conversion's health report</param>
        public int AddHealthReportReadings(HealthReport hr) => AddHealthReportReadings(hr, SqlMinDateTime);

        /// <summary>
        /// Creates a list of CPU Readings for the system model, which is returned.
        /// </summary>
        /// <param name="item">A state database entry with cpu load readings</param>
        /// <returns>A CPU load reading.</returns>
        public CpuLoad GetCpuReading(HealthReportEntry item)
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
        public RamLoad GetRamReading(long? totalRam, HealthReportEntry item)
        {
            if (!item.ReportNumericValue.HasValue || !totalRam.HasValue || !item.LogTime.HasValue) 
            {
                throw new ArgumentException("Cannot create reading without a log time, a report value, and a total RAM value.");
            }

            int executionId = item.ExecutionId.Value;
            long reportNumValue = item.ReportNumericValue.Value;
            double load = 1 - Convert.ToDouble(reportNumValue) / Convert.ToDouble(totalRam);
            long available = item.ReportNumericValue.Value;
            DateTime logTime = item.LogTime.Value;
            return new RamLoad(executionId, load, available, logTime);
        }

        /// <summary>
        /// Returns the type of the log message parameter 'entry'.
        /// </summary>
        /// <param name="entry">A single entry from the [LOGGING] table in the state database.</param>
        /// <returns>A log message type besed on the enum in the log message class.</returns>
        /// <exception cref="ArgumentException">Thrown if the parameter passed is not a legal log message type.</exception>
        public LogMessageType GetLogMessageType(LoggingEntry entry, string content)
        {
            var type = entry.LogLevel switch
            {
                "INFO" => LogMessageType.Info,
                "WARN" => LogMessageType.Warning,
                "ERROR" => LogMessageType.Error,
                "FATAL" => LogMessageType.Fatal,
                _ => LogMessageType.None,
            };

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
        public ValidationStatus GetValidationStatus(AfstemningEntry entry)
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
        public void BuildHealthReport(HealthReport hr)
        {
            List<HealthReportEntry> queryResult = Database.QueryHealthReport();

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
        public IList<NetworkUsage> BuildNetworkUsage(IList<HealthReportEntry> entries)
        {
            List<NetworkUsage> result = new();
            List<List<HealthReportEntry>> distinctReports = new();
            int entryCount = entries.Count;

            for (int i = 0; i < entryCount; i += 6)
            {
                distinctReports.Add(entries.Skip(i).Take(6).ToList());
            }
            
            foreach (var report in distinctReports)
            {
                int? execId = report.First().ExecutionId.Value;
                DateTime? logTime = report.First().LogTime.Value;

                if (execId.HasValue && logTime.HasValue)
                {
                    long bytesSend = 0,
                          bytesSendDelta = 0,
                          bytesSendSpeed = 0,
                          bytesRcv = 0,
                          bytesRcvDelta = 0,
                          bytesRcvSpeed = 0;
                    foreach (var reading in report)
                    {
                        switch (reading.ReportKey)
                        {
                            case "Interface 0: Bytes Send":
                                bytesSend = reading.ReportNumericValue.Value;
                                break;
                            case "Interface 0: Bytes Send (Delta)":
                                bytesSendDelta = reading.ReportNumericValue.Value;
                                break;
                            case "Interface 0: Bytes Send (Speed)":
                                bytesSendSpeed = reading.ReportNumericValue.Value;
                                break;
                            case "Interface 0: Bytes Received":
                                bytesRcv = reading.ReportNumericValue.Value;
                                break;
                            case "Interface 0: Bytes Received (Delta)":
                                bytesRcvDelta = reading.ReportNumericValue.Value;
                                break;
                            case "Interface 0: Bytes Received (Speed)":
                                bytesRcvSpeed = reading.ReportNumericValue.Value;
                                break;
                            default:
                                break;
                        }
                    }
                    result.Add(new NetworkUsage(execId.Value,
                                                bytesSend,
                                                bytesSendDelta,
                                                bytesSendSpeed,
                                                bytesRcv,
                                                bytesRcvDelta,
                                                bytesRcvSpeed,
                                                logTime.Value));
                }
            }
            return result;
        }
    }
}
