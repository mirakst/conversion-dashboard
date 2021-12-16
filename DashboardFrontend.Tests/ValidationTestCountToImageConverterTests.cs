using Xunit;
using DashboardFrontend.ValueConverters;
using System;
using DashboardFrontend.ViewModels;
using Model;
using static Model.Reconciliation;

namespace DashboardFrontend.Tests
{
    public class ReconciliationCountToImageConverterTests
    {
        [Theory]
        [InlineData(ReconciliationStatus.Ok, "/Icons/ReconciliationOk.png")]
        [InlineData(ReconciliationStatus.Disabled, "/Icons/ReconciliationDisabled.png")]
        [InlineData(ReconciliationStatus.Failed, "/Icons/ReconciliationFailed.png")]
        public void Convert_ManagerObservableAsInput_ResultEqualsStringTrue(ReconciliationStatus status, string expected)
        {   
            //Arrange
            ReconciliationCountToImageConverter VTCTIC = new ReconciliationCountToImageConverter();
            Manager M = new Manager();
            M.Reconciliations.Add(new Reconciliation(new DateTime(2021, 12, 4),"", status,"", 1,2,3, "", ""));
            ManagerObservable MO = new ManagerObservable(M);
         
            //Act
            string actual = (string)VTCTIC.Convert(MO, null, null, null);

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
