using System;
using Xunit;

namespace Model.Tests
{
    public class CpuLoadTests
    {
        [Fact]
        public void Equals_DiffrentCpuLoadWithSameExecutionIdLoadAndDate_ReturnsTrue()
        {
            var expected = new CpuLoad(1, 0.5, DateTime.Parse("01-01-2020 12:00:00"));
            var actual = new CpuLoad(1, 0.5, DateTime.Parse("01-01-2020 12:00:00"));

            Assert.Equal(expected, actual);
        }
    }
}