using System;
using System.Collections.Generic;
using Xunit;

namespace Model.Tests
{
    public class LogTests
    {
        [Fact]
        public void Equals_DiffrentLogWithNoParameters_ReturnsTrue()
        {
            var expected = new Log();
            var actual = new Log();

            Assert.Equal(expected, actual);
        }
    }
}