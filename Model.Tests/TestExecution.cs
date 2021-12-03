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
            var expected = $"ID: 1\nSTART TIME: 01-01-2020 12:00:00\nEND TIME: 01-01-0001 00:00:00\n" +
                           $"RUNTIME: 737424.12:00:00\nROWS READ TOTAL: 0\n" +
                           $"STATUS: Finished\nMANAGERS: 0\n";

            var actual = execution.ToString();

            Assert.Equal(expected, actual);
        }
    }
}