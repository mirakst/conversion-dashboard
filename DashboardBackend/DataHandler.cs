using DashboardBackend.Database;
using DashboardBackend.Database.Models;
using Model;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using static Model.LogMessage;
using static Model.ValidationTest;
using static Model.Manager;
using DashboardBackend.Parsers;
using Microsoft.EntityFrameworkCore;
using DashboardBackend.Settings;

namespace DashboardBackend
{
    public class DataHandler : IDataHandler
    {
        private readonly IDataParser<LogMessage, Tuple<List<Manager>, List<Execution>>> _logParser;
        private readonly IDataParser<EnginePropertyEntry, List<Manager>> _managerParser;

        public DataHandler()
        {
            _logParser = new LogMessageParser();
            _managerParser = new ManagerParser();
        }

        public IDatabase Database { get; set; }
        protected DateTime SqlMinDateTime => SqlDateTime.MinValue.Value;

        public void SetupDatabase(Profile profile)
        {
            DbContextOptions<NetcompanyDbContext> options = new DbContextOptionsBuilder<NetcompanyDbContext>()
                .UseSqlServer(profile.ConnectionString)
                .Options;
            Database = new EntityFrameworkDb(options);
        }

        /// <summary>
        /// Queries the state database for executions newer than minDate, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of executions, matching the supplied constraints.</returns>
        public List<Execution> GetExecutions(DateTime minDate)
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
        public List<ValidationTest> GetValidations(DateTime minDate)
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
        public List<LogMessage> GetLogMessages(DateTime minDate)
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

        /// <summary>
        /// Queries the state database for log messages newer than minDate, 
        /// then creates a list of them for the system model, which is returned.
        /// </summary>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <returns>A list of log messages, matching the supplied constraints.</returns>
        public Tuple<List<Manager>, List<Execution>> GetParsedLogData(List<LogMessage> messages)
        {
            var result = _logParser.Parse(messages);
            return result;
        }

        /// <summary>
        /// Queries the state database for managers added since the specified minimum date.
        /// 
        /// </summary>
        /// <remarks>The ENGINE_PROPERTIES table is used since it contains all managers and their values, and it is periodically updated.</remarks>
        /// <param name="minDate"></param>
        /// <param name="allManagers"></param>
        public List<Manager> GetManagers(DateTime minDate)
        {
            List<EnginePropertyEntry> engineEntries = Database.QueryEngineProperties(minDate);
            return _managerParser.Parse(engineEntries);
        }

        public int GetEstimatedManagerCount(int executionId)
        {
            return Database.QueryLoggingContext(executionId).Count;
        }

        /// <summary>
        /// Queries the state database for health report performance entries, 
        /// and adds them to the health report.
        /// </summary>
        /// <param name="healthReport">The conversion's health report</param>
        /// <param name="minDate">The minimum DateTime for the query results.</param>
        /// <exception cref="FormatException">Thrown if somehow the query failed and an unexpected entry is met.</exception>
        public int AddHealthReportReadings(HealthReport healthReport, DateTime minDate)
        {
            healthReport.LastModified = DateTime.Now;
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
                        ramRes.Add(GetRamReading(healthReport.Ram.Total, item));
                        break;
                    default:
                        throw new FormatException(nameof(item));
                }
            }
            netRes = BuildNetworkUsage(networkEntries);

            healthReport.Cpu.Readings = cpuRes;
            healthReport.Ram.Readings = ramRes;
            healthReport.Network.Readings = netRes;
            return queryResult.Count;
        }

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
        private LogMessageType GetLogMessageType(LoggingEntry entry, string content)
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
        private ValidationStatus GetValidationStatus(AfstemningEntry entry)
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
        public void BuildHealthReport(HealthReport healthReport)
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

            healthReport.Build(hostName, monitorName, cpu, network, ram);
        }

        /// <summary>
        /// Builds network usage readings by coupling network entries 6 at a time.
        /// </summary>
        /// <param name="entries">A list of network usage entries from the state database.</param>
        /// <returns>A coupled list of network usage entries.</returns>
        public List<NetworkUsage> BuildNetworkUsage(List<HealthReportEntry> entries)
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
