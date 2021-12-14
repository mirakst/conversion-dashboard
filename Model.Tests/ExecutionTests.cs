using System;
using Xunit;

namespace Model.Tests
{
    public class ExecutionTests
    {
        [Fact]
        public void Equals_DiffrentExecutionsWithSameIdAndStartTime_ReturnsTrue()
        {
            var expected = new Execution(1, DateTime.Parse("01-01-2020 12:00:00"));
            var actual = new Execution(1, DateTime.Parse("01-01-2020 12:00:00"));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddManager_AddsANewManagerToTheExecution_ReturnsTrue()
        {
            var expected = 1;

            var execution = new Execution(0, DateTime.Parse("01-01-2020 12:00:00"));
            var managerToAdd = new Manager() { Status = Manager.ManagerStatus.Ok};
            execution.AddManager(managerToAdd);

            var actual = execution.Managers.Count;

            Assert.True(expected == actual);
        }
    }
}