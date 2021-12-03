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

        [Fact]
        public void ToString_SameCpu_ReturnsTrue()
        {
            var Cpu = new Cpu("cpuName", 4, 1234567890);
            var expected = "CPU NAME: cpuName\nCPU CORES: 4\nCPU MAX FREQUENCY: 1234567890 Hz";
            var actual = Cpu.ToString();

            Assert.Equal(expected, actual);
        }
    }
}