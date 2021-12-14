using Xunit;
using DashboardFrontend.ValueConverters;
using DashboardFrontend.Settings;


namespace DashboardFrontend.Tests
{
    public class TestProfileConverter
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
