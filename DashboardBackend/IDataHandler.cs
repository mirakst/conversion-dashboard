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
        List<HealthReportEntry> GetHealthReportEntries(DateTime minDate);
        HealthReport GetParsedHealthReport(List<HealthReportEntry> data, HealthReport healthReport);
    }
}
