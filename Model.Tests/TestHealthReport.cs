using System;
using Xunit;

namespace Model.Tests
{
    public class TestHealthReport
    {
        [Fact]
        public void Equals_DifrentHealthReportsNoParameters_ThrowsNullRefranceExceptionNoNetworkRamAndCpuReadings()
        {
            var expected = new HealthReport();
            var actual = new HealthReport();

            Assert.Throws<NullReferenceException>(() => expected.Equals(actual));
        }

        [Fact]
        public void Equals_DifrentHealthReportsWithSameHostNameAndMonitorName_ThrowsNullRefranceExceptionNoNetworkRamAndCpuReadings()
        {
            var expected = new HealthReport("Host Name", "Monitor Name");
            var actual = new HealthReport("Host Name", "Monitor Name");

            Assert.Throws<NullReferenceException>(() => expected.Equals(actual));
        }

        [Fact]
        public void Equals_DifferentHealthReportsWithSameHostNameMonitorNameCpuNetworkAndRam_ReturnsTrue()
        {
            var expected = new HealthReport("Host Name",
                                            "Monitor Name",
                                            new Cpu("Cpu Name", 4, 1234567890),
                                            new Network("Network Name", "MAC address", 1234567890),
                                            new Ram(1234567890));
            var actual = new HealthReport("Host Name",
                                          "Monitor Name",
                                          new Cpu("Cpu Name", 4, 1234567890),
                                          new Network("Network Name", "MAC address", 1234567890),
                                          new Ram(1234567890));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_SameHealthReportNoPerameters_ReturnsTrue()
        {
            var healthReport = new HealthReport();
            var expected = $"SYSTEM INFO:\nHOSTNAME: \nMONITOR NAME: \n"+
                           $"CPU: \nNETWORK: \nRAM: ";
            var actual = healthReport.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_SameHealthReportWithHostNameAndMonitorName_ReturnsTrue()
        {
            var healthReport = new HealthReport("Host Name", "Monitor Name");
            var expected = $"SYSTEM INFO:\nHOSTNAME: Host Name\nMONITOR NAME: Monitor Name\n"+
                           $"CPU: \nNETWORK: \nRAM: ";
            var actual = healthReport.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_SameHealthReportWithHostNameMonitorNameCpuNetworkAndRam_ReturnsTrue()
        {
            var healthReport = new HealthReport("Host Name",
                                            "Monitor Name",
                                            new Cpu("Cpu Name", 4, 1234567890),
                                            new Network("Network Name", "MAC address", 1234567890),
                                            new Ram(1234567890));

            var expected = $"SYSTEM INFO:\nHOSTNAME: Host Name\nMONITOR NAME: Monitor Name\n"+
                           $"CPU: CPU NAME: Cpu Name\nCPU CORES: 4\nCPU MAX FREQUENCY: 1234567890 Hz\n" +
                           $"NETWORK: ADAPTER NAME: Network Name\nMAC ADDRESS: MAC address\nSPEED: 1234567890 bps\n" +
                           $"RAM: TOTAL MEMORY: 1234567890 bytes";

            var actual = healthReport.ToString();

            Assert.Equal(expected, actual);
        }
    }
}