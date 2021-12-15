using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DashboardBackend.Database.Models;
using DashboardBackend.Parsers;
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

        [Fact]
        public void Parse_TwoManagers_CreatesAndUpdatesBoth()
        {
            ManagerParser parser = new();
            List<EnginePropertyEntry> input = new()
            {
                new()
                {
                    Manager = "test.manager.1",
                    Key = "START_TIME",
                    Value = "2010-01-01 12:00:00",
                },
                new()
                {
                    Manager = "test.manager.1",
                    Key = "Læste rækker",
                    Value = "10",
                },
                new()
                {
                    Manager = "test.manager.1",
                    Key = "Skrevne rækker",
                    Value = "20",
                },
                new()
                {
                    Manager = "test.manager.1",
                    Key = "END_TIME",
                    Value = "2010-01-01 12:00:01",
                },
                new()
                {
                    Manager = "test.manager.2",
                    Key = "START_TIME",
                    Value = "2010-01-01 12:00:02",
                },
                new()
                {
                    Manager = "test.manager.2",
                    Key = "Læste rækker",
                    Value = "30",
                },
                new()
                {
                    Manager = "test.manager.2",
                    Key = "Skrevne rækker",
                    Value = "40",
                },
                new()
                {
                    Manager = "test.manager.2",
                    Key = "END_TIME",
                    Value = "2010-01-01 12:00:03",
                }
            };

            var managers = parser.Parse(input);

            Assert.Equal(2, managers.Count);
            var manager1 = Assert.IsType<Manager>(managers[0]);
            Assert.Equal(0, manager1.ContextId);
            Assert.Equal(DateTime.Parse("2010-01-01 12:00:00"), manager1.StartTime);
            Assert.Equal(DateTime.Parse("2010-01-01 12:00:01"), manager1.EndTime);
            Assert.Equal(10, manager1.RowsRead);
            Assert.Equal(20, manager1.RowsWritten);
            Assert.Equal("test.manager.1", manager1.Name);
            var manager2 = Assert.IsType<Manager>(managers[1]);
            Assert.Equal(0, manager2.ContextId);
            Assert.Equal(DateTime.Parse("2010-01-01 12:00:02"), manager2.StartTime);
            Assert.Equal(DateTime.Parse("2010-01-01 12:00:03"), manager2.EndTime);
            Assert.Equal(30, manager2.RowsRead);
            Assert.Equal(40, manager2.RowsWritten);
            Assert.Equal("test.manager.2", manager2.Name);
        }

        [Fact]
        public void Parse_NoRelevantData_ReturnsEmptyList()
        {
            ManagerParser parser = new();
            List<EnginePropertyEntry> input = new();

            var managers = parser.Parse(input);

            Assert.IsType<List<Manager>>(managers);
            Assert.Empty(managers);
        }
    }
}