using System;
using Xunit;

namespace Model.Tests
{
    public class TestManager
    {
        [Fact]
        public void Equal_DiffrentManagerWithSameParametersIdExecutionIdAndName_ReturnsTrue()
        {
            var expected = new Manager(0, 1, "managerOne");
            var actual = new Manager(0, 1, "managerOne");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equal_DiffrentManagerWithSameParametersIdExecutionIdAndNameStartTime_ReturnsTrue()
        {
            var expected = new Manager(0, 1, "managerOne", DateTime.Parse("01-01-2020 12:00:00"));
            var actual = new Manager(0, 1, "managerOne", DateTime.Parse("01-01-2020 12:00:00"));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equal_DiffrentManagerWithSameParametersIdExecutionIdAndNameStartTimeEndTime_ReturnsTrue()
        {
            var expected = new Manager(0, 1, "managerOne", DateTime.Parse("01-01-2020 12:00:00"), DateTime.Parse("01-01-2020 13:00:00"));
            var actual = new Manager(0, 1, "managerOne", DateTime.Parse("01-01-2020 12:00:00"), DateTime.Parse("01-01-2020 13:00:00"));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_SameManager_ReturnsTrue()
        {
            var manager = new Manager(0, 1, "managerOne");
            var expected = $"MANAGER ID: 0\nMANAGER EXECUTION ID: 1\nMANAGER NAME: managerOne\n" +
                           $"START TIME: 01-01-0001 00:00:00\nEND TIME: 01-01-0001 00:00:00\nRUNTIME: 00:00:00\nROWS READ: 0\n" +
                           $"ROWS WRITTEN: 0\n";

            var actual = manager.ToString();

            Assert.Equal(expected, actual);
        }
    }
}