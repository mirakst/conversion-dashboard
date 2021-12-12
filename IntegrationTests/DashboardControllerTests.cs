using System;
using DashboardBackend.Database;
using DashboardBackend.Database.Models;
using DashboardFrontend;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace IntegrationTests
{
    public class DashboardControllerTests
    {
        private readonly string _testConnectionString;
        private readonly IDashboardController _controllerSeed;

        public DashboardControllerTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddUserSecrets<DashboardControllerTests>()
                .Build();
            _testConnectionString = config.GetConnectionString("TestDb");

            _controllerSeed = new DashboardController(new FakeUserSettings());
        }

        [Fact]
        public void TryUpdateLog_NoExistingData_AddsExecutionsAndManagers()
        {
            var options = new DbContextOptionsBuilder<NetcompanyDbContext>()
                .UseSqlServer(_testConnectionString)
                .Options;
            var controller = _controllerSeed;
            controller.SetupNewConversion();
            controller.DataHandler.Database = new EntityFrameworkDatabase(options);

            controller.TryUpdateLog();

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
        public void TryUpdateExecutions_GotExecutionFromLog_DoesNotAddDuplicate()
        {
            var options = new DbContextOptionsBuilder<NetcompanyDbContext>()
                .UseSqlServer(_testConnectionString)
                .Options;
            var controller = _controllerSeed;
            controller.SetupNewConversion();
            controller.DataHandler.Database = new EntityFrameworkDatabase(options);
            controller.TryUpdateLog();

            controller.TryUpdateExecutions();

            Assert.Collection(controller.Conversion!.Executions,
                item => Assert.Equal(1, item.Id),
                item => Assert.Equal(2, item.Id));
        }

        [Fact]
        public void TryUpdateLog_OneExecutionExists_DoesNotAddDuplicate()
        {
            var options = new DbContextOptionsBuilder<NetcompanyDbContext>()
                .UseSqlServer(_testConnectionString)
                .Options;
            var controller = _controllerSeed;
            controller.SetupNewConversion();
            controller.DataHandler.Database = new EntityFrameworkDatabase(options);
            controller.TryUpdateExecutions();

            controller.TryUpdateLog();

            Assert.Collection(controller.Conversion!.Executions,
                item => Assert.Equal(1, item.Id),
                item => Assert.Equal(2, item.Id));
        }

        [Fact]
        public void TryUpdateManagers_NoManagersExist_AddsManagers()
        {
            var options = new DbContextOptionsBuilder<NetcompanyDbContext>()
                .UseSqlServer(_testConnectionString)
                .Options;
            var controller = _controllerSeed;
            controller.SetupNewConversion();
            controller.DataHandler.Database = new EntityFrameworkDatabase(options);
            controller.TryUpdateExecutions();

            controller.TryUpdateManagers();

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
        public void TryUpdateManagers_ManagerExists_UpdatesManagerWithNoDuplicate()
        {
            var options = new DbContextOptionsBuilder<NetcompanyDbContext>()
                .UseSqlServer(_testConnectionString)
                .Options;
            var controller = _controllerSeed;
            controller.SetupNewConversion();
            controller.DataHandler.Database = new EntityFrameworkDatabase(options);
            controller.TryUpdateLog();

            controller.TryUpdateManagers();

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
    }
}