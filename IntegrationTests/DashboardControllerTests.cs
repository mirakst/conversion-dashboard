using System;
using System.Collections.Generic;
using DashboardBackend;
using DashboardBackend.Database;
using DashboardBackend.Database.Models;
using DashboardFrontend;
using Microsoft.EntityFrameworkCore;
using Model;
using Xunit;

namespace IntegrationTests
{
    public class DashboardControllerTests
    {
        public IDashboardController ControllerSeed => new DashboardController(new FakeUserSettings(), new DatabaseHandler());

        [Fact]
        public void DashboardController_GetsExecutionAndManagerFromLOGGING()
        {
            // Setup database
            var options = new DbContextOptionsBuilder<NetcompanyDbContext>()
                .UseInMemoryDatabase("IntegrationTesting")
                .Options;
            using (var context = new NetcompanyDbContext(options))
            {
                var loggingData = new List<LoggingEntry>
                {
                    new()
                    {
                        Created = DateTime.Now,
                        LogMessage = "Starting execution",
                        LogLevel = "INFO",
                        ExecutionId = 1,
                        ContextId = 0,
                    },
                    new()
                    {
                        Created = DateTime.Now,
                        LogMessage = "Starting manager: manager.test.name",
                        LogLevel = "INFO",
                        ExecutionId = 1,
                        ContextId = 1,
                    },
                };
                context.Loggings.AddRange(loggingData);
                context.SaveChanges();
                context.Dispose();
            }

            // Setup test
            var controller = ControllerSeed;
            controller.DatabaseHandler.Database = new SqlDatabase(options);
            controller.SetupNewConversion();

            // Perform test
            controller.TryUpdateLog();

            // Assert
            var execution = Assert.Single(controller.Conversion!.Executions);
            Assert.NotNull(execution);
            Assert.Equal(1, execution.Id);
            var manager = Assert.Single(execution.Managers);
            Assert.NotNull(manager);
            Assert.Equal(1, manager.ContextId);
            Assert.Equal("manager.test.name", manager.Name);
        }
    }
}