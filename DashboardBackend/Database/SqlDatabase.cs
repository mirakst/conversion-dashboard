using DashboardBackend.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlTypes;

namespace DashboardBackend.Database
{   
    /// <inheritdoc />
    public class SqlDatabase : IDatabaseHandler
    {
        public SqlDatabase() : base()
        {
        }

        public DateTime SqlMinDateTime { get; } = SqlDateTime.MinValue.Value;
        
        /// <inheritdoc/>
        public List<Afstemning> GetAfstemninger(DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.Afstemnings.Where(e => e.Afstemtdato > minDate)
                                            .OrderBy(e => e.Afstemtdato);
            return queryResult.ToList();
        }

        /// <inheritdoc/>
        public List<Afstemning> GetAfstemninger()
        {
            return GetAfstemninger(SqlMinDateTime);
        }

        /// <inheritdoc />
        public List<Execution> GetExecutions(DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.Executions.Where(e => e.Created > minDate)
                                           .OrderBy(e => e.Created);
            return queryResult.ToList();
        }

        /// <inheritdoc />
        public List<Execution> GetExecutions()
        {
            return GetExecutions(SqlMinDateTime);
        }

        /// <inheritdoc/>
        public List<Logging> GetLogMessages(DateTime minDate)
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.Loggings.Where(e => e.Created > minDate)
                                         .OrderBy(e => e.Created);
            return queryResult.ToList();
        }

        /// <inheritdoc/>
        public List<Logging> GetLogMessages()
        {
            return GetLogMessages(SqlMinDateTime);
        }
    }
}
