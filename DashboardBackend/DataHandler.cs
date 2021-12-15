using DashboardBackend.Database;
using DashboardBackend.Database.Models;
using Model;
using DashboardBackend.Parsers;
using Microsoft.EntityFrameworkCore;
using DashboardBackend.Settings;

namespace DashboardBackend
{
    /// <summary>
    /// Handles getting information from the state database and transforming it into models used by the Dashboard system through a list of <see cref="IDataParser{TInput, TOutput}"/> classes.
    /// </summary>
    public class DataHandler : IDataHandler
    {
        private readonly IDataParser<LoggingEntry, Tuple<List<LogMessage>, List<Manager>, List<Execution>>> _logParser;
        private readonly IDataParser<EnginePropertyEntry, List<Manager>> _managerParser;
        private readonly IDataParser<HealthReportEntry, HealthReport> _healthReportParser;
        private readonly IDataParser<AfstemningEntry, List<ValidationTest>> _reconciliationParser;
        private readonly IDataParser<ExecutionEntry, List<Execution>> _executionParser;

        public DataHandler()
        {
            _logParser = new LogMessageParser();
            _managerParser = new ManagerParser();
            _executionParser = new ExecutionParser();
            _healthReportParser = new HealthReportParser();
            _reconciliationParser = new ReconciliationParser();
        }

        public IDatabase Database { get; set; }

        /// <inheritdoc/>
        public void SetupDatabase(Profile profile)
        {
            DbContextOptions<NetcompanyDbContext> options = new DbContextOptionsBuilder<NetcompanyDbContext>()
                .UseSqlServer(profile.ConnectionString)
                .Options;
            Database = new EntityFrameworkDb(options);
        }

        /// <inheritdoc/>
        public List<Execution> GetParsedExecutions(DateTime minDate)
        {
            List<ExecutionEntry> data = Database.QueryExecutions(minDate);
            return _executionParser.Parse(data);
        }

        /// <inheritdoc/>
        public List<ValidationTest> GetParsedValidations(DateTime minDate)
        {
            List<AfstemningEntry> data = Database.QueryAfstemninger(minDate);
            return _reconciliationParser.Parse(data);
        }

        /// <inheritdoc/>
        public Tuple<List<LogMessage>, List<Manager>, List<Execution>> GetParsedLogData(DateTime minDate)
        {
            List<LoggingEntry> entries = Database.QueryLogMessages(minDate);
            return _logParser.Parse(entries);
        }

        /// <inheritdoc/>
        public List<Manager> GetParsedManagers(DateTime minDate)
        {
            List<EnginePropertyEntry> engineEntries = Database.QueryEngineProperties(minDate);
            return _managerParser.Parse(engineEntries);
        }

        /// <inheritdoc/>
        public int GetEstimatedManagerCount(int executionId)
        {
            return Database.QueryLoggingContext(executionId).Count;
        }

        /// <inheritdoc/>
        public HealthReport GetParsedHealthReport(DateTime minDate, HealthReport healthReport)
        {
            List<HealthReportEntry> entries = Database.QueryHealthReport(minDate);
            var parsed = _healthReportParser.Parse(entries);
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
