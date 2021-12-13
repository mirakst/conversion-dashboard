using DashboardBackend.Database;
using DashboardBackend.Database.Models;
using DashboardBackend.Settings;
using Model;

namespace DashboardBackend
{
    public interface IDataHandler
    {
        IDatabase Database { get; set; }
        void SetupDatabase(Profile profile);
        List<LogMessage> GetLogMessages(DateTime minDate);
        Tuple<List<Manager>, List<Execution>> GetParsedLogData(List<LogMessage> messages);
        List<Execution> GetExecutions(DateTime minDate);
        List<Manager> GetManagers(DateTime minDate);
        List<ValidationTest> GetValidations(DateTime minDate);
        int GetEstimatedManagerCount(int executionId);
        int AddHealthReportReadings(HealthReport healthReport, DateTime minDate);
        CpuLoad GetCpuReading(HealthReportEntry item);
        RamLoad GetRamReading(long? totalRam, HealthReportEntry item);
        void BuildHealthReport(HealthReport healthReport);
        List<NetworkUsage> BuildNetworkUsage(List<HealthReportEntry> entries);
    }
}
