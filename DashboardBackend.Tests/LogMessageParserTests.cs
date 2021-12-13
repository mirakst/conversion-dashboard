using System;
using System.Collections.Generic;
using DashboardBackend.Parsers;
using Model;
using Xunit;
using static Model.Execution;
using static Model.LogMessage;

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

        [Fact]
        public void Parse_NothingToParse_ReturnsEmptyLists()
        {
            LogMessageParser parser = new();
            List<LogMessage> input = new();

            var (managers, executions) = parser.Parse(input);

            Assert.Empty(managers);
            Assert.Empty(executions);
        }

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