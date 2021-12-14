using System;
using Xunit;

namespace Model.Tests
{
    public class TestManagerUsage
    {
        [Fact]
        public void Equals_DiffrentManagerUsageWithSameParameters_ReturnsTrue()
        {
            var expected = new ManagerUsage(1, 0, 0, DateTime.Parse("01-01-2020 12:00:00"));
            var actual = new ManagerUsage(1, 0, 0, DateTime.Parse("01-01-2020 12:00:00"));

            Assert.Equal(expected, actual);
        }
    }
}