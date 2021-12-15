using System;
using Xunit;

namespace Model.Tests
{
    public class HealthReportTests
    {
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
    }
}