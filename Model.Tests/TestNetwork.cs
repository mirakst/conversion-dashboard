using Xunit;

namespace Model.Tests
{
    public class TestNetwork
    {
        [Fact]
        public void Equals_DiffrentNetworkWithSameParameters_ReturnTrue()
        {
            var expected = new Network("Name", "MAC", 1234567890);
            var actual = new Network("Name", "MAC", 1234567890);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_SameNetwork_ReturnsTrue()
        {
            var network = new Network("Name", "MAC", 1234567890);
            var expected = $"ADAPTER NAME: Name\nMAC ADDRESS: MAC\nSPEED: 1234567890 bps";

            var actual = network.ToString();

            Assert.Equal(expected, actual);
        }
    }
}