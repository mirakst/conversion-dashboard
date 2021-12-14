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
        public Controller GetTestController(bool startWithExecution)
        {
            Controller c = new();
            if (startWithExecution)
            {
                c.Conversion!.Executions.Add(new(1, DateTime.MinValue));
            }
            return c;
        }

        [Fact]
        public void UpdateManagerOverview_ManagerCreatedFromLog_AssignsStartTime()
        {
            DateTime expected = new DateTime(1999, 02, 23);
            var dh = new TestDataHandler();
            dh.EnginePropertyEntries.Add(new()
            {
                Manager = "Manager.1",
                Key = "START_TIME",
                Value = expected.ToString()
            });
            DataUtilities.DatabaseHandler = dh;
            Controller controller = GetTestController(startWithExecution: true);
            controller!.Conversion!.AllManagers.Add(new() { ContextId = 0, Name = "Manager.1"});

            controller.UpdateManagerOverview();
            DateTime? actual = controller.Conversion.AllManagers[0].StartTime;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateManagerOverview_ManagerNotFound_CreatesManager()
        {
            var dh = new TestDataHandler();
            dh.EnginePropertyEntries.Add(new()
            {
                Manager = "Manager.1",
                Key = "START_TIME",
                Value = ""
            });
            DataUtilities.DatabaseHandler = dh;
            Controller controller = GetTestController(startWithExecution: true);

            controller.UpdateManagerOverview();

            Assert.True(controller!.Conversion!.AllManagers.Count == 1);
        }

        [Fact]
        public void UpdateManagerOverview_OneManagerCreatedFromLogAndOneNotFound_UpdatesManagerOneAndCreatesNew()
        {
            var dh = new TestDataHandler();
            dh.EnginePropertyEntries.AddRange(new List<EnginePropertyEntry>
            {
                new() { Manager = "Manager.1", Key = "START_TIME", Value = DateTime.MinValue.ToString() },
                new() { Manager = "Manager.2", Key = "START_TIME", Value = DateTime.MinValue.ToString() }
            });
            DataUtilities.DatabaseHandler = dh;
            Controller controller = GetTestController(startWithExecution: true);
            controller!.Conversion!.AllManagers.Add(new() { Name = "Manager.1", ContextId = 1 });

            controller.UpdateManagerOverview();

            Assert.True(controller.Conversion.AllManagers[0].StartTime == DateTime.MinValue && controller.Conversion.AllManagers.Count == 2);
        }

        [Fact]
        public void UpdateEstimatedManagerCounts_NoLoggingContextEntries_GetsCountFromLogMessage()
        {
            var dh = new TestDataHandler();
            DataUtilities.DatabaseHandler = dh;
            Controller controller = GetTestController(startWithExecution: true);
            controller.Conversion!.ActiveExecution.Log.Messages.Add(new("Loaded managerclasses:\nManager.1\nManager.2\nManager.3", LogMessage.LogMessageType.Info, 0, 1, DateTime.MinValue));
            int expected = 3;

            controller.UpdateEstimatedManagerCounts();
            int actual = controller!.Conversion!.ActiveExecution.EstimatedManagerCount;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateEstimatedManagerCounts_HasLoggingContextEntries_GetsCount()
        {
            var dh = new TestDataHandler();
            dh.LoggingContextEntries.AddRange(new List<LoggingContextEntry>
            {
                new() { ExecutionId = 1, Context = "Manager.1" },
                new() { ExecutionId = 1, Context = "Manager.2" },
                new() { ExecutionId = 1, Context = "Manager.3" }
            });
            DataUtilities.DatabaseHandler = dh;
            Controller controller = GetTestController(startWithExecution: true);
            int expected = 3;

            controller.UpdateEstimatedManagerCounts();
            int actual = controller!.Conversion!.ActiveExecution!.EstimatedManagerCount;

            Assert.Equal(expected, actual);
        }

        //[Fact]
        //public void UpdateLog_ExecutionNotFound_CreatesExecution()
        //{
        //    var dh = new TestDataHandler();
        //    dh.LoggingEntries.Add(new()
        //    {
        //        ExecutionId = 1,
        //        Created = DateTime.MinValue,
        //        LogMessage = "",
        //        LogLevel = "INFO",
        //        ContextId = 0
        //    });
        //    DataUtilities.DatabaseHandler = dh;
        //    Controller controller = GetTestController(startWithExecution: false);

        //    controller.UpdateLog();
        //    var result = controller!.Conversion!.Executions.Find(e => e.Id == 1);

        //    Assert.NotNull(result);
        //}

        [Fact]
        public void UpdateLog_ExecutionExists_AddsLogMessageToExecution()
        {
            var dh = new TestDataHandler();
            dh.LoggingEntries.Add(new()
            {
                ExecutionId = 1,
                Created = DateTime.MinValue,
                LogMessage = "",
                LogLevel = "INFO",
                ContextId = 0
            });
            DataUtilities.DatabaseHandler = dh;
            Controller controller = GetTestController(startWithExecution: true);

            controller.UpdateLog();
            var result = controller!.Conversion!.ActiveExecution.Log.Messages.Find(m => m.ExecutionId == 1);

            Assert.NotNull(result);
        }

        //[Fact]
        //public void UpdateLog_OneLogMessageFound_EnqueuesLogMessage()
        //{
        //    var dh = new TestDataHandler();
        //    dh.LoggingEntries.Add(new()
        //    {
        //        ExecutionId = 1,
        //        Created = DateTime.MinValue,
        //        LogMessage = "",
        //        LogLevel = "INFO",
        //        ContextId = 0
        //    });
        //    DataUtilities.DatabaseHandler = dh;
        //    Controller controller = GetTestController(startWithExecution: true);

        //    controller.UpdateLog();

        //    Assert.NotEmpty(controller.LogParseQueue);
        //}

        [Fact]
        public void UpdateLog_NoNewData_DoesNotUpdateLastLogUpdatedProperty()
        {
            var dh = new TestDataHandler();
            DataUtilities.DatabaseHandler = dh;
            Controller controller = GetTestController(startWithExecution: true);
            DateTime expected = new(1999, 02, 23);
            controller!.Conversion!.LastLogUpdated = expected;

            controller.UpdateLog();
            DateTime actual = controller.Conversion.LastLogUpdated;
            
            Assert.Equal(expected, actual);
        }

        //[Fact]
        //public void ParseLogMessage_StartedManagerNotFound_CreatesManager()
        //{
        //    var dh = new TestDataHandler();
        //    DataUtilities.DatabaseHandler = dh;
        //    Controller controller = GetTestController(startWithExecution: true);
        //    LogMessage input = new("Starting manager: Manager.1", LogMessage.LogMessageType.Info, 1, 1, DateTime.MinValue);

        //    controller.ParseLogMessage(input);

        //    Assert.NotEmpty(controller!.Conversion!.AllManagers);
        //}

        //[Fact]
        //public void ParseLogMessage_StartedManagerExistsWithoutContextId_AssignsContextId()
        //{
        //    var dh = new TestDataHandler();
        //    DataUtilities.DatabaseHandler = dh;
        //    Controller controller = GetTestController(startWithExecution: true);
        //    Manager manager = new()
        //    {
        //        Name = "Manager.1",
        //        ContextId = 0
        //    };
        //    controller!.Conversion!.ActiveExecution.Managers.Add(manager);
        //    controller.Conversion.AllManagers.Add(manager);
        //    LogMessage input = new("Starting manager: Manager.1", LogMessage.LogMessageType.Info, 1, 1, DateTime.MinValue);
        //    int expected = 1;

        //    controller.ParseLogMessage(input);
        //    int actual = manager.ContextId;

        //    Assert.Equal(expected, actual);
        //}

        //[Fact]
        //public void ParseLogMessage_StartedManagerIsInMultipleExecutions_AddsManagerToCorrectExecution()
        //{
        //    var dh = new TestDataHandler();
        //    DataUtilities.DatabaseHandler = dh;
        //    Controller controller = GetTestController(startWithExecution: true);
        //    Manager manager = new()
        //    {
        //        Name = "Manager.1",
        //        ContextId = 1
        //    };
        //    controller!.Conversion!.Executions[0].Managers.Add(manager);
        //    controller.Conversion.AddExecution(new(2, DateTime.Now));
        //    LogMessage input = new("Starting manager: Manager.1", LogMessage.LogMessageType.Info, 1, 2, DateTime.MinValue);

        //    controller.ParseLogMessage(input);

        //    Assert.NotEmpty(controller.Conversion.Executions[1].Managers);
        //}

        //[Fact]
        //public void ParseLogMessage_StartedManagerExistsButIsNotInExecution_AddsManagerToExecution()
        //{
        //    var dh = new TestDataHandler();
        //    DataUtilities.DatabaseHandler = dh;
        //    Controller controller = GetTestController(true);
        //    Manager manager = new()
        //    {
        //        Name = "Manager.1",
        //        ContextId = 0
        //    };
        //    controller!.Conversion!.AllManagers.Add(manager);
        //    LogMessage input = new("Starting manager: Manager.1", LogMessage.LogMessageType.Info, 1, 1, DateTime.MinValue);

        //    controller.ParseLogMessage(input);

        //    Assert.NotEmpty(controller.Conversion.Executions[0].Managers);
        //}

        [Fact]
        public void ParseLogMessage_FinishedManagerExistsWithEndTime_AssignsOkStatus()
        {
            var dh = new TestDataHandler();
            DataUtilities.DatabaseHandler = dh;
            Controller controller = GetTestController(startWithExecution: true);
            Manager manager = new()
            {
                Name = "Manager.1",
                ContextId = 1,
                EndTime = DateTime.MinValue,
                Status = Manager.ManagerStatus.Running
            };
            controller!.Conversion!.ActiveExecution.AddManager(manager);
            LogMessage input = new("Manager execution done.", LogMessage.LogMessageType.Info, 1, 1, DateTime.MinValue);

            controller.ParseLogMessage(input);

            Assert.True(manager.Status == Manager.ManagerStatus.Ok);
        }

        [Fact]
        public void ParseLogMessage_FinishedManagerExistsWithoutEndTime_AssignsOkStatusAndEndTime()
        {
            var dh = new TestDataHandler();
            DataUtilities.DatabaseHandler = dh;
            Controller controller = GetTestController(startWithExecution: true);
            Manager manager = new()
            {
                Name = "Manager.1",
                ContextId = 1,
                Status = Manager.ManagerStatus.Running
            };
            controller!.Conversion!.ActiveExecution.AddManager(manager);
            LogMessage input = new("Manager execution done.", LogMessage.LogMessageType.Info, 1, 1, DateTime.MinValue);

            controller.ParseLogMessage(input);

            Assert.True(manager.Status == Manager.ManagerStatus.Ok && manager.EndTime.HasValue);
        }

        [Fact]
        public void ParseLogMessage_FinishedManagerIsInMultipleExecutions_UpdatesCorrectManager()
        {
            var dh = new TestDataHandler();
            DataUtilities.DatabaseHandler = dh;
            Controller controller = GetTestController(startWithExecution: true);
            // Set up execution 1
            Manager exec1Manager = new()
            {
                Name = "Manager.1",
                ContextId = 1,
                Status = Manager.ManagerStatus.Running
            };
            controller!.Conversion!.ActiveExecution.AddManager(exec1Manager);
            // Set up execution 2
            controller.Conversion.AddExecution(new(2, DateTime.MinValue));
            Manager exec2Manager = new()
            {
                Name = "Manager.1",
                ContextId = 1,
                Status = Manager.ManagerStatus.Running
            };
            controller.Conversion.ActiveExecution.AddManager(exec2Manager);
            LogMessage input = new("Manager execution done.", LogMessage.LogMessageType.Info, 1, 2, DateTime.MinValue);

            controller.ParseLogMessage(input);

            Assert.True(exec2Manager.Status == Manager.ManagerStatus.Ok && exec2Manager.EndTime.HasValue);
        }

        [Fact]
        public void ParseLogMessage_FinishedExecution_UpdatesStatusAndEndTime()
        {
            var dh = new TestDataHandler();
            DataUtilities.DatabaseHandler = dh;
            Controller controller = GetTestController(startWithExecution: true);
            LogMessage input = new("Program closing due to the following error:", LogMessage.LogMessageType.Fatal, 1, 1, DateTime.Now);

            controller.ParseLogMessage(input);
            Execution result = controller!.Conversion!.ActiveExecution;

            Assert.True(result.EndTime.HasValue && result.Status == Execution.ExecutionStatus.Finished);
        }

        internal class TestDataHandler : IDatabaseHandler
        {
            public TestDataHandler()
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
