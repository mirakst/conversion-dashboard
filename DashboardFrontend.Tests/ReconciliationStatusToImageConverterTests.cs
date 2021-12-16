using Xunit;
using DashboardFrontend.ValueConverters;
using System;
using Model;
using static Model.Reconciliation;

namespace DashboardFrontend.Tests
{
    public class ReconciliationStatusToImageConverterTests
    {
        [Theory]
        [InlineData(ReconciliationStatus.Ok, "/Icons/ReconciliationOk.png")]
        [InlineData(ReconciliationStatus.Disabled, "/Icons/ReconciliationDisabled.png")]
        [InlineData(ReconciliationStatus.Failed, "/Icons/ReconciliationFailed.png")]
        public void Convert_ReconciliationAsInput_ResultEqualsStringTrue(ReconciliationStatus status, string expected)
        {
            //Arrange
            ReconciliationStatusToImageConverter VSTIC = new ReconciliationStatusToImageConverter();
            Reconciliation VT = new Reconciliation(new DateTime(2021, 12, 4), "", status, "", 1, 2, 3, "", "");

            //Act
            string actual = (string)VSTIC.Convert(VT, null, null, null);

            //Assert
            Assert.Equal(expected, actual);

        }
    }
}
