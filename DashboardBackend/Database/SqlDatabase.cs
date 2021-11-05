using DashboardBackend.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlTypes;
using Model;

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
        public List<ValidationTest> GetAfstemninger(DateTime minDate = default(DateTime))
        {
            minDate = minDate == default(DateTime) ? SqlMinDateTime : minDate;

            using NetcompanyDbContext db = new();
            var queryResult = db.Afstemnings.Where(e => e.Afstemtdato > minDate)
                                            .OrderBy(e => e.Afstemtdato).ToList();
            List<ValidationTest> result = new();

            foreach (AfstemningEntry test in queryResult)
            {
                result.Add(new ValidationTest(test.Afstemtdato, test.Description, DataConversion.GetValidationStatus(test), test.Manager));
            }

            return result;
        }


        /// <inheritdoc />
        public List<Execution> GetExecutions(DateTime minDate = default(DateTime))
        {
            minDate = minDate == default(DateTime) ? SqlMinDateTime : minDate;

            using NetcompanyDbContext db = new();
            var queryResult = db.Executions.Where(e => e.Created > minDate)
                                           .OrderBy(e => e.Created).ToList();
            List<Execution> result = new();

            foreach (var item in queryResult)
            {
                result.Add(new Execution((int)item.ExecutionId, (DateTime)item.Created));
            }

            return result;
        }

        /// <inheritdoc/>
        public List<LogMessage> GetLogMessages(int ExecutionId, DateTime minDate = default(DateTime))
        {
            minDate = minDate == default(DateTime) ? SqlMinDateTime : minDate;

            using NetcompanyDbContext db = new();
            var queryResult = db.Loggings.Where(e => e.Created > minDate)
                                         .Where(e => e.ExecutionId == ExecutionId)
                                         .OrderBy(e => e.Created).ToList();
            List<LogMessage> result = new();
            
            foreach (var item in queryResult)
            {
                result.Add(new LogMessage(item.LogMessage, DataConversion.GetLogMessageType(item), (int)item.ContextId, (DateTime)item.Created));
            }

            return result;
        }

        public List<Manager> GetManagers()
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.Managers.OrderBy(e => e.RowId);
            List<Manager> result = new();

            foreach (var item in queryResult)
            {
                result.Add(new Manager((int)item.RowId, (int)item.ExecutionsId, item.ManagerName));
            }

            return result;
        }

        public HealthReport GetHealthReport()
        {
            using NetcompanyDbContext db = new();
            var queryResult = db.HealthReports.OrderBy(e => e.LogTime)
                                              .ToList();

            HealthReport result = DataConversion.InitHealthReport(queryResult);

            return result;
        }

        public List<CpuLoad> GetCpuReadings(DateTime minDate = default(DateTime))
        {
            minDate = minDate == default(DateTime) ? SqlMinDateTime : minDate;

            using NetcompanyDbContext db = new();
            var queryResult = db.HealthReports.Where(e => e.LogTime > minDate && e.ReportType == "CPU")
                                              .OrderBy(e => e.LogTime).ToList();
            List<CpuLoad> result = new();

            foreach (var item in queryResult.Where(e => e.ReportKey == "LOAD"))
            {
                result.Add(new CpuLoad((int)item.ExecutionId, (long)item.ReportNumericValue, (DateTime)item.LogTime));
            }

            return result;
        }

        public List<NetworkUsage> GetNetworkReadings(DateTime minDate = default(DateTime))
        {
            minDate = minDate == default(DateTime) ? SqlMinDateTime : minDate;

            using NetcompanyDbContext db = new();
            var queryResult = db.HealthReports.Where(e => e.LogTime > minDate && e.ReportType == "NETWORK")
                                              .OrderBy(e => e.LogTime).ToList();

            List<NetworkUsage> result = DataConversion.BuildNetworkUsage(queryResult);

            return result;
        }

        public List<RamUsage> GetRamReadings(DateTime minDate = default(DateTime))
        {
            minDate = minDate == default(DateTime) ? SqlMinDateTime : minDate;

            using NetcompanyDbContext db = new();
            var queryResult = db.HealthReports.Where(e => e.LogTime > minDate && e.ReportType == "Memory")
                                              .OrderBy(e => e.LogTime).ToList();
            List<RamUsage> result = new();

            foreach (var item in queryResult.Where(e => e.ReportKey == "AVAILABLE"))
            {
                result.Add(new RamUsage((int)item.ExecutionId, (long)item.ReportNumericValue, (DateTime)item.LogTime));
            }

            return result;
        }
    }
}
