using Xunit;

namespace Model.Tests
{
    public class TestCpu
    {
        [Fact]
        public void Equals_DifferentCpuWithSameNameCoresAndMaxFrequency_ReturnsTrue()
        {
            var expected = new Cpu("cpuName", 4, 1234567890);

            var actual = new Cpu("cpuName", 4, 1234567890);

            Assert.Equal(expected, actual);
        }
    }
}