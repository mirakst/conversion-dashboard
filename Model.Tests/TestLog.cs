using System;
using System.Collections.Generic;
using Xunit;

namespace Model.Tests
{
    public class TestLog
    {
        [Fact]
        public void Equals_DiffrentLogWithNoParameters_ReturnsTrue()
        {
            var expected = new Log();
            var actual = new Log();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateCounters_SameLogWithNoParameters_ReturnsTrue()
        {
            var log = new Log();

            log.Messages = new List<LogMessage>()
            {
                new LogMessage("info", LogMessage.LogMessageType.Info, 1, DateTime.Parse("01-01-2020 12:00:00")),
                new LogMessage("warning", LogMessage.LogMessageType.Warning, 1, DateTime.Parse("01-01-2020 12:00:00")),
                new LogMessage("error", LogMessage.LogMessageType.Error, 1, DateTime.Parse("01-01-2020 12:00:00")),
                new LogMessage("fatal", LogMessage.LogMessageType.Fatal, 1, DateTime.Parse("01-01-2020 12:00:00")),
                new LogMessage("validation", LogMessage.LogMessageType.Validation, 1, DateTime.Parse("01-01-2020 12:00:00"))
            };

            var expected = new[] { 1, 1, 1, 1, 1 };
            var actual = new[] { log.InfoCount, log.WarnCount, log.ErrorCount, log.FatalCount, log.ValidationCount };

            Assert.Equal(expected, actual);
        }


    }
}