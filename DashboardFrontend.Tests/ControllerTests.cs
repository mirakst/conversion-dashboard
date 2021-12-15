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
    public class DashboardControllerTests
    {
        public DashboardController GetTestDashboardController(bool startWithExecution, IDatabase database)
        {
            DashboardController c = new();
            if (startWithExecution)
            {
                c.Conversion!.Executions.Add(new(1, DateTime.MinValue));
            }
            c.DataHandler.Database = database;
            return c;
        }

        [Fact]
        public void UpdateManagerOverview_ManagerCreatedFromLog_AssignsStartTime()
        {
            DateTime expected = new(1999, 02, 23);
            var dh = new TestDatabase
            {
                EnginePropertyEntries =
                {
                    new()
                    {
                        Manager = "Manager.1",
                        Key = "START_TIME",
                        Value = expected.ToString()
                    }
                }
            };
            var controller = GetTestDashboardController(startWithExecution: true, dh);
            controller!.Conversion!.AllManagers.Add(new() { ContextId = 0, Name = "Manager.1" });

            controller.UpdateManagers();
            DateTime? actual = controller.Conversion.AllManagers[0].StartTime;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateManagerOverview_ManagerNotFound_CreatesManager()
        {
            var dh = new TestDatabase
            {
                EnginePropertyEntries =
                {
                    new()
                    {
                        Manager = "Manager.1",
                        Key = "START_TIME",
                        Value = ""
                    }
                }
            };
            var controller = GetTestDashboardController(startWithExecution: true, dh);

            controller.UpdateManagers();

            Assert.Collection(controller!.Conversion!.AllManagers,
                item => Assert.Equal("Manager.1", item.Name));
        }

        [Fact]
        public void UpdateManagerOverview_OneManagerCreatedFromLogAndOneNotFound_UpdatesManagerOneAndCreatesNew()
        {
            var dh = new TestDatabase
            {
                EnginePropertyEntries =
                {
                    new() { Manager = "Manager.1", Key = "START_TIME", Value = DateTime.MinValue.ToString() },
                    new() { Manager = "Manager.2", Key = "START_TIME", Value = DateTime.MinValue.ToString() }
                }
            };
            var controller = GetTestDashboardController(startWithExecution: true, dh);
            controller!.Conversion!.AllManagers.Add(new() { Name = "Manager.1", ContextId = 1 });

            controller.UpdateManagers();

            Assert.Collection(controller.Conversion!.AllManagers,
                item => Assert.Equal("Manager.1", item.Name),
                item => Assert.Equal("Manager.2", item.Name));
        }

        [Fact]
        public void UpdateEstimatedManagerCounts_NoLoggingContextEntries_GetsCountFromLogMessage()
        {
            var dh = new TestDatabase();
            var controller = GetTestDashboardController(startWithExecution: true, dh);
            controller.Conversion!.ActiveExecution.Log.Messages.Add(new("Loaded managerclasses:\nManager.1\nManager.2\nManager.3", LogMessage.LogMessageType.Info, 0, 1, DateTime.MinValue));
            int expected = 3;

            controller.UpdateEstimatedManagerCounts();
            int actual = controller!.Conversion!.ActiveExecution.EstimatedManagerCount;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateEstimatedManagerCounts_HasLoggingContextEntries_GetsCount()
        {
            var dh = new TestDatabase 
            {
                LoggingContextEntries =
                {
                    new() { ExecutionId = 1, Context = "Manager.1" },
                    new() { ExecutionId = 1, Context = "Manager.2" },
                    new() { ExecutionId = 1, Context = "Manager.3" }
                }
            };
            var controller = GetTestDashboardController(startWithExecution: true, dh);
            int expected = 3;

            controller.UpdateEstimatedManagerCounts();
            int actual = controller!.Conversion!.ActiveExecution!.EstimatedManagerCount;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateLog_ExecutionNotFound_CreatesExecution()
        {
            var dh = new TestDatabase
            {
                LoggingEntries =
                {
                    new()
                    {
                        ExecutionId = 1,
                        Created = DateTime.MinValue,
                        LogMessage = "",
                        LogLevel = "INFO",
                        ContextId = 0
                    }
                }
            };
            var controller = GetTestDashboardController(startWithExecution: false, dh);

            controller.UpdateLog();
            var result = controller!.Conversion!.Executions.Find(e => e.Id == 1);

            Assert.NotNull(result);
        }

        [Fact]
        public void UpdateLog_ExecutionExists_AddsLogMessageToExecution()
        {
            var dh = new TestDatabase();
            dh.LoggingEntries.Add(new()
            {
                ExecutionId = 1,
                Created = DateTime.MinValue,
                LogMessage = "",
                LogLevel = "INFO",
                ContextId = 0
            });
            var controller = GetTestDashboardController(startWithExecution: true, dh);

            controller.UpdateLog();
            var result = controller!.Conversion!.ActiveExecution.Log.Messages.Find(m => m.ExecutionId == 1);

            Assert.NotNull(result);
        }

        [Fact]
        public void UpdateLog_NoNewData_DoesNotUpdateLastLogUpdatedProperty()
        {
            var dh = new TestDatabase();
            var controller = GetTestDashboardController(startWithExecution: true, dh);
            DateTime expected = new(1999, 02, 23);
            controller!.Conversion!.LastLogUpdated = expected;

            controller.UpdateLog();
            DateTime actual = controller.Conversion.LastLogUpdated;
            
            Assert.Equal(expected, actual);
        }

        internal class TestDatabase : IDatabase
        {
            public TestDatabase()
            {
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

            public List<HealthReportEntry> QueryHealthReport(DateTime minDate)
            {
                return HealthReportEntries;
            }
        }
    }
}
