using System;
using System.Collections.Generic;
using DashboardBackend.Database.Models;
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
            List<LoggingEntry> input = new()
            {
                new()
                {
                    LogMessage = "Starting manager: manager.test.name",
                    LogLevel = "INFO",
                    ExecutionId = 1,
                    ContextId = 1,
                    Created = DateTime.MinValue
                }
            };

            var (messages, managers, executions) = parser.Parse(input);

            var message = Assert.Single(messages);
            Assert.Equal("Starting manager: manager.test.name", message.Content);
            Assert.True(message.Type.HasFlag(LogMessageType.Info));
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
            List<LoggingEntry> input = new()
            {
                new()
                {
                    LogMessage = "Test message",
                    LogLevel = "WARN",
                    ExecutionId = 1,
                    ContextId = 0,
                    Created = DateTime.MinValue
                }
            };

            var (messages, managers, executions) = parser.Parse(input);

            var message = Assert.Single(messages);
            Assert.True(message.Type.HasFlag(LogMessageType.Warning));
            Assert.Empty(managers);
            var execution = Assert.Single(executions);
            Assert.NotNull(execution);
            Assert.Equal(1, execution.Id);
        }

        [Fact]
        public void Parse_NothingToParse_ReturnsEmptyLists()
        {
            LogMessageParser parser = new();
            List<LoggingEntry> input = new();

            var (messages, managers, executions) = parser.Parse(input);

            Assert.Empty(messages);
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
            List<LoggingEntry> input = new()
            {
                new()
                {
                    LogMessage = inputContent,
                    LogLevel = "INFO",
                    ExecutionId = 1,
                    ContextId = 55,
                    Created = DateTime.MinValue
                }
            };

            var (messages, managers, executions) = parser.Parse(input);

            var message = Assert.Single(messages);
            Assert.True(message.Type.HasFlag(LogMessageType.Info));
            Assert.Empty(managers);
            var execution = Assert.Single(executions);
            Assert.NotNull(execution);
            Assert.Equal(1, execution.Id);
            Assert.Equal(ExecutionStatus.Finished, execution.Status);
        }

        [Fact]
        public void Parse_RemovesColorCoding()
        {
            LogMessageParser parser = new();
            List<LoggingEntry> input = new()
            {
                new()
                {
                    LogMessage = "[30;102mAfstemning - failed!!![0m",
                    LogLevel = "ERROR",
                    ExecutionId = 1,
                    ContextId = 5,
                    Created = DateTime.MinValue
                }
            };

            var (messages, managers, executions) = parser.Parse(input);

            var message = Assert.Single(messages);
            Assert.Equal("Afstemning - failed!!!", message.Content);
            Assert.True(message.Type.HasFlag(LogMessageType.Error));
            Assert.True(message.Type.HasFlag(LogMessageType.Validation));
            Assert.Empty(managers);
            Assert.Single(executions);
        }
    }
}