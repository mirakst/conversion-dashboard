using System;
using Xunit;

namespace Model.Tests
{
    public class TestExecution
    {
        [Fact]
        public void Equals_DiffrentExecutionsWithSameIdAndStartTime_ReturnsTrue()
        {
            var expected = new Execution(1, DateTime.Parse("01-01-2020 12:00:00"));
            var actual = new Execution(1, DateTime.Parse("01-01-2020 12:00:00"));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_SameExecution_ReturnsTrue()
        {
            var execution = new Execution(1, DateTime.Parse("01-01-2020 12:00:00"));
            var expected = "Execution 1: Status=Started Start=01-01-2020 12:00:00 End=";

            var actual = execution.ToString();

            Assert.Equal(expected, actual);
        }
    }
}