using System;
using Xunit;

namespace Model.Tests
{
    public class TestCpuload
    {
        [Fact]
        public void Equals_DiffrentCpuLoadWithSameExecutionIdLoadAndDate_ReturnsTrue()
        {
            var expected = new CpuLoad(1, 0.5, DateTime.Parse("01-01-2020 12:00:00"));
            var actual = new CpuLoad(1, 0.5, DateTime.Parse("01-01-2020 12:00:00"));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_SameCpuLoad_ReturnsTrue()
        {
            var cpuLoad = new CpuLoad(1, 0.5, DateTime.Parse("01-01-2020 12:00:00"));
            var expected = "12:00:00: 0,5%";
            var actual = cpuLoad.ToString();

            Assert.Equal(expected, actual);
        }
    }
}