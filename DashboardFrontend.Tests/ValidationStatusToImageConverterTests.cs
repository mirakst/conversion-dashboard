using Xunit;
using DashboardFrontend.ValueConverters;
using System;
using Model;
using static Model.ValidationTest;

namespace DashboardFrontend.Tests
{
    public class ValidationStatusToImageConverterTests
    {
        [Theory]
        [InlineData(ValidationStatus.Ok, "/Icons/ValidationOk.png")]
        [InlineData(ValidationStatus.Disabled, "/Icons/ValidationDisabled.png")]
        [InlineData(ValidationStatus.Failed, "/Icons/ValidationFailed.png")]
        public void Convert_ValidationTestAsInput_ResultEqualsStringTrue(ValidationStatus status, string expected)
        {
            //Arrange
            ValidationStatusToImageConverter VSTIC = new ValidationStatusToImageConverter();
            ValidationTest VT = new ValidationTest(new DateTime(2021, 12, 4), "", status, "", 1, 2, 3, "", "");

            //Act
            string actual = (string)VSTIC.Convert(VT, null, null, null);

            //Assert
            Assert.Equal(expected, actual);

        }
    }
}
