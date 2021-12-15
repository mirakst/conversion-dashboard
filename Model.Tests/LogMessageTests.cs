using System;
using Xunit;

namespace Model.Tests
{
    public class LogMessageTests
    {
        [Fact]
        public void Equals_DiffrentLogMessageWithSameParameters_ReturnsTrue()
        {
            var expected = new LogMessage("content", LogMessage.LogMessageType.Info, 1, 0, DateTime.Parse("01-01-2020 12:00:00"));
            var actual = new LogMessage("content", LogMessage.LogMessageType.Info, 1, 0, DateTime.Parse("01-01-2020 12:00:00"));

            Assert.Equal(expected, actual);
        }
    }
}