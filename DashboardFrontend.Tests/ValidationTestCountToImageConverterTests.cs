using Xunit;
using DashboardFrontend.ValueConverters;
using System;
using DashboardFrontend.ViewModels;
using Model;
using static Model.ValidationTest;

namespace DashboardFrontend.Tests
{
    public class ValidationTestCountToImageConverterTests
    {
        [Theory]
        [InlineData(ValidationStatus.Ok, "/Icons/ValidationOk.png")]
        [InlineData(ValidationStatus.Disabled, "/Icons/ValidationDisabled.png")]
        [InlineData(ValidationStatus.Failed, "/Icons/ValidationFailed.png")]
        public void Convert_ManagerObservableAsInput_ResultEqualsStringTrue(ValidationStatus status, string expected)
        {   
            //Arrange
            ValidationTestCountToImageConverter VTCTIC = new ValidationTestCountToImageConverter();
            Manager M = new Manager();
            M.Validations.Add(new ValidationTest(new DateTime(2021, 12, 4),"", status,"", 1,2,3, "", ""));
            ManagerObservable MO = new ManagerObservable(M);
         
            //Act
            string actual = (string)VTCTIC.Convert(MO, null, null, null);

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
