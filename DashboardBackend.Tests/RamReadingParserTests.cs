using DashboardBackend.Database.Models;
using DashboardBackend.Parsers;
using System.Collections.Generic;
using System;
using Xunit;
using Model;

namespace DashboardBackend.Tests
{
    public class RamReadingParserTests
    {
        [Fact]
        public void Parse_OneEntry_GetsExecIdAndAvailable()
        {
            var parser = new RamReadingParser();
            var input = new List<HealthReportEntry>
            {
                new()
                {
                    RowNo = 1,
                    MonitorNo = 1,
                    ExecutionId = 1,
                    ReportType = "RAM",
                    ReportKey = "AVAILABLE",
                    ReportNumericValue = 1000000000,
                    ReportValueType = "bytes",
                    ReportValueHuman = "1 GB",
                    LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                },
            };
            long expected = 1000000000;

            var readings = parser.Parse(input);

            Assert.IsType<List<RamLoad>>(readings);
            var reading = Assert.Single(readings);
            Assert.NotNull(reading);
            Assert.Equal(1, reading.ExecutionId);
            double actual = Assert.IsType<long>(reading.Available);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Parse_OneEntry_GetsExecIdAndDate()
        {
            var parser = new RamReadingParser();
            var input = new List<HealthReportEntry>
            {
                new()
                {
                    RowNo = 1,
                    MonitorNo = 1,
                    ExecutionId = 1,
                    ReportType = "RAM",
                    ReportKey = "AVAILABLE",
                    ReportNumericValue = 1000000000,
                    ReportValueType = "bytes",
                    ReportValueHuman = "1 GB",
                    LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                },
            };
            DateTime expected = DateTime.Parse("2010-01-01 12:00:00");

            var readings = parser.Parse(input);

            Assert.IsType<List<RamLoad>>(readings);
            var reading = Assert.Single(readings);
            Assert.NotNull(reading);
            Assert.Equal(1, reading.ExecutionId);
            DateTime actual = Assert.IsType<DateTime>(reading.Date);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Parse_TwoEntries_GetsExecIdAndDateAndLoad()
        {
            var parser = new RamReadingParser();
            var input = new List<HealthReportEntry>
            {
                new()
                {
                    RowNo = 1,
                    MonitorNo = 1,
                    ExecutionId = 1,
                    ReportType = "RAM",
                    ReportKey = "AVAILABLE",
                    ReportNumericValue = 1000000000,
                    ReportValueType = "bytes",
                    ReportValueHuman = "1 GB",
                    LogTime = System.DateTime.Parse("2010-01-01 12:00:00"),
                },
                new()
                {
                    RowNo = 1,
                    MonitorNo = 1,
                    ExecutionId = 2,
                    ReportType = "RA,",
                    ReportKey = "AVAILABLE",
                    ReportNumericValue = 2000000000,
                    ReportValueType = "bytes",
                    ReportValueHuman = "2 GB",
                    LogTime = System.DateTime.Parse("2010-01-01 12:00:30"),
                }
            };

            var readings = parser.Parse(input);

            Assert.IsType<List<RamLoad>>(readings);
            Assert.Collection(readings,
                item => Assert.Equal(1, item.ExecutionId),
                item => Assert.Equal(2, item.ExecutionId));
            Assert.Collection(readings,
                item => Assert.Equal(1000000000, item.Available),
                item => Assert.Equal(2000000000, item.Available));
            Assert.Collection(readings,
                item => Assert.Equal(DateTime.Parse("2010-01-01 12:00:00"), item.Date),
                item => Assert.Equal(DateTime.Parse("2010-01-01 12:00:30"), item.Date));
        }
    }
}