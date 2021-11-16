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
                                .OrderBy(e => e.Afstemtdato);
            
            return queryResult.ToList();
        }


        /// <inheritdoc />
        public List<ExecutionEntry> QueryExecutions(DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.Executions
                                .Where(e => e.Created > minDate)
                                .OrderBy(e => e.Created);

            return queryResult.ToList();
        }

        /// <inheritdoc/>
        public List<LoggingEntry> QueryLogMessages(int ExecutionId, DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.Loggings
                                .Where(e => e.Created > minDate)
                                .Where(e => e.ExecutionId == ExecutionId)
                                .OrderBy(e => e.Created);

            return queryResult.ToList();
        }

        /// <inheritdoc/>
        public List<LoggingEntry> QueryLogMessages(DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.Loggings
                                .Where(e => e.Created > minDate)
                                .OrderBy(e => e.Created);

            return queryResult.ToList();
        }

        /// <inheritdoc/>
        public List<LoggingContextEntry> QueryManagers()
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.LoggingContexts
                                .Where(e => e.ContextId > 0)
                                .OrderBy(e => e.ContextId);

            return queryResult.ToList();
        }

        /// <inheritdoc/>
        public List<HealthReportEntry> QueryHealthReport()
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.HealthReports
                                .Where(e => e.ReportType.EndsWith("INIT"))
                                .OrderBy(e => e.LogTime);

            return queryResult.ToList();
        }

        /// <inheritdoc/>
        public List<HealthReportEntry> QueryPerformanceReadings(DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.HealthReports
                                .Where(e => e.ReportType == "CPU"
                                         || e.ReportType == "NETWORK"
                                         || e.ReportType == "MEMORY")
                                .OrderBy(e => e.LogTime);

            return queryResult.ToList();
        }

        /// <inheritdoc/>
        public List<EnginePropertyEntry> QueryEngineProperties(DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.EngineProperties
                                .Where(e => e.Timestamp > minDate)
                                .OrderBy(e => e.Timestamp);

            return queryResult.ToList();
        }
    }
}
