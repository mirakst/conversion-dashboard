using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DashboardBackend;
using DashboardBackend.Database;
using DashboardBackend.Database.Models;
using Model;
using Xunit;

namespace DashboardFrontend.Tests
{
    public class ControllerTests
    {
        [Fact]
        public void UpdateManagerOverview_ManagerCreatedFromLog_AssignsStartTime()
        {
            DateTime expected = new DateTime(1999, 02, 23);
            var dh = new TestDataHandler(false);
            dh.EnginePropertyEntries.Add(new()
            {
                Manager = "Manager 1",
                Key = "START_TIME",
                Value = expected.ToString()
            });
            DataUtilities.DatabaseHandler = dh;
            Controller controller = new() { Conversion = new() };
            controller.Conversion.Executions.Add(new(1, DateTime.MinValue));
            controller.Conversion.AllManagers.Add(new() { ContextId = 0, Name = "Manager 1"});

            controller.UpdateManagerOverview();
            DateTime? actual = controller.Conversion.AllManagers[0].StartTime;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateManagerOverview_ManagerNotFound_CreatesManager()
        {
            var dh = new TestDataHandler(false);
            dh.EnginePropertyEntries.Add(new()
            {
                Manager = "Manager 1",
                Key = "START_TIME",
                Value = ""
            });
            DataUtilities.DatabaseHandler = dh;
            Conversion c = new()
            {
                AllManagers = new List<Manager>()
            };

            _ = DataUtilities.GetAndUpdateManagers(DateTime.Now, c.AllManagers);

            Assert.True(c.AllManagers.Count == 1);
        }

        [Fact]
        public void UpdateManagerOverview_OneManagerCreatedFromLogAndOneNotFound_UpdatesManagerOneAndCreatesNew()
        {
            var dh = new TestDataHandler(false);
            dh.EnginePropertyEntries.AddRange(new List<EnginePropertyEntry>
            {
                new() { Manager = "Manager 1", Key = "START_TIME", Value = DateTime.MinValue.ToString() },
                new() { Manager = "Manager 2", Key = "START_TIME", Value = DateTime.MinValue.ToString() }
            });
            DataUtilities.DatabaseHandler = dh;

            List<Manager> managers = new()
            {
                new Manager() { Name = "Manager 1", ContextId = 1 }
            };

            _ = DataUtilities.GetAndUpdateManagers(DateTime.Now, managers);

            Assert.True(managers.Count == 2 && managers[0].StartTime == DateTime.MinValue);
        }

        internal class TestDataHandler : IDatabaseHandler
        {
            public TestDataHandler(bool startWithExecution)
            {
                if (startWithExecution)
                {
                    ExecutionEntries.Add(new() { ExecutionId = 0, Created = DateTime.Now });
                }
            }

            public List<AfstemningEntry> AfstemningEntries { get; set; } = new();
            public List<EnginePropertyEntry> EnginePropertyEntries { get; set; } = new();
            public List<ExecutionEntry> ExecutionEntries{ get; set; } = new();
            public List<HealthReportEntry> HealthReportEntries { get; set; } = new();
            public List<LoggingContextEntry> LoggingContextEntries { get; set; } = new();
            public List<LoggingEntry> LoggingEntries { get; set; } = new();

            public List<AfstemningEntry> QueryAfstemninger(DateTime minDate)
            {
                return AfstemningEntries;
            }

            public List<EnginePropertyEntry> QueryEngineProperties(DateTime minDate)
            {
                return EnginePropertyEntries;
            }

            public List<ExecutionEntry> QueryExecutions(DateTime minDate)
            {
                return ExecutionEntries;
            }

            public List<HealthReportEntry> QueryHealthReport()
            {
                return HealthReportEntries;
            }

            public List<LoggingContextEntry> QueryLoggingContext(int executionId)
            {
                return LoggingContextEntries;
            }

            public List<LoggingEntry> QueryLogMessages(int executionId, DateTime minDate)
            {
                return LoggingEntries;
            }

            public List<LoggingEntry> QueryLogMessages(DateTime minDate)
            {
                return LoggingEntries;
            }

            public List<HealthReportEntry> QueryPerformanceReadings(DateTime minDate)
            {
                return HealthReportEntries;
            }
        }
    }
}
