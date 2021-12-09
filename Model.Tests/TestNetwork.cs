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
    }
}