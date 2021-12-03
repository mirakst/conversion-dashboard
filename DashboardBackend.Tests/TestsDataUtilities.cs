using DashboardBackend.Tests.Database;
using Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace DashboardBackend.Tests
{
    public class TestsDataUtilities
    {
        #region GetExecution
        [Fact]
        public void GetExecution_GetsExecutionsFromTestDatabase_ReturnTrue()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            var expected = new List<Execution>()
            {
                new Execution(1, DateTime.Parse("01-01-2020 12:00:00")),
                new Execution(2, DateTime.Parse("01-01-2020 13:00:00")),
            };

            var actual = DataUtilities.GetExecutions();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetExecution_GetsExecutionsFromTestDatabaseWithNewerSQLMinTimeThanLatestExecution_ReturnEmpty()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();

            var actual = DataUtilities.GetExecutions(DateTime.Parse("02-01-2020 12:00:00"));

            Assert.Empty(actual);
        }

        [Fact]
        public void GetExecution_GetsExecutionsFromTestDatabaseNegativeExecutionId_ReturnFalse()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            var expected = new List<Execution>()
            {
                new Execution(-1, DateTime.Parse("01-01-2020 12:00:00")),
                new Execution(-2, DateTime.Parse("01-01-2020 13:00:00")),
            };

            var actual = DataUtilities.GetExecutions();

            Assert.False(expected == actual);
        }
        #endregion
        #region GetAfstemninger
        [Fact]
        public void GetAfstemninger_GetsAfstemningerFromTestDatabase_ReturnTrue()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            var expected = new List<ValidationTest>()
            {
                new ValidationTest(DateTime.Parse("01-01-2020 12:00:00"),
                                   "validationOne",
                                   ValidationTest.ValidationStatus.Ok,
                                   "managerOne",
                                   0,
                                   0,
                                   0,
                                   "srcSql",
                                   "dstSql"),
                new ValidationTest(DateTime.Parse("01-01-2020 13:00:00"),
                                   "validationTwo",
                                   ValidationTest.ValidationStatus.Ok,
                                   "managerTwo",
                                   0,
                                   0,
                                   0,
                                   "srcSql",
                                   "dstSql")
            };

            var actual = DataUtilities.GetAfstemninger();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetAfstemninger_GetsAfstemningerFromTestDatabaseWhereAfstemningerDateIsOlderThanMinDate_ReturnEmpty()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();

            var actual = DataUtilities.GetAfstemninger(DateTime.Parse("02-01-2020 12:00:00"));

            Assert.Empty(actual);
        }
        #endregion
        #region GetLogMessages
        [Fact]
        public void GetLogMessage_GetsLogMessagesFromTestDatabase_ReturnTrue()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            var expected = new List<LogMessage>()
            {
                new LogMessage("Afstemning Error",
                               LogMessage.LogMessageType.Validation | LogMessage.LogMessageType.Error,
                               0,
                               DateTime.Parse("01-01-2020 17:00:00")),
                new LogMessage("Check - Error",
                               LogMessage.LogMessageType.Validation | LogMessage.LogMessageType.Error,
                               0,
                               DateTime.Parse("01-01-2020 18:00:00")),
                new LogMessage("Info",
                               LogMessage.LogMessageType.Info,
                               0,
                               DateTime.Parse("01-01-2020 12:00:00")),
                new LogMessage("Warning",
                               LogMessage.LogMessageType.Warning,
                               0,
                               DateTime.Parse("01-01-2020 13:00:00")),
                new LogMessage("Error",
                               LogMessage.LogMessageType.Error,
                               0,
                               DateTime.Parse("01-01-2020 14:00:00")),
                new LogMessage("Fatal",
                               LogMessage.LogMessageType.Fatal,
                               0,
                               DateTime.Parse("01-01-2020 15:00:00")),
                new LogMessage("None",
                               LogMessage.LogMessageType.None,
                               0,
                               DateTime.Parse("01-01-2020 16:00:00")),
            };

            var actual = DataUtilities.GetLogMessages();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetLogMessage_GetsLogMessagesFromTestDatabaseWhereMinDateIsGreaterThanNewestLogMessage_ReturnEmpty()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();

            var actual = DataUtilities.GetLogMessages(DateTime.Parse("02-01-2020 12:00:00"));

            Assert.Empty(actual);
        }

        [Fact]
        public void GetLogMessage_GetsLogMessagesWithSpecificExecutionId_ReturnTrue()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            var expected = new List<LogMessage>()
            {
                new LogMessage("Info",
                               LogMessage.LogMessageType.Info,
                               0,
                               DateTime.Parse("01-01-2020 12:00:00")),
                new LogMessage("Warning",
                               LogMessage.LogMessageType.Warning,
                               1,
                               DateTime.Parse("01-01-2020 13:00:00")),
            };

            var actual = DataUtilities.GetLogMessages(0);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetLogMessage_GetsLogMessagesWithTooLowExecutionIdAndNewerMinDateThanNewestLogMessage_ThrowsArgumentOutOfRangeException()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();

            Assert.Throws<ArgumentOutOfRangeException>(() => DataUtilities.GetLogMessages(-1, DateTime.Parse("02-01-2020 12:00:00")));
        }
        #endregion
        #region GetManager
        [Fact]
        public void GetManager_GetsManagersFromTestDatabase_ReturnTrue()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            var expected = new List<Manager>()
            {
                new Manager(1, 1, "ManagerOne"),
                new Manager(2, 1, "ManagerTwo"),
                new Manager(3, 1, "ManagerThree"),
            };

            var actual = DataUtilities.GetManagers();

            Assert.Equal(expected, actual);
        }

        [Fact] /* This test should be chacked later dosen't care about minDate*/
        public void TODO_DoesFail_GetManager_GetsManagersFromTestDatabaseWhereMinDateIsGreaterThanNewestManager_ReturnEmpty()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();

            var actual = DataUtilities.GetManagers(DateTime.Parse("02-01-2020 12:00:00"));

            Assert.NotEmpty(actual);
        }
        #endregion
        #region AddManager
        [Fact]
        public void AddManager_AddsManagerFromExecutionToManagerList_ReturnTrue()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            var expected = new List<Manager>()
            {
                new Manager(1, 1, "ManagerOne"),
                new Manager(2, 1, "ManagerTwo"),
                new Manager(3, 1, "ManagerThree"),
            };

            var executionList = DataUtilities.GetExecutions();
            DataUtilities.AddManagers(executionList);

            var actual = DataUtilities.GetManagers();

            Assert.Equal(expected, actual);
        }
        #endregion
        #region AddHealthReport
        [Fact]
        public void AddHealthReportReadings_GetsReadingsFromTheHealthReport_ReturnTrue()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            HealthReport expected = new(string.Empty,
                                        string.Empty,
                                        new Cpu(string.Empty, 69, 420),
                                        new Network(string.Empty, string.Empty, null),
                                        new Ram(100));

            expected.Cpu.Readings = new List<CpuLoad>
            {
                new(1, 0.1, DateTime.Parse("01-01-2020 12:00:00")),
                new(1, 0.2, DateTime.Parse("01-01-2020 13:00:00"))
            };

            expected.Ram.Readings = new List<RamLoad>
            {
                new(1, 0.9, 10, DateTime.Parse("01-01-2020 12:00:00")),
                new(1, 0.8, 20, DateTime.Parse("01-01-2020 13:00:00"))
            };

            expected.Network.Readings = new List<NetworkUsage>
            {
                new(1, 10, 10, 10, 10, 10, 10,DateTime.Parse("01-01-2020 12:00:00")),
                new(1, 20, 20, 20, 20, 20, 20,DateTime.Parse("01-01-2020 13:00:00"))
            };

            HealthReport actual = new(string.Empty,
                                      string.Empty,
                                      new Cpu(string.Empty, 69, 420),
                                      new Network(string.Empty, string.Empty, null),
                                      new Ram(100));

            DataUtilities.AddHealthReportReadings(actual);


            Assert.Equal(expected, actual);
        }
        #endregion
        #region AddManagerReadings
        [Fact]
        public void AddManagerReadings_GetReadingsForManagersAndAssignsThemToManager_ReturnTrue()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();

            var expected = new Execution(1, DateTime.Parse("01-01-2020 12:00:00"));
            expected.Managers.Add(new Manager(1, 1, "managerone"));
            expected.Managers.Add(new Manager(2, 1, "managertwo"));
            expected.Managers.Add(new Manager(3, 1, "managerthree"));

            var actual = new Execution(1, DateTime.Parse("01-01-2020 12:00:00"));

            DataUtilities.AddManagers(new List<Execution>() { actual });
            DataUtilities.AddManagerReadings(actual);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddManagerReadings_GetReadingsForManagersWhereReadingsAreNotAssignedToManagersSinceMinDateIsNewerThanNewestManagerReading_DoesNotAddAnyReadings()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();

            var expected = new Execution(1, DateTime.Parse("01-01-2020 12:00:00"));
            expected.Managers.Add(new Manager(1, 1, "managerone"));
            expected.Managers.Add(new Manager(2, 1, "managertwo"));
            expected.Managers.Add(new Manager(3, 1, "managerthree"));

            var actual = new Execution(1, DateTime.Parse("01-01-2020 12:00:00"));

            DataUtilities.AddManagers(new List<Execution>() { actual });
            DataUtilities.AddManagerReadings(actual, DateTime.Parse("02-01-2020 12:00:00"));

            Assert.Equal(expected, actual);
        }

        #endregion
        #region BuildHealthReport
        [Fact]
        public void BuildHealthReport_BuildsAHealthReport_ReturnTrue()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            HealthReport expected = new(string.Empty,
                                        string.Empty,
                                        new Cpu("CPU", 100, null),
                                        new Network("Interface", "MAC address", null),
                                        new Ram(null));

            HealthReport actual = new();

            DataUtilities.BuildHealthReport(actual);


            Assert.Equal(expected, actual);
        }
        #endregion
    }
}