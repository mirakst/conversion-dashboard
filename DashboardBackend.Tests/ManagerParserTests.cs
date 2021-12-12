using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DashboardBackend.Database.Models;
using Model;
using Xunit;

namespace DashboardBackend.Tests
{
    public class ManagerParserTests
    {
        public Conversion ConversionSeed
            => new Conversion().AddExecution(new(1, DateTime.MinValue));

        [Fact]
        public void Parse_CreatesSingleManagerAndUpdatesValues()
        {
            ManagerParser parser = new();
            List<EnginePropertyEntry> input = new()
            {
                new()
                {
                    Manager = "test.manager",
                    Key = "START_TIME",
                    Value = "2010-01-01 12:00:00.000",
                },
                new()
                {
                    Manager = "test.manager",
                    Key = "Læste rækker",
                    Value = "10",
                },
                new()
                {
                    Manager = "test.manager",
                    Key = "Skrevne rækker",
                    Value = "20",
                },
                new()
                {
                    Manager = "test.manager",
                    Key = "END_TIME",
                    Value = "2010-01-01 12:00:01.000",
                }

            };

            var managers = parser.Parse(input);

            var manager = Assert.Single(managers);
            Assert.Equal(0, manager.ContextId);
            Assert.Equal(DateTime.Parse("2010-01-01 12:00:00.000"), manager.StartTime);
            Assert.Equal(DateTime.Parse("2010-01-01 12:00:01.000"), manager.EndTime);
            Assert.Equal(10, manager.RowsRead);
            Assert.Equal(20, manager.RowsWritten);
            Assert.Equal("test.manager", manager.Name);
        }
    }
}
