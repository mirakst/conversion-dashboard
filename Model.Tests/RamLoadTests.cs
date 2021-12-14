using System;
using Xunit;

namespace Model.Tests
{
    public class RamLoadTests
    {
        [Fact]
        public void Equals_DiffrentRamLoadWithSameParameters_ReturnTrue()
        {
            var expected = new RamLoad(1, 0.5, 50, DateTime.Parse("01-01/2020 12:00:00"));
            var actual = new RamLoad(1, 0.5, 50, DateTime.Parse("01-01/2020 12:00:00"));

            Assert.Equal(expected, actual);
        }
    }
}