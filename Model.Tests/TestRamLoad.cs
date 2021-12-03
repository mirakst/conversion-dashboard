using System;
using Xunit;

namespace Model.Tests
{
    public class TestRamLoad
    {
        [Fact]
        public void Equals_DiffrentRamLoadWithSameParameters_ReturnTrue()
        {
            var expected = new RamLoad(1, 0.5, 50, DateTime.Parse("01-01/2020 12:00:00"));
            var actual = new RamLoad(1, 0.5, 50, DateTime.Parse("01-01/2020 12:00:00"));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_SameRamLoad_ReturnsTrue()
        {
            var ramLoad = new RamLoad(1, 0.5, 50, DateTime.Parse("01-01/2020 12:00:00"));
            var expected = "01-01-2020 12:00:00: 50,00 % bytes";

            var actual = ramLoad.ToString();

            Assert.Equal(expected, actual);
        }
    }
}