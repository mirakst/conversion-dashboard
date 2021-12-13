using Xunit;
using Model;
using System;

namespace FUCKING_ASS
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var FUUUUUUUUUUUUUUUUUUUUUUUUUUCK = new ManagerScore();
            var SHIIIIIIIIIIIIIIIIIIIIIIIIIIIIT = new Manager()
            {
                Runtime = TimeSpan.FromSeconds(10),
                RowsWritten = 10
            };
            var FFUUUCKCKCKCKCKCKfFUCKASCKAK = new Manager()
            {
                Runtime = TimeSpan.FromSeconds(100),
                RowsWritten = 10
            };
            FUUUUUUUUUUUUUUUUUUUUUUUUUUCK.GetPerformanceScore(SHIIIIIIIIIIIIIIIIIIIIIIIIIIIIT);
            var expected = 10;

            var actual = FUUUUUUUUUUUUUUUUUUUUUUUUUUCK.GetPerformanceScore(FFUUUCKCKCKCKCKCKfFUCKASCKAK);

            Assert.Equal(expected, actual);
        }
    }
}