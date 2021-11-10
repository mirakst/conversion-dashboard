using DashboardBackend.Database.Models;

namespace DashboardBackend.Database
{   
    /// <inheritdoc />
    public class SqlDatabase : IDatabaseHandler
    {
        public SqlDatabase() : base()
        {
        }

        /// <inheritdoc/>
        public List<AfstemningEntry> QueryAfstemninger(DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.Afstemnings
                                .Where(e => e.Afstemtdato > minDate)
                                .OrderBy(e => e.Afstemtdato).ToList();
            
            return queryResult;
        }


        /// <inheritdoc />
        public List<ExecutionEntry> QueryExecutions(DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.Executions
                                .Where(e => e.Created > minDate)
                                .OrderBy(e => e.Created).ToList();

            return queryResult;
        }

        /// <inheritdoc/>
        public List<LoggingEntry> QueryLogMessages(int ExecutionId, DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.Loggings
                                .Where(e => e.Created > minDate)
                                .Where(e => e.ExecutionId == ExecutionId)
                                .OrderBy(e => e.Created).ToList();

            return queryResult;
        }

        /// <inheritdoc/>
        public List<LoggingEntry> QueryLogMessages(DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.Loggings
                                .Where(e => e.Created > minDate)
                                .OrderBy(e => e.Created).ToList();

            return queryResult;
        }

        /// <inheritdoc/>
        public List<ManagerEntry> QueryManagers()
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.Managers
                                .OrderBy(e => e.RowId).ToList();

            return queryResult;
        }

        /// <inheritdoc/>
        public List<HealthReportEntry> QueryHealthReport()
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.HealthReports
                                .Where(e => e.ReportType.EndsWith("INIT"))
                                .OrderBy(e => e.LogTime).ToList();

            return queryResult;
        }

        /// <inheritdoc/>
        public List<HealthReportEntry> QueryCpuReadings(DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.HealthReports
                                .Where(e => e.LogTime > minDate)
                                .Where(e => e.ReportType == "CPU")
                                .Where(e => e.ReportKey == "LOAD")
                                .OrderBy(e => e.LogTime).ToList();
            
            return queryResult;
        }

        /// <inheritdoc/>
        public List<HealthReportEntry> QueryNetworkReadings(DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.HealthReports
                                .Where(e => e.LogTime > minDate)
                                .Where(e => e.ReportType == "NETWORK")
                                .OrderBy(e => e.LogTime).ToList();

            return queryResult;
        }

        /// <inheritdoc/>
        public List<HealthReportEntry> QueryRamReadings(DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.HealthReports
                                .Where(e => e.LogTime > minDate)
                                .Where(e => e.ReportType == "MEMORY")
                                .Where(e => e.ReportKey == "AVAILABLE")
                                .OrderBy(e => e.LogTime).ToList();

            return queryResult;
        }
    }
}
