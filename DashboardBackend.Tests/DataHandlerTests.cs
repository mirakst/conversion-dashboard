using DashboardBackend.Database.Models;
using DashboardBackend.Tests.Database;
using Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace DashboardBackend.Tests
{
    public class DataHandlerTests
    {
        #region GetExecution
        [Fact]
        public void GetExecution_GetsExecutionsFromTestDatabase_ReturnTrue()
        {
            var dataHandler = new DataHandler { Database = new TestDatabase() };
            var expected = new List<Execution>()
            {
                new Execution(1, DateTime.Parse("01-01-2020 12:00:00")),
                new Execution(2, DateTime.Parse("01-01-2020 13:00:00")),
            };

            var actual = dataHandler.GetExecutions(DateTime.Parse("01-01-2020 12:00:00"));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetExecution_GetsExecutionsFromTestDatabaseWithNewerMinDateThanLatestExecution_ReturnEmpty()
        {
            var dataHandler = new DataHandler { Database = new TestDatabase() };

            var actual = dataHandler.GetExecutions(DateTime.Parse("02-01-2020 12:00:00"));

            Assert.Empty(actual);
        }

        [Fact]
        public void GetExecution_GetsExecutionsFromTestDatabaseWhereExecutionsWithSameIdExists_ReturnExecutions()
        {
            var dataHandler = new DataHandler { Database = new TestDatabase() };
            var expected = 2;

            var actual = dataHandler.GetExecutions(DateTime.MinValue).FindAll(e => e.Id == 1);

            Assert.Equal(expected, actual.Count);
        }
        #endregion

        #region GetAfstemninger
        [Fact]
        public void GetValidations_GetsValidationsFromTestDatabase_ReturnsTrue()
        {
            var dataHandler = new DataHandler { Database = new TestDatabase() };
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

            var actual = dataHandler.GetValidations(DateTime.Parse("01-01-2020 12:00:00"));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValidations_GetsValidationsFromTestDatabaseWhereOneIsDuplicated_ReturnsTrue()
        {
            var dataHandler = new DataHandler { Database = new TestDatabase() };
            var expected = 2;

            var actual = dataHandler.GetValidations(DateTime.Parse("01-01-2020 10:00:00")).FindAll(a => a.Name == "validationOne").Count;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValidations_NoValidationsAfterSpecifiedMinDate_ReturnsEmpty()
        {
            var dataHandler = new DataHandler { Database = new TestDatabase() };

            var actual = dataHandler.GetValidations(DateTime.Parse("02-01-2020 12:00:00"));

            Assert.Empty(actual);
        }
        #endregion

        #region GetLogMessages
        [Fact]
        public void GetLogMessages_GetsLogMessagesFromTestDatabase_ReturnsTrue()
        {
            var dataHandler = new DataHandler { Database = new TestDatabase() };
            var expected = new List<LogMessage>()
            {
                new LogMessage("Afstemning Error",
                               LogMessage.LogMessageType.Validation | LogMessage.LogMessageType.Error,
                               0,
                               0,
                               DateTime.Parse("01-01-2020 17:00:00")),
                new LogMessage("Check - Error",
                               LogMessage.LogMessageType.Validation | LogMessage.LogMessageType.Error,
                               0,
                               0,
                               DateTime.Parse("01-01-2020 18:00:00")),
                new LogMessage("Info",
                               LogMessage.LogMessageType.Info,
                               0,
                               0,
                               DateTime.Parse("01-01-2020 12:00:00")),
                new LogMessage("Warning",
                               LogMessage.LogMessageType.Warning,
                               0,
                               0,
                               DateTime.Parse("01-01-2020 13:00:00")),
                new LogMessage("Error",
                               LogMessage.LogMessageType.Error,
                               0,
                               0,
                               DateTime.Parse("01-01-2020 14:00:00")),
                new LogMessage("Fatal",
                               LogMessage.LogMessageType.Fatal,
                               0,
                               0,
                               DateTime.Parse("01-01-2020 15:00:00")),
                new LogMessage("None",
                               LogMessage.LogMessageType.None,
                               0,
                               0,
                               DateTime.Parse("01-01-2020 16:00:00")),
            };

            var actual = dataHandler.GetLogMessages(DateTime.MinValue);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetLogMessage_NoLogMessagesAfterSpecifiedMinDate_ReturnsEmpty()
        {
            var dataHandler = new DataHandler { Database = new TestDatabase() };

            var actual = dataHandler.GetLogMessages(DateTime.Parse("02-01-2020 12:00:00"));

            Assert.Empty(actual);
        }
        #endregion

        [Fact]
        public void GetEstimatedManagerCount_GetsCountFromDatabase()
        {
            var dataHandler = new DataHandler { Database = new TestDatabase() };
            var expected = 2;

            var actual = dataHandler.GetEstimatedManagerCount(0);

            Assert.Equal(expected, actual);
        }

        #region GetManager
        [Fact]
        public void GetManagers_HasDataForThreeManagers_CreatesAllManagers()
        {
            var dataHandler = new DataHandler { Database = new TestDatabase() };

            var result = dataHandler.GetManagers(DateTime.Parse("01-01-2020 10:00:00"));

            var managers = Assert.IsType<List<Manager>>(result);
            Assert.Collection(managers,
                item => Assert.Equal("ManagerOne", item.Name),
                item => Assert.Equal("ManagerTwo", item.Name),
                item => Assert.Equal("ManagerThree", item.Name));
            var managerOne = managers[0];
            Assert.Equal(69420, managerOne.RowsRead);
        }

        [Fact]
        public void GetManagers_NoManagersAfterSpecifiedMinDate_ReturnsEmpty()
        {
            var dataHandler = new DataHandler { Database = new TestDatabase() };

            var actual = dataHandler.GetManagers(DateTime.Parse("02-01-2020 12:00:00"));

            Assert.Empty(actual);
        }
        #endregion

        #region GetParsedHealthReport
        [Fact]
        public void GetParsedHealthReport_GetsNewData_MaintainsPreviousReadings()
        {
            var dataHandler = new DataHandler { Database = new TestDatabase() };
            HealthReport healthReport = new()
            {
                HostName = "",
                MonitorName = "",
                Cpu = new()
                {
                    Readings =
                    {
                        new(1, 0.2, DateTime.Parse("01-01-2020 11:00:00"))
                    }
                },
                Ram = new(10),
                Network = new()
                {
                    Readings =
                    {
                        new(1, 5, 5, 5, 5, 5, 5, DateTime.Parse("01-01-2020 11:00:00"))
                    }
                }
            };
            healthReport.Ram.AddReading(new RamLoad(1, 5, DateTime.Parse("01-01-2020 11:00:00")));
            List<HealthReportEntry> entries = dataHandler.GetHealthReportEntries(DateTime.MinValue);

            healthReport = dataHandler.GetParsedHealthReport(entries, healthReport);

            Assert.NotNull(healthReport);
            Assert.Equal("Host 1", healthReport.HostName);
            Assert.Equal("Monitor 1", healthReport.MonitorName);
            Assert.Equal("CPU 1", healthReport.Cpu.Name);
            Assert.Collection(healthReport.Cpu.Readings,
                item => Assert.Equal(0.2, item.Load),
                item => Assert.Equal(0.1, item.Load),
                item => Assert.Equal(0.2, item.Load));
            Assert.NotNull(healthReport.Ram.Total);
            Assert.Equal(40000000000, healthReport.Ram.Total);
            Assert.Collection(healthReport.Ram.Readings,
                item => Assert.Equal(0.5, item.Load),
                item => Assert.Equal(0.5, item.Load),
                item => Assert.Equal(0.75, item.Load));
            Assert.Equal("Interface 1", healthReport.Network.Name);
            Assert.Collection(healthReport.Network.Readings,
                item => Assert.Equal(5, item.BytesSend),
                item => Assert.Equal(10, item.BytesSend),
                item => Assert.Equal(20, item.BytesSend));
        }

        [Fact]
        public void GetParsedHealthReport_HasAllDataFromDb_UpdatesValuesInHealthReport()
        {
            var dataHandler = new DataHandler { Database = new TestDatabase() };
            var entries = dataHandler.GetHealthReportEntries(DateTime.MinValue);
            
            var result = dataHandler.GetParsedHealthReport(entries, new HealthReport());

            Assert.NotNull(result);
            Assert.IsType<HealthReport>(result);
            Assert.Equal("Host 1", result.HostName);
            Assert.Equal("Monitor 1", result.MonitorName);

            Assert.Equal("CPU 1", result.Cpu.Name);
            Assert.Equal(100, result.Cpu.Cores);
            Assert.Equal(100, result.Cpu.MaxFrequency);
            Assert.NotEmpty(result.Cpu.Readings);

            Assert.NotNull(result.Ram.Total);
            Assert.Equal(40000000000, result.Ram.Total);
            Assert.NotEmpty(result.Ram.Readings);

            Assert.Equal("Interface 1", result.Network.Name);
            Assert.Equal("MAC address 1", result.Network.MacAddress);
            Assert.Equal(100, result.Network.Speed);
            Assert.NotEmpty(result.Network.Readings);
        }
        #endregion
    }
}