using Xunit;
using Model;
using static Model.LogMessage;
using DashboardFrontend.ViewModels;
using System;
using System.Collections.Generic;

namespace DashboardFrontend.Tests
{
    public class LogViewModelTests
    {
        [Fact]
        public void UpdateData_NoPreviousData_AddsDataAndUpdatesSelectedExecution()
        {   
            //Arrange
            LogViewModel viewModel = new();
            Log log = new()
            {
                Messages =
                {
                    new LogMessage("Test", LogMessageType.Info, 1, 1, new DateTime(2021, 12, 4))
                }
            };
            List<Execution> executions = new()
            {
                new(1, DateTime.MinValue)
                {
                    Log = log
                }
            };

            //Act
            viewModel.UpdateData(executions);

            //Assert
            Assert.NotNull(viewModel.SelectedExecution);
            var msg = Assert.Single(viewModel.SelectedExecution!.LogMessages);
            Assert.Equal("Test", msg.Content);
            Assert.Equal(LogMessageType.Info, msg.Type);
            Assert.NotEmpty(viewModel.MessageView.SourceCollection);
        }

        [Fact]
        public void UpdateData_FilterInfoMessages_ViewIsEmpty()
        {
            //Arrange
            LogViewModel viewModel = new();
            Log log = new()
            {
                Messages =
                {
                    new LogMessage("Test", LogMessageType.Info, 1, 1, new DateTime(2021, 12, 4))
                }
            };
            List<Execution> executions = new()
            {
                new(1, DateTime.MinValue)
                {
                    Log = log
                }
            };

            //Act
            viewModel.UpdateData(executions);
            viewModel.ShowInfo = false;

            //Assert
            Assert.NotNull(viewModel.SelectedExecution);
            var msg = Assert.Single(viewModel.SelectedExecution!.LogMessages);
            Assert.Equal("Test", msg.Content);
            Assert.Equal(LogMessageType.Info, msg.Type);
            Assert.NotEmpty(viewModel.MessageView.SourceCollection);
            Assert.Empty(viewModel.MessageView);
        }
    }
}
