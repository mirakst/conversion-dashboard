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
        List<Manager> GetManagers(DateTime minDate);
        List<ValidationTest> GetValidations(DateTime minDate);
        //int GetEstimatedManagerCount(int executionId);
        //int AddHealthReportReadings(HealthReport healthReport, DateTime minDate);
        //int AddHealthReportReadings(HealthReport hr);
        //CpuLoad GetCpuReading(HealthReportEntry item);
        //RamLoad GetRamReading(long? totalRam, HealthReportEntry item);
        //void BuildHealthReport(HealthReport healthReport);
        //void BuildNetworkUsage(IList<HealthReportEntry> entries);
    }
}