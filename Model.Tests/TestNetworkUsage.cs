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
    }
}