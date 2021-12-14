using DashboardBackend.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace DashboardBackend.Database
{   
    /// <inheritdoc />
    public class EntityFrameworkDb : IDatabase
    {
        private readonly DbContextOptions<NetcompanyDbContext> _options;

        public EntityFrameworkDb(DbContextOptions<NetcompanyDbContext> options)
        {
            _options = options;
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
        public List<LoggingEntry> QueryLogMessages(int executionId, DateTime minDate)
        {
            using NetcompanyDbContext db = new(_options);
            var queryResult = db.Loggings
                                .Where(e => e.Created > minDate)
                                .Where(e => e.ExecutionId == executionId)
                                .OrderBy(e => e.Created);

            return queryResult.ToList();
        }

        /// <inheritdoc/>
        public List<LoggingEntry> QueryLogMessages(DateTime minDate)
        {
            using NetcompanyDbContext db = new(_options);
            var queryResult = db.Loggings
                                .Where(e => e.Created > minDate)
                                .OrderBy(e => e.Created);

            return queryResult.ToList();
        }

        /// <inheritdoc/>
        public List<LoggingContextEntry> QueryLoggingContext(int executionId)
        {
            using NetcompanyDbContext db = new(_options);
            var queryResult = db.LoggingContexts
                                .Where(e => e.ContextId > 0 && e.ExecutionId == executionId);
            return queryResult.ToList();
        }

        /// <inheritdoc/>
        public List<HealthReportEntry> QueryHealthReport(DateTime minDate)
        {
            using NetcompanyDbContext db = new(_options);
            var queryResult = db.HealthReports
                                .Where(e => e.LogTime > minDate)
                                .OrderBy(e => e.LogTime);

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
    }
}
