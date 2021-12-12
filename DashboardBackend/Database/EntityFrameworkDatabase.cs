using DashboardBackend.Database.Models;

using Microsoft.EntityFrameworkCore;

namespace DashboardBackend.Database
{
    /// <inheritdoc />
    public class EntityFrameworkDatabase : IDatabase
    {
        private readonly DbContextOptions<NetcompanyDbContext> _options;

        public EntityFrameworkDatabase(DbContextOptions<NetcompanyDbContext> options)
        {
            _options = options;
        }

        /// <inheritdoc/>
        public IList<LoggingEntry> QueryLogMessages(DateTime minDate)
        {
            using NetcompanyDbContext db = new(_options);
            var queryResult = db.Loggings
                                .Where(m => m.Created > minDate)
                                .OrderBy(m => m.Created);
            return queryResult.ToList();
        }

        /// <inheritdoc />
        public List<ExecutionEntry> QueryExecutions(DateTime minDate)
        {
            using NetcompanyDbContext db = new(_options);
            var queryResult = db.Executions
                                .Where(e => e.Created > minDate)
                                .OrderBy(e => e.Created);
            return queryResult.ToList();
        }

        /// <inheritdoc/>
        public List<EnginePropertyEntry> QueryEngineProperties(DateTime minDate)
        {
            using NetcompanyDbContext db = new(_options);
            var queryResult = db.EngineProperties
                                .Where(e => e.Timestamp > minDate)
                                .OrderBy(e => e.Timestamp);
            return queryResult.ToList();
        }

        /// <inheritdoc/>
        public List<AfstemningEntry> QueryAfstemninger(DateTime minDate)
        {
            using NetcompanyDbContext db = new(_options);
            var queryResult = db.Afstemnings
                                .Where(e => e.Afstemtdato > minDate)
                                .OrderBy(e => e.Afstemtdato);
            return queryResult.ToList();
        }

        #region Obsolete



        ///// <inheritdoc/>
        //public List<LoggingContextEntry> QueryLoggingContext(int executionId)
        //{
        //    using NetcompanyDbContext db = new(_options);
        //    var queryResult = db.LoggingContexts
        //                        .Where(e => e.ContextId > 0 && e.ExecutionId == executionId);
        //    return queryResult.ToList();
        //}

        ///// <inheritdoc/>
        //public List<HealthReportEntry> QueryHealthReport()
        //{
        //    using NetcompanyDbContext db = new(_options);
        //    var queryResult = db.HealthReports
        //                        .Where(e => e.ReportType.EndsWith("INIT"))
        //                        .OrderBy(e => e.LogTime);

        //    return queryResult.ToList();
        //}

        ///// <inheritdoc/>
        //public List<HealthReportEntry> QueryPerformanceReadings(DateTime minDate)
        //{
        //    using NetcompanyDbContext db = new(_options);
        //    var queryResult = db.HealthReports
        //                        .Where(e => e.LogTime > minDate)
        //                        .Where(e => e.ReportType == "CPU"
        //                                 || e.ReportType == "NETWORK"
        //                                 || e.ReportType == "MEMORY")
        //                        .OrderBy(e => e.LogTime);

        //    return queryResult.ToList();
        //}


        #endregion
    }
}
