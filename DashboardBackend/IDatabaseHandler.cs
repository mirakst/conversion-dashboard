using DashboardBackend.Database;
using DashboardBackend.Database.Models;
using Model;

namespace DashboardBackend
{
    public interface IDatabaseHandler
    {
        IDatabase Database { get; set; }
        IList<Execution> GetExecutionsSince(DateTime minDate);
        IList<ValidationTest> GetValidationsSince(DateTime minDate);
        IList<LogMessage> GetLogMessagesSince(DateTime minDate);
        IList<LogMessage> GetLogMessagesFromExecutionSince(DateTime minDate, int executionId);
        IList<Manager> GetManagersSince(DateTime minDate);
        bool TryUpdateManagerProperties(IList<Manager> managers, DateTime lastUpdate);
        int GetEstimatedManagerCount(int executionId);
        int AddHealthReportReadings(HealthReport healthReport, DateTime minDate);
        int AddHealthReportReadings(HealthReport hr);
        CpuLoad GetCpuReading(HealthReportEntry item);
        RamLoad GetRamReading(long? totalRam, HealthReportEntry item);
        LogMessageType GetLogMessageType(LoggingEntry entry, string content);
        ValidationStatus GetValidationStatus(AfstemningEntry entry);
        void BuildHealthReport(HealthReport healthReport);
        IList<NetworkUsage> BuildNetworkUsage(IList<HealthReportEntry> entries);
    }
}