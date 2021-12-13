using System;
using DashboardBackend.Database;
using DashboardBackend.Database.Models;
using DashboardFrontend;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Model;
using Xunit;

namespace IntegrationTests
{
    ////// NOTE: These tests require setup of a test database and a connectionstring in UserSecrets
    /// The database can be setup by executing the SQL script 'create_testdb.sql' in /Scripts.
    /// UserSecrets can be setup by right-clicking this project and selecting 'Manage User Secrets'
    /// - It should follow the template below:
    ///  {
    ///     "ConnectionStrings": {
    ///         "testdb": "CONNECTION_STRING"
    ///     }
    ///  }
    //////

    public class DashboardControllerTests
    {
        private readonly string _testConnectionString;
        private readonly DashboardController _controllerSeed;
        private readonly DbContextOptions<NetcompanyDbContext> _options;

        public DashboardControllerTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddUserSecrets<DashboardControllerTests>()
                .Build();
            _testConnectionString = config.GetConnectionString("testdb");
            _options = new DbContextOptionsBuilder<NetcompanyDbContext>()
                .UseSqlServer(_testConnectionString)
                .Options;

            _controllerSeed = new DashboardController();
            _controllerSeed.UserSettings = new FakeUserSettings();
        }

        [Fact]
        public void UpdateLog_NoExistingData_AddsExecutionsAndManagers()
        {
            var controller = _controllerSeed;
            controller.DataHandler.Database = new EntityFrameworkDb(_options);

            controller.UpdateLog();

            Assert.Collection(controller.Conversion!.Executions,
                item => Assert.Equal(1, item.Id),
                item => Assert.Equal(2, item.Id));
            var exec1 = controller.Conversion.Executions[0];
            Assert.Collection(exec1.Log.Messages,
                item => Assert.Equal(1, item.ExecutionId),
                item => Assert.Equal(1, item.ExecutionId),
                item => Assert.Equal(1, item.ExecutionId),
                item => Assert.Equal(1, item.ExecutionId));
            var manager = Assert.Single(exec1.Managers);
            Assert.NotNull(manager);
            Assert.Equal(1, manager.ContextId);
            Assert.Equal("test.manager", manager.Name);
            var exec2 = controller.Conversion.Executions[1];
            Assert.Collection(exec2.Log.Messages,
                item => Assert.Equal(2, item.ExecutionId));
            Assert.Empty(exec2.Managers);
        }

        [Fact]
        public void UpdateExecutions_GotExecutionFromLog_DoesNotAddDuplicate()
        {
            var controller = _controllerSeed;
            controller.DataHandler.Database = new EntityFrameworkDb(_options);
            controller.UpdateLog();

            controller.UpdateExecutions();

            Assert.Collection(controller.Conversion!.Executions,
                item => Assert.Equal(1, item.Id),
                item => Assert.Equal(2, item.Id));
        }

        [Fact]
        public void UpdateExecutions_NoExistingData_AddsExecution()
        {
            var controller = _controllerSeed;
            controller.DataHandler.Database = new EntityFrameworkDb(_options);

            controller.UpdateExecutions();

            var execution = Assert.Single(controller.Conversion!.Executions);
            Assert.Equal(1, execution.Id);
        }

        [Fact]
        public void UpdateLog_OneExecutionExists_DoesNotAddDuplicate()
        {
            var controller = _controllerSeed;
            controller.DataHandler.Database = new EntityFrameworkDb(_options);
            controller.UpdateExecutions();

            controller.UpdateLog();

            Assert.Collection(controller.Conversion!.Executions,
                item => Assert.Equal(1, item.Id),
                item => Assert.Equal(2, item.Id));
        }

        [Fact]
        public void UpdateManagers_NoManagersExist_AddsManagers()
        {
            var controller = _controllerSeed;
            controller.DataHandler.Database = new EntityFrameworkDb(_options);
            controller.UpdateExecutions();

            controller.UpdateManagers();

            var execution = controller.Conversion!.Executions[0];
            var manager = Assert.Single(execution.Managers);
            Assert.NotNull(manager);
            Assert.Equal(0, manager.ContextId);
            Assert.Equal("test.manager", manager.Name);
            Assert.Equal(DateTime.Parse("2010-01-01 12:00:01"), manager.StartTime);
            Assert.Equal(DateTime.Parse("2010-01-01 12:00:02"), manager.EndTime);
            Assert.Equal(10, manager.RowsRead);
            Assert.Equal(20, manager.RowsWritten);
        }

