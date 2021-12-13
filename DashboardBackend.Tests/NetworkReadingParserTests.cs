using System;
using System.Collections.Generic;
using System.Linq;
using DashboardBackend.Database.Models;
using DashboardBackend.Parsers;
using Model;
using Xunit;

namespace DashboardBackend.Tests
{
    public class NetworkReadingParserTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Parse_OneDistinctSet_GetsCorrectValuesIndependentOfOrdering(int seed)
        {
            NetworkReadingParser parser = new();
            List<HealthReportEntry> input = new()
            {
                new()
                {
                    ExecutionId = 1,
                    LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Received",
                    ReportNumericValue = 10000000000,
                },
                new()
                {
                    ExecutionId = 1,
                    LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Send",
                    ReportNumericValue = 20000000000,
                },
                new()
                {
                    ExecutionId = 1,
                    LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Received (Delta)",
                    ReportNumericValue = 10000,
                },
                new()
                {
                    ExecutionId = 1,
                    LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Send (Delta)",
                    ReportNumericValue = 20000,
                },
                new()
                {
                    ExecutionId = 1,
                    LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Received (Speed)",
                    ReportNumericValue = 1000,
                },
                new()
                {
                    ExecutionId = 1,
                    LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Send (Speed)",
                    ReportNumericValue = 2000,
                },
            };
            // Shuffle the list - the parser should work regardless of the ordering
            var rnd = new Random(seed);
            input = input.OrderBy(item => rnd.Next()).ToList();

            var result = parser.Parse(input);

            Assert.IsType<List<NetworkUsage>>(result);
            var reading = Assert.Single(result);
            Assert.NotNull(reading);
            Assert.Equal(10000000000, reading.BytesReceived);
            Assert.Equal(20000000000, reading.BytesSend);
            Assert.Equal(10000, reading.BytesReceivedDelta);
            Assert.Equal(20000, reading.BytesSendDelta);
            Assert.Equal(1000, reading.BytesReceivedSpeed);
            Assert.Equal(2000, reading.BytesSendSpeed);
        }
    }
}