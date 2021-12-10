using DashboardBackend.Database.Models;

using Microsoft.EntityFrameworkCore;

namespace DashboardBackend.Database
{
    /// <inheritdoc />
    public class EntityFrameworkSqlDatabase : IDatabase
    {
        private readonly DbContextOptions<NetcompanyDbContext> _options;

        public EntityFrameworkSqlDatabase(DbContextOptions<NetcompanyDbContext> options)
        {
            _options = options;
        }

        /// <inheritdoc/>
        public IList<LoggingEntry> QueryLogMessages(DateTime minDate)
        {
            using NetcompanyDbContext db = new(_options);
            IList<LoggingEntry> queryResult = (from item in db.Loggings
                                               where item.Created > minDate
                                               orderby item.Created
                                               select item).ToList();
            db.Dispose();
            return queryResult;
        }

        #region Obsolete
        /// <inheritdoc/>
        //public List<AfstemningEntry> QueryAfstemninger(DateTime minDate)
        //{
        //    using NetcompanyDbContext db = new(_options);
        //    var queryResult = db.Afstemnings
        //                        .Where(e => e.Afstemtdato > minDate)
        //                        .OrderBy(e => e.Afstemtdato);
        //    db.Dispose();
        //    return queryResult.ToList();
        //}

        ///// <inheritdoc />
        //public List<ExecutionEntry> QueryExecutions(DateTime minDate)
        //{
        //    using NetcompanyDbContext db = new(_options);
        //    var queryResult = db.Executions
        //                        .Where(e => e.Created > minDate)
        //                        .OrderBy(e => e.Created);
        //    db.Dispose();
        //    return queryResult.ToList();
        //}

        ///// <inheritdoc/>
        //public IList<LoggingEntry> QueryLogMessages(int executionId, DateTime minDate)
        //{
        //    using NetcompanyDbContext db = new(_options);
        //    var queryResult = db.Loggings
        //                        .Where(e => e.Created > minDate)
        //                        .Where(e => e.ExecutionId == executionId)
        //                        .OrderBy(e => e.Created);

        //    return queryResult.ToList();
        //}

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

        ///// <inheritdoc/>
        //public List<EnginePropertyEntry> QueryEngineProperties(DateTime minDate)
        //{
        //    using NetcompanyDbContext db = new(_options);
        //    var queryResult = db.EngineProperties
        //                        .Where(e => e.Timestamp > minDate 
        //                                && (e.Key == "START_TIME"
        //                                ||  e.Key == "END_TIME"
        //                                ||  e.Key == "Læste rækker"
        //                                ||  e.Key == "Skrevne rækker"))
        //                        .OrderBy(e => e.Timestamp);

        //    return queryResult.ToList();
        //}
        #endregion
    }
}