        [Fact]
        public void UpdateManagers_ManagerExists_UpdatesManagerWithNoDuplicate()
        {
            var controller = _controllerSeed;
            controller.DataHandler.Database = new EntityFrameworkDb(_options);
            controller.UpdateLog();

            controller.UpdateManagers();

            var execution = controller.Conversion!.Executions[0];
            var manager = Assert.Single(execution.Managers);
            Assert.NotNull(manager);
            Assert.Equal(1, manager.ContextId);
            Assert.Equal("test.manager", manager.Name);
            Assert.Equal(DateTime.Parse("2010-01-01 12:00:01"), manager.StartTime);
            Assert.Equal(DateTime.Parse("2010-01-01 12:00:02"), manager.EndTime);
            Assert.Equal(10, manager.RowsRead);
            Assert.Equal(20, manager.RowsWritten);
        }

        [Fact]
        public void UpdateHealthReport_NoExistingData_AddsAllValues()
        {
            var controller = _controllerSeed;
            controller.DataHandler.Database = new EntityFrameworkDb(_options);

            controller.UpdateHealthReport();

            var healthReport = controller.Conversion!.HealthReport;
            Assert.NotNull(healthReport);
            Assert.Equal("test.host", healthReport.HostName);
            Assert.Equal("test.monitor", healthReport.MonitorName);
            // CPU
            Assert.NotNull(healthReport.Cpu);
            Assert.Equal("test.cpu", healthReport.Cpu.Name);
            Assert.Equal(4, healthReport.Cpu.Cores);
            Assert.Equal(1000000000, healthReport.Cpu.MaxFrequency);
            Assert.Collection(healthReport.Cpu.Readings,
                item => Assert.Equal(0.05, item.Load));
            // RAM
            Assert.NotNull(healthReport.Ram);
            Assert.NotNull(healthReport.Ram.Total);
            Assert.Equal(1000000000, healthReport.Ram.Total);
            Assert.Collection(healthReport.Ram.Readings,
                item => Assert.Equal(0.50, item.Load));
            // Network
            Assert.NotNull(healthReport.Network);
            Assert.Equal("test.network", healthReport.Network.Name);
            Assert.Equal("test.macaddress", healthReport.Network.MacAddress);
            Assert.Equal(1000000000, healthReport.Network.Speed);
            var reading = Assert.Single(healthReport.Network.Readings);
            Assert.Equal(1000000000, reading.BytesReceived);
            Assert.Equal(1000000000, reading.BytesSend);
            Assert.Equal(1000000000, reading.BytesReceivedDelta);
            Assert.Equal(1000000000, reading.BytesSendDelta);
            Assert.Equal(1, reading.BytesReceivedSpeed);
            Assert.Equal(1, reading.BytesSendSpeed);
            Assert.True(reading.Date != default);
        }

        [Fact]
        public void UpdateHealthReport_HasCpuReadingsButCpuChanges_JoinsReadingsAndUpdatesValues()
        {
            var controller = _controllerSeed;
            controller.DataHandler.Database = new EntityFrameworkDb(_options);
            Cpu cpu = new()
            {
                Name = "first.test.cpu",
                Cores = 2,
                MaxFrequency = 10,
                Readings = new()
                {
                    new CpuLoad(1, 0.5, DateTime.Parse("2010-01-01 12:00:00"))
                }
            };
            controller.Conversion!.HealthReport.Cpu = cpu;

            controller.UpdateHealthReport();

            var result = controller.Conversion.HealthReport.Cpu;
            Assert.NotNull(result);
            Assert.Equal("test.cpu", result.Name);
            Assert.Equal(4, result.Cores);
            Assert.Equal(1000000000, result.MaxFrequency);
            Assert.Collection(result.Readings,
                item => Assert.Equal(0.5, item.Load),
                item => Assert.Equal(0.05, item.Load));
        }

        [Fact]
        public void UpdateHealthReport_HasRamReadingsButRamChanges_JoinsReadingsAndMaintainsPercentage()
        {
            var controller = _controllerSeed;
            controller.DataHandler.Database = new EntityFrameworkDb(_options);
            Ram ram = new()
            {
                Total = 10,
                Readings = new()
                {
                    new RamLoad()
                    {
                        ExecutionId = 1,
                        Available = 5,
                        Load = 0.5,
                        Date = DateTime.Parse("2010-01-01 12:00:00")
                    }
                }
            };
            controller.Conversion!.HealthReport.Ram = ram;

            controller.UpdateHealthReport();

            var result = controller.Conversion.HealthReport.Ram;
            Assert.NotNull(result);
            Assert.NotNull(result.Total);
            Assert.Equal(1000000000, result.Total);
            Assert.Collection(result.Readings,
                item => Assert.Equal(0.5, item.Load),
                item => Assert.Equal(0.5, item.Load));
        }
    }
}