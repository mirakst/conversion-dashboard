using DashboardBackend.Database;
using DashboardBackend.Settings;
using Model;

namespace DashboardBackend
{
    public interface IDataHandler
    {
        IDatabase Database { get; set; }
        public void SetupDatabase(Profile profile);
        List<LogMessage> GetLogMessages(DateTime minDate);
        Tuple<List<Manager>, List<Execution>> GetParsedLogData(IList<LogMessage> messages);
        List<Execution> GetExecutions(DateTime minDate);

        //void GetExecutionsSince(DateTime minDate);
        //void GetValidationsSince(DateTime minDate);
        //void GetLogMessagesFromExecutionSince(DateTime minDate, int executionId);
        //void GetManagersSince(DateTime minDate);
        //bool TryUpdateManagerProperties(IList<Manager> managers, DateTime lastUpdate);
        //int GetEstimatedManagerCount(int executionId);
        //int AddHealthReportReadings(HealthReport healthReport, DateTime minDate);
        //int AddHealthReportReadings(HealthReport hr);
        //CpuLoad GetCpuReading(HealthReportEntry item);
        //RamLoad GetRamReading(long? totalRam, HealthReportEntry item);
        //LogMessageType GetLogMessageType(LoggingEntry entry, string content);
        //ValidationStatus GetValidationStatus(AfstemningEntry entry);
        //void BuildHealthReport(HealthReport healthReport);
        //void BuildNetworkUsage(IList<HealthReportEntry> entries);
    }
}