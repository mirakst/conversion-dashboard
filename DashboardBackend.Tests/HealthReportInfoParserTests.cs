using System.Collections.Generic;
using DashboardBackend.Database.Models;
using DashboardBackend.Parsers;
using Model;
using Xunit;

namespace DashboardBackend.Tests
{
    public class HealthReportInfoParserTests
    {
        public List<HealthReportEntry> HostDataInput => new()
        {
            new HealthReportEntry()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "INIT",
                ReportKey = "Hostname",
                ReportStringValue = "Test-Host",
            },
            new HealthReportEntry()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "INIT",
                ReportKey = "Monitor Name",
                ReportStringValue = "Test-Monitor",
            },
        };
        public List<HealthReportEntry> CpuDataInput => new()
        {
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "CPU_INIT",
                ReportKey = "CPU Name",
                ReportStringValue = "Test CPU",
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
        };
        public List<HealthReportEntry> RamDataInput => new()
        {
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "MEMORY_INIT",
                ReportKey = "TOTAL",
                ReportNumericValue = 1000000000,
            }
        };
        public List<HealthReportEntry> NetworkDataInput => new()
        {
            new()
            {
                RowNo = 1,
                MonitorNo = 1,
                ExecutionId = 1,
                ReportType = "NETWORK_INIT",
                ReportKey = "Interface 0: Name",
                ReportStringValue = "Test Network",
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

        [Fact]
        public void Parse_GetsHostData()
        {
            HealthReportInfoParser parser = new();
            List<HealthReportEntry> input = HostDataInput;

            var result = parser.Parse(input);

            Assert.NotNull(result);
            var healthReport = Assert.IsType<HealthReport>(result);
            Assert.Equal("Test-Host", healthReport.HostName);
            Assert.Equal("Test-Monitor", healthReport.MonitorName);
        }

        [Fact]
        public void Parse_GetsCpuData()
        {
            HealthReportInfoParser parser = new();
            List<HealthReportEntry> input = CpuDataInput;

            var result = parser.Parse(input);

            Assert.NotNull(result);
            var healthReport = Assert.IsType<HealthReport>(result);
            Assert.Equal("Test CPU", healthReport.Cpu.Name);
            Assert.Equal(4, healthReport.Cpu.Cores);
            Assert.Equal(1000000000, healthReport.Cpu.MaxFrequency);
            Assert.Empty(healthReport.Cpu.Readings);
        }

        [Fact]
        public void Parse_GetsRamData()
        {
            HealthReportInfoParser parser = new();
            List<HealthReportEntry> input = RamDataInput;

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
            HealthReportInfoParser parser = new();
            List<HealthReportEntry> input = NetworkDataInput;

            var result = parser.Parse(input);

            var healthReport = Assert.IsType<HealthReport>(result);
            Assert.Equal("Test Network", healthReport.Network.Name);
            Assert.Equal("00:00:00:00:00:00", healthReport.Network.MacAddress);
            Assert.Equal(1000000000, healthReport.Network.Speed);
            Assert.Empty(healthReport.Network.Readings);
        }

        [Fact]
        public void Parse_GetsAllData()
        {
            HealthReportInfoParser parser = new();
            List<HealthReportEntry> input = new();
            input.AddRange(HostDataInput);
            input.AddRange(CpuDataInput);
            input.AddRange(RamDataInput);
            input.AddRange(NetworkDataInput);

            var result = parser.Parse(input);

            Assert.NotNull(result);
            var healthReport = Assert.IsType<HealthReport>(result);
            // Host
            Assert.Equal("Test-Host", healthReport.HostName);
            Assert.Equal("Test-Monitor", healthReport.MonitorName);
            // CPU
            Assert.Equal("Test CPU", healthReport.Cpu.Name);
            Assert.Equal(4, healthReport.Cpu.Cores);
            Assert.Equal(1000000000, healthReport.Cpu.MaxFrequency);
            Assert.Empty(healthReport.Cpu.Readings);
            // Memory
            Assert.NotNull(healthReport.Ram.Total);
            Assert.Equal(1000000000, healthReport.Ram.Total);
            Assert.Empty(healthReport.Ram.Readings);
            // Network
            Assert.Equal("Test Network", healthReport.Network.Name);
            Assert.Equal("00:00:00:00:00:00", healthReport.Network.MacAddress);
            Assert.Equal(1000000000, healthReport.Network.Speed);
            Assert.Empty(healthReport.Network.Readings);
        }
    }
}