using Xunit;
using DashboardFrontend.ValueConverters;
using DashboardBackend.Settings;

namespace DashboardFrontend.Tests
{
    public class ProfileConverterTests
    {
        [Fact]
        public void Convert_ProfileAsInput_ReturnsTrue()
        {
            //Arrange
            ProfileConverter PCv  = new ProfileConverter();
            Profile p = new Profile();

            //Act
            bool result = (bool)PCv.Convert(p, null, null, null);

            //Assert
            Assert.True(result);
        }
    }
}
