using System;
using Xunit;

namespace Model.Tests
{
    public class TestManager
    {
        [Fact]
        public void Equal_DiffrentManagerWithSameParametersIdExecutionIdAndName_ReturnsTrue()
        {
            var expected = new Manager()
            {
                Name = "managerOne",
                ContextId = 1
            };

            var actual = new Manager()
            {
                Name = "managerOne",
                ContextId = 1
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equal_DiffrentManagerWithSameParametersIdExecutionIdAndNameStartTime_ReturnsTrue()
        {
            var expected = new Manager()
            {
                Name = "managerOne",
                ContextId= 1,
                StartTime = DateTime.Parse("01-01-2020 12:00:00")
            };

            var actual = new Manager()
            {
                Name = "managerOne",
                ContextId= 1,
                StartTime = DateTime.Parse("01-01-2020 12:00:00")
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equal_DiffrentManagerWithSameParametersIdExecutionIdAndNameStartTimeEndTime_ReturnsTrue()
        {
            var expected = new Manager()
            {
                Name = "managerOne",
                ContextId = 1,
                StartTime = DateTime.Parse("01-01-2020 12:00:00"),
                EndTime = DateTime.Parse("01-01-2020 13:00:00"),
            };

            var actual = new Manager()
            {
                Name = "managerOne",
                ContextId = 1,
                StartTime = DateTime.Parse("01-01-2020 12:00:00"),
                EndTime = DateTime.Parse("01-01-2020 13:00:00"),
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_SameManager_ReturnsTrue()
        {
            var manager = new Manager()
            {
                Name = "managerOne",
                ContextId = 1
            };
            var expected = "Manager [managerOne], status [Ready]";

            var actual = manager.ToString();

            Assert.Equal(expected, actual);
        }
    }
}