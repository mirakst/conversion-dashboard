using Xunit;
using Model;
using static Model.LogMessage;
using DashboardFrontend.ViewModels;
using System;


namespace DashboardFrontend.Tests
{
    public class TestLogViewModel
    {
        //[Fact]
        //public void UpdateData_LogAsInput_ResultLogViewModelMessageListConTainsAddedMessageFromLogTrue()
        //{   //Arrange
        //    LogViewModel LVM = new LogViewModel();
        //    Log l = new Log();
        //    LogMessage LM = new LogMessage("Test", LogMessageType.Info, 1, 1, new DateTime(2021, 12, 4));
        //    l.Messages.Add(LM);
            
        //    //Act
        //    LVM.UpdateData(l);

        //    //Assert
        //    Assert.True(LVM.MessageList.Contains(LM));
        //}

        //[Fact]
        //public void UpdateCounters_LogAsInput_ResultLogViewModelCountsEqualLogCountsTrue()
        //{
        //    //Arrange
        //    LogViewModel LVM = new LogViewModel();
        //    Log L = new Log();
        //    LVM.InfoCount = 1;
        //    LVM.WarnCount = 1;
        //    LVM.ErrorCount = 1;
        //    LVM.FatalCount = 1;
        //    LVM.ValidationCount = 1;

        //    //Act
        //    LVM.UpdateData(L);

        //    //Assert
        //    Assert.True(L.InfoCount == LVM.InfoCount && L.WarnCount == LVM.WarnCount && L.ErrorCount == LVM.ErrorCount && L.FatalCount == LVM.FatalCount && L.ValidationCount == LVM.ValidationCount);

        //}
    }
}
