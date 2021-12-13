using System;
using System.Collections.Generic;
using DashboardBackend.Database.Models;
using DashboardBackend.Parsers;
using Xunit;

namespace DashboardBackend.Tests
{
    public class CpuReadingParserTests
    {
        [Fact]
        public void Parse_OneEntry_GetsExecIdAndLoad()
        {
            var parser = new CpuReadingParser();
            var input = new List<HealthReportEntry>
            {
                new()
                {
                    RowNo = 1,
                    MonitorNo = 1,
                    ExecutionId = 1,
                    ReportType = "CPU",
                    ReportKey = "LOAD",
                    ReportNumericValue = 15,
                    ReportValueType = "%",
                    ReportValueHuman = "15 %",
                    LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                },
            };
            double expected = 0.15;

            var readings = parser.Parse(input);

            var reading = Assert.Single(readings);
            Assert.NotNull(reading);
            Assert.Equal(1, reading.ExecutionId);
            double actual = Assert.IsType<double>(reading.Load);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Parse_OneEntry_GetsExecIdAndDate()
        {
            var parser = new CpuReadingParser();
            var input = new List<HealthReportEntry>
            {
                new()
                {
                    RowNo = 1,
                    MonitorNo = 1,
                    ExecutionId = 1,
                    ReportType = "CPU",
                    ReportKey = "LOAD",
                    ReportNumericValue = 15,
                    ReportValueType = "%",
                    ReportValueHuman = "15 %",
                    LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                },
            };
            DateTime expected = DateTime.Parse("2010-01-01 12:00:00");

            var readings = parser.Parse(input);

            var reading = Assert.Single(readings);
            Assert.NotNull(reading);
            Assert.Equal(1, reading.ExecutionId);
            DateTime actual = Assert.IsType<DateTime>(reading.Date);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Parse_TwoEntries_GetsExecIdAndDateAndLoad()
        {
            var parser = new CpuReadingParser();
            var input = new List<HealthReportEntry>
            {
                new()
                {
                    RowNo = 1,
                    MonitorNo = 1,
                    ExecutionId = 1,
                    ReportType = "CPU",
                    ReportKey = "LOAD",
                    ReportNumericValue = 15,
                    ReportValueType = "%",
                    ReportValueHuman = "15 %",
                    LogTime = System.DateTime.Parse("2010-01-01 12:00:00"),
                },
                new()
                {
                    RowNo = 1,
                    MonitorNo = 1,
                    ExecutionId = 2,
                    ReportType = "CPU",
                    ReportKey = "LOAD",
                    ReportNumericValue = 20,
                    ReportValueType = "%",
                    ReportValueHuman = "20 %",
                    LogTime = System.DateTime.Parse("2010-01-01 12:00:30"),
                }
            };

            var readings = parser.Parse(input);

            Assert.Collection(readings,
                item => Assert.Equal(1, item.ExecutionId),
                item => Assert.Equal(2, item.ExecutionId));
            Assert.Collection(readings,
                item => Assert.Equal(0.15, item.Load),
                item => Assert.Equal(0.20, item.Load));
            Assert.Collection(readings,
                item => Assert.Equal(DateTime.Parse("2010-01-01 12:00:00"), item.Date),
                item => Assert.Equal(DateTime.Parse("2010-01-01 12:00:30"), item.Date));
        }
    }
}