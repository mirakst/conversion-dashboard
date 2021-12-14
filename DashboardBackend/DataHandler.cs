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
        private readonly IDataParser<HealthReportEntry, HealthReport> _healthReportParser;

        public DataHandler()
        {
            _logParser = new LogMessageParser();
            _managerParser = new ManagerParser();
            _healthReportParser = new HealthReportParser();
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
            var tuple = _logParser.Parse(messages);
            return tuple;
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
        public List<HealthReportEntry> GetHealthReportEntries(DateTime minDate)
        {
            List<HealthReportEntry> result = Database.QueryHealthReport(minDate);
            return result;
        }

        public HealthReport GetParsedHealthReport(List<HealthReportEntry> data, HealthReport healthReport)
        {
            var parsed = _healthReportParser.Parse(data);
            // Check for any info updates
            if (parsed.HostName != string.Empty || healthReport.HostName is null)
            {
                healthReport.HostName = parsed.HostName;
            }
            if (parsed.MonitorName != string.Empty || healthReport.MonitorName is null)
            {
                healthReport.MonitorName = parsed.MonitorName;
            }
            if (parsed.Cpu.Name != string.Empty || healthReport.Cpu.Name is null)
            {
                healthReport.Cpu.Name = parsed.Cpu.Name;
                healthReport.Cpu.MaxFrequency = parsed.Cpu.MaxFrequency;
                healthReport.Cpu.Cores = parsed.Cpu.Cores;
            }
            if (parsed.Network.Name != string.Empty || healthReport.Network.Name is null)
            {
                healthReport.Network.Name = parsed.Network.Name;
                healthReport.Network.MacAddress = parsed.Network.MacAddress;
                healthReport.Network.Speed = parsed.Network.Speed;
            }
            if (parsed.Ram.Total.HasValue)
            {
                healthReport.Ram.Total = parsed.Ram.Total.Value;
            }
            // Add any new readings
            healthReport.Cpu.Readings.AddRange(parsed.Cpu.Readings);
            healthReport.Ram.AddReadings(parsed.Ram.Readings);
            healthReport.Network.Readings.AddRange(parsed.Network.Readings);
            return healthReport;
        }
    }
}
