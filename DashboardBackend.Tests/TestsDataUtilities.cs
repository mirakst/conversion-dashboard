using DashboardBackend.Database.Models;
using DashboardBackend.Tests.Database;
using Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace DashboardBackend.Tests
{
    public class TestsDataUtilities
    {
        [Fact]
        public void TestGetExecutions_True()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            var expected = new List<Execution>()
            {
                new Execution(1, DateTime.Parse("01/01/2020 12:00:00")),
                new Execution(2, DateTime.Parse("01/01/2020 13:00:00")),
            };

            var actual = DataUtilities.GetExecutions();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetAfstemninger_True()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            var expected = new List<ValidationTest>()
            {
                new ValidationTest(DateTime.Parse("01/01/2020 12:00:00"),
                                   "validationOne",
                                   ValidationTest.ValidationStatus.Ok,
                                   "managerOne",
                                   0,
                                   0,
                                   0,
                                   "srcSql",
                                   "dstSql"),
                new ValidationTest(DateTime.Parse("01/01/2020 13:00:00"),
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
        public void TestGetLogMessages_True()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            var expected = new List<LogMessage>()
            {
                new LogMessage("Afstemning Error",
                               LogMessage.LogMessageType.Validation | LogMessage.LogMessageType.Error,
                               0,
                               DateTime.Parse("01/01/2020 17:00:00")),
                new LogMessage("Check - Error",
                               LogMessage.LogMessageType.Validation | LogMessage.LogMessageType.Error,
                               0,
                               DateTime.Parse("01/01/2020 18:00:00")),
                new LogMessage("Info",
                               LogMessage.LogMessageType.Info,
                               0,
                               DateTime.Parse("01/01/2020 12:00:00")),
                new LogMessage("Warning",
                               LogMessage.LogMessageType.Warning,
                               0,
                               DateTime.Parse("01/01/2020 13:00:00")),
                new LogMessage("Error",
                               LogMessage.LogMessageType.Error,
                               0,
                               DateTime.Parse("01/01/2020 14:00:00")),
                new LogMessage("Fatal",
                               LogMessage.LogMessageType.Fatal,
                               0,
                               DateTime.Parse("01/01/2020 15:00:00")),
                new LogMessage("None",
                               LogMessage.LogMessageType.None,
                               0,
                               DateTime.Parse("01/01/2020 16:00:00")),
            };

            var actual = DataUtilities.GetLogMessages();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetLogMessagesWExecutionID_True()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            var expected = new List<LogMessage>()
            {
                new LogMessage("Info",
                               LogMessage.LogMessageType.Info,
                               0,
                               DateTime.Parse("01/01/2020 12:00:00")),
                new LogMessage("Warning",
                               LogMessage.LogMessageType.Warning,
                               1,
                               DateTime.Parse("01/01/2020 13:00:00")),
            };

            var actual = DataUtilities.GetLogMessages(0);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetLogMessagesWExecutionID_Throws()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();

            Assert.Throws<ArgumentOutOfRangeException>(() => DataUtilities.GetLogMessages(-1));
        }

        [Fact]
        public void TestGetManagers_True()
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

        [Fact]
        public void TestAddManagers_True()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            var expected = new List<Manager>()
            {
                new Manager(1, 1, "ManagerOne"),
                new Manager(2, 1, "ManagerTwo"),
                new Manager(3, 1, "ManagerThree"),
            };

            var executinList = DataUtilities.GetExecutions();
            DataUtilities.AddManagers(executinList);

            var actual = DataUtilities.GetManagers();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestAddHealthReport_True()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            HealthReport expected = new(string.Empty,
                                        string.Empty,
                                        new Cpu(string.Empty, 69, 420),
                                        new Network(string.Empty, string.Empty, null),
                                        new Ram(100));

            expected.Cpu.Readings = new List<CpuLoad>
            {
                new(1, 0.1, DateTime.Parse("01/01/2020 12:00:00")),
                new(1, 0.2, DateTime.Parse("01/01/2020 13:00:00"))
            };

            expected.Ram.Readings = new List<RamLoad>
            {
                new(1, 0.9, 10, DateTime.Parse("01/01/2020 12:00:00")),
                new(1, 0.8, 20, DateTime.Parse("01/01/2020 13:00:00"))
            };

            expected.Network.Readings = new List<NetworkUsage>
            {
                new(1, 10, 10, 10, 10, 10, 10,DateTime.Parse("01/01/2020 12:00:00")),
                new(1, 20, 20, 20, 20, 20, 20,DateTime.Parse("01/01/2020 13:00:00"))
            };

            HealthReport actual = new(string.Empty,
                                      string.Empty,
                                      new Cpu(string.Empty, 69, 420),
                                      new Network(string.Empty, string.Empty, null),
                                      new Ram(100));

            DataUtilities.AddHealthReportReadings(actual);


            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestBuildHealthReport_True()
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

        [Fact]
        public void TestAddManagerReadings_True()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            var expected = new Execution(1, DateTime.Parse("01/01/2020 12:00:00"));
            expected.Managers.Add(new Manager(1, 1, "managerone"));
            expected.Managers.Add(new Manager(2, 1, "managertwo"));
            expected.Managers.Add(new Manager(3, 1, "managerthree"));

            var actual = new Execution(1, DateTime.Parse("01/01/2020 12:00:00"));

            DataUtilities.AddManagers(new List<Execution>() { actual });
            DataUtilities.AddManagerReadings(actual);

            Assert.Equal(expected, actual);
        }
    }
}