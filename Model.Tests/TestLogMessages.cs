using System;
using Xunit;

namespace Model.Tests
{
    public class TestLogMessages
    {
        [Fact]
        public void Equals_DiffrentLogMessageWithSameParameters_ReturnsTrue()
        {
            var expected = new LogMessage("content", LogMessage.LogMessageType.Info, 1, DateTime.Parse("01-01-2020 12:00:00"));
            var actual = new LogMessage("content", LogMessage.LogMessageType.Info, 1, DateTime.Parse("01-01-2020 12:00:00"));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_SameLogmessage_ReturnsTrue()
        {
            var logMessage = new LogMessage("content", LogMessage.LogMessageType.Info, 1, DateTime.Parse("01-01-2020 12:00:00"));
            var expected = "01-01-2020 12:00:00 [Info]: content";

            var actual = logMessage.ToString();

            Assert.Equal(expected, actual);
        }
    }
}