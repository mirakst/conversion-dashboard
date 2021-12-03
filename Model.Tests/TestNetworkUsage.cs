using System;
using Xunit;

namespace Model.Tests
{
    public class TestNetworkUsage
    {
        [Fact]
        public void Equals_DiffrentNetworkUsage_ReturnTrue()
        {
            var expected = new NetworkUsage(1, 123, 123, 123, 123, 123, 123, DateTime.Parse("01-01-2020 12:00:00"));
            var actual = new NetworkUsage(1, 123, 123, 123, 123, 123, 123, DateTime.Parse("01-01-2020 12:00:00"));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_SameNetworkUsage_ReturnsTrue()
        {
            var networkUsage = new NetworkUsage(1, 123, 123, 123, 123, 123, 123, DateTime.Parse("01-01-2020 12:00:00"));
            var expected = $"BYTES SENT: 123 bytes\n" +
                           $"BYTES SENT (DELTA): 123 bytes\n" +
                           $"BYTES SENT (SPEED): 123 bps\n" +
                           $"BYTES RECEIVED: 123 bytes\n" +
                           $"BYTES RECEIVED (DELTA): 123 bytes\n" +
                           $"BYTES RECEIVED (SPEED) 123 bps";

            var actual = networkUsage.ToString();

            Assert.Equal(expected, actual);
        }
    }
}