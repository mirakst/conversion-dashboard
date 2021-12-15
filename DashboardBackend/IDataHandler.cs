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
        Tuple<List<LogMessage>, List<Manager>, List<Execution>> GetParsedLogData(DateTime minDate);
        List<Execution> GetParsedExecutions(DateTime minDate);
        List<Manager> GetParsedManagers(DateTime minDate);
        List<ValidationTest> GetParsedValidations(DateTime minDate);
        int GetEstimatedManagerCount(int executionId);
        HealthReport GetParsedHealthReport(DateTime minDate, HealthReport healthReport);
    }
}
