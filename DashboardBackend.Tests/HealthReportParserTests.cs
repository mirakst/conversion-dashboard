using System;
using System.Collections.Generic;
using DashboardBackend.Database.Models;
using DashboardBackend.Parsers;
using Model;
using Xunit;

namespace DashboardBackend.Tests
{
    public class HealthReportParserTests
    {
        public List<HealthReportEntry> InitData1 => new()
        {
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "INIT",
                ReportKey = "Hostname",
                ReportStringValue = "Test-Host-1",
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "INIT",
                ReportKey = "Monitor Name",
                ReportStringValue = "Test-Monitor-1",
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "CPU_INIT",
                ReportKey = "CPU Name",
                ReportStringValue = "Test CPU 1",
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "CPU_INIT",
                ReportKey = "PhysicalCores",
                ReportNumericValue = 4,
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "CPU_INIT",
                ReportKey = "CPU Max frequency",
                ReportNumericValue = 1000000000,
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "MEMORY_INIT",
                ReportKey = "TOTAL",
                ReportNumericValue = 1000000000,
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "NETWORK_INIT",
                ReportKey = "Interface 0: Name",
                ReportStringValue = "Test Network 1",
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "NETWORK_INIT",
                ReportKey = "Interface 0: MAC address",
                ReportStringValue = "00:00:00:00:00:00"
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "NETWORK_INIT",
                ReportKey = "Interface 0: Speed",
                ReportNumericValue = 1000000000,
            },
        };
        public List<HealthReportEntry> InitData2 => new()
        {
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 2,
                ReportType = "INIT",
                ReportKey = "Hostname",
                ReportStringValue = "Test-Host-2",
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "INIT",
                ReportKey = "Monitor Name",
                ReportStringValue = "Test-Monitor-2",
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "CPU_INIT",
                ReportKey = "CPU Name",
                ReportStringValue = "Test CPU 2",
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "CPU_INIT",
                ReportKey = "PhysicalCores",
                ReportNumericValue = 8,
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "CPU_INIT",
                ReportKey = "CPU Max frequency",
                ReportNumericValue = 2000000000,
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "MEMORY_INIT",
                ReportKey = "TOTAL",
                ReportNumericValue = 2000000000,
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "NETWORK_INIT",
                ReportKey = "Interface 0: Name",
                ReportStringValue = "Test Network 2",
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "NETWORK_INIT",
                ReportKey = "Interface 0: MAC address",
                ReportStringValue = "11:11:11:11:11:11"
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "NETWORK_INIT",
                ReportKey = "Interface 0: Speed",
                ReportNumericValue = 2000000000,
            },
        };
        public List<HealthReportEntry> Readings1 => new()
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
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "MEMORY",
                ReportKey = "AVAILABLE",
                ReportNumericValue = 500000000,
                ReportValueType = "bytes",
                ReportValueHuman = "0.5 GB",
                LogTime = DateTime.Parse("2010-01-01 12:00:00"),
            },
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
        public List<HealthReportEntry> Readings2 => new()
        {
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 2,
                ReportType = "CPU",
                ReportKey = "LOAD",
                ReportNumericValue = 30,
                ReportValueType = "%",
                ReportValueHuman = "30 %",
                LogTime = DateTime.Parse("2010-01-01 12:00:00"),
            },
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 2,
                ReportType = "MEMORY",
                ReportKey = "AVAILABLE",
                ReportNumericValue = 1000000000,
                ReportValueType = "bytes",
                ReportValueHuman = "1.0 GB",
                LogTime = DateTime.Parse("2010-01-01 12:00:00"),
            },
            new()
            {
                ExecutionId = 1,
                LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                ReportType = "NETWORK",
                ReportKey = "Interface 0: Bytes Received",
                ReportNumericValue = 20000000000,
            },
            new()
            {
                ExecutionId = 1,
                LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                ReportType = "NETWORK",
                ReportKey = "Interface 0: Bytes Send",
                ReportNumericValue = 40000000000,
            },
            new()
            {
                ExecutionId = 1,
                LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                ReportType = "NETWORK",
                ReportKey = "Interface 0: Bytes Received (Delta)",
                ReportNumericValue = 20000,
            },
            new()
            {
                ExecutionId = 1,
                LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                ReportType = "NETWORK",
                ReportKey = "Interface 0: Bytes Send (Delta)",
                ReportNumericValue = 40000,
            },
            new()
            {
                ExecutionId = 1,
                LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                ReportType = "NETWORK",
                ReportKey = "Interface 0: Bytes Received (Speed)",
                ReportNumericValue = 2000,
            },
            new()
            {
                ExecutionId = 1,
                LogTime = DateTime.Parse("2010-01-01 12:00:00"),
                ReportType = "NETWORK",
                ReportKey = "Interface 0: Bytes Send (Speed)",
                ReportNumericValue = 4000,
            },
        };

        [Fact]
        public void Parse_TwoDifferentInits_GetsCorrectMemoryLoad()
        {
            List<HealthReportEntry> input = new();
            input.AddRange(InitData1);
            input.AddRange(Readings1);
            input.AddRange(InitData2);
            input.AddRange(Readings2);
            HealthReportParser parser = new();
            
            var result = parser.Parse(input);

            Assert.NotNull(result);
            var healthReport = Assert.IsType<HealthReport>(result);
            Assert.Equal("Test-Host-2", healthReport.HostName);
            Assert.NotNull(healthReport.Ram.Total);
            Assert.Equal(2000000000, healthReport.Ram.Total);
            Assert.Collection(healthReport.Ram.Readings,
                item => Assert.Equal(0.5, item.Load),
                item => Assert.Equal(0.5, item.Load));
            Assert.Collection(healthReport.Cpu.Readings,
                item => Assert.Equal(0.15, item.Load),
                item => Assert.Equal(0.30, item.Load));
            Assert.Collection(healthReport.Network.Readings,
                item => Assert.Equal(10000000000, item.BytesReceived),
                item => Assert.Equal(20000000000, item.BytesReceived));
        }

        [Fact]
        public void Parse_GetsHostData()
        {
            HealthReportParser parser = new();
            List<HealthReportEntry> input = InitData1;

            var result = parser.Parse(input);

            Assert.NotNull(result);
            var healthReport = Assert.IsType<HealthReport>(result);
            Assert.Equal("Test-Host-1", healthReport.HostName);
            Assert.Equal("Test-Monitor-1", healthReport.MonitorName);
        }

        [Fact]
        public void Parse_GetsCpuData()
        {
            HealthReportParser parser = new();
            List<HealthReportEntry> input = InitData1;

            var result = parser.Parse(input);

            Assert.NotNull(result);
            var healthReport = Assert.IsType<HealthReport>(result);
            Assert.Equal("Test CPU 1", healthReport.Cpu.Name);
            Assert.Equal(4, healthReport.Cpu.Cores);
            Assert.Equal(1000000000, healthReport.Cpu.MaxFrequency);
            Assert.Empty(healthReport.Cpu.Readings);
        }

        [Fact]
        public void Parse_GetsRamData()
        {
            HealthReportParser parser = new();
            List<HealthReportEntry> input = InitData1;

            var result = parser.Parse(input);

            Assert.NotNull(result);
            var healthReport = Assert.IsType<HealthReport>(result);
            Assert.NotNull(healthReport.Ram.Total);
            Assert.Equal(1000000000, healthReport.Ram.Total);
            Assert.Empty(healthReport.Ram.Readings);
        }

        [Fact]
        public void Parse_GetsNetworkData()
        {
            HealthReportParser parser = new();
            List<HealthReportEntry> input = InitData1;

            var result = parser.Parse(input);

            var healthReport = Assert.IsType<HealthReport>(result);
            Assert.Equal("Test Network 1", healthReport.Network.Name);
            Assert.Equal("00:00:00:00:00:00", healthReport.Network.MacAddress);
            Assert.Equal(1000000000, healthReport.Network.Speed);
            Assert.Empty(healthReport.Network.Readings);
        }

        [Fact]
        public void Parse_OneExecution_GetsAllData()
        {
            HealthReportParser parser = new();
            List<HealthReportEntry> input = new();
            input.AddRange(InitData1);
            input.AddRange(Readings1);

            var result = parser.Parse(input);

            Assert.NotNull(result);
            var healthReport = Assert.IsType<HealthReport>(result);
            // Host
            Assert.Equal("Test-Host-1", healthReport.HostName);
            Assert.Equal("Test-Monitor-1", healthReport.MonitorName);
            // CPU
            Assert.Equal("Test CPU 1", healthReport.Cpu.Name);
            Assert.Equal(4, healthReport.Cpu.Cores);
            Assert.Equal(1000000000, healthReport.Cpu.MaxFrequency);
            Assert.Single(healthReport.Cpu.Readings);
            // Memory
            Assert.NotNull(healthReport.Ram.Total);
            Assert.Equal(1000000000, healthReport.Ram.Total);
            Assert.Single(healthReport.Ram.Readings);
            // Network
            Assert.Equal("Test Network 1", healthReport.Network.Name);
            Assert.Equal("00:00:00:00:00:00", healthReport.Network.MacAddress);
            Assert.Equal(1000000000, healthReport.Network.Speed);
            Assert.Single(healthReport.Network.Readings);
        }

        [Fact]
        public void Parse_NoInitData_GetsReadings()
        {
            HealthReportParser parser = new();
            List<HealthReportEntry> input = new();
            input.AddRange(Readings1);

            var result = parser.Parse(input);

            Assert.NotNull(result);
            var healthReport = Assert.IsType<HealthReport>(result);
            Assert.NotEmpty(healthReport.Cpu.Readings);
            Assert.NotEmpty(healthReport.Ram.Readings);
            Assert.NotEmpty(healthReport.Network.Readings);
        }
    }
}