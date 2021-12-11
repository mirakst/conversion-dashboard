using System;
using System.Collections.Generic;

using Model;

using Xunit;

namespace DashboardBackend.Tests
{
    public class LogMessageParserTests
    {
        public Conversion ConversionSeed 
            => new Conversion().AddExecution(new(1, DateTime.MinValue));

        [Fact]
        public void Parse_StartingNonExistingManager_CreatesManager()
        {
            LogMessageParser parser = new();
            List<LogMessage> input = new()
            {
                new("Starting manager: manager.test.name", LogMessageType.Info, 1, 1, DateTime.MinValue),
            };

            var (managers, executions) = parser.Parse(input);

            var execution = Assert.Single(executions);
            Assert.NotNull(execution);
            Assert.Equal(1, execution.Id);
            var manager = Assert.Single(managers);
            Assert.NotNull(manager);
            Assert.Equal(1, manager.ContextId);
            Assert.Equal("manager.test.name", manager.Name);
        }

        [Fact]
        public void Parse_ManagerExecutionDone_UpdatesManager()
        {
            LogMessageParser parser = new();
            List<LogMessage> input = new()
            {
                new("Manager execution done.", LogMessageType.Info, 1, 1, DateTime.MinValue),
            };

            var (managers, executions) = parser.Parse(input);

            var execution = Assert.Single(executions);
            Assert.Equal(1, execution.Id);
            var manager = Assert.Single(managers);
            Assert.Equal(1, manager.ContextId);
            Assert.Equal(ManagerStatus.Ok, manager.Status);
        }

        [Fact]
        public void Parse_NoExecutions_CreatesExecution()
        {
            LogMessageParser parser = new();
            List<LogMessage> input = new()
            {
                new("Test message", LogMessageType.Info, 0, 1, DateTime.MinValue),
            };

            var (managers, executions) = parser.Parse(input);

            Assert.Empty(managers);
            var execution = Assert.Single(executions);
            Assert.NotNull(execution);
            Assert.Equal(1, execution.Id);
        }

        // These could be static properties in the LogMessageParser class.
        [Theory]
        [InlineData("Program closing due to the following error:")]
        [InlineData("Exiting from GuiManager...")]
        [InlineData("No managers left to start automatically for BATCH")]
        [InlineData("Deploy is finished!!")]
        public void Parse_ExecutionFinished_ValidDomain(string inputContent)
        {
            LogMessageParser parser = new();
            List<LogMessage> input = new()
            {
                new(inputContent, LogMessageType.Info, 0, 1, DateTime.MinValue),
            };

            var (managers, executions) = parser.Parse(input);

            Assert.Empty(managers);
            var execution = Assert.Single(executions);
            Assert.NotNull(execution);
            Assert.Equal(1, execution.Id);
            Assert.Equal(ExecutionStatus.Finished, execution.Status);
        }
    }
}