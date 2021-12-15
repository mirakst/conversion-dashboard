using DashboardBackend.Settings;
using Xunit;

namespace DashboardFrontend.Tests
{
    public class ProfileTests
    {
        [Fact]
        public void BuildConnectionString_ProfileConnectionStringAsInput_ResultEqualsStringTrue()
        {   //Arrange
            Profile profile = new Profile("TestName", "TestConversion", "TestdataSource", "Testdatabase", 30);
            string expected = "Data Source=TestdataSource;Initial Catalog=Testdatabase;Connect Timeout=30;Integrated Security=True;User Id=TestUser;Password=Test";

            //Act
            profile.BuildConnectionString("TestUser", "Test");
            string actual = profile.ConnectionString;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestEquals_ProfileIdAsInput_ResultEqualsIdTrue()
        {   //Arrange
            Profile p = new Profile();
      
            //Act
            var actual = p.Equals(p); 

            //Assert
            Assert.True(actual);
        }

        [Fact]
        public void TestHashCode_ProfileGetHashCodeAsInput_ResultEqualsIdAsIntTrue()
        {
            //Arrange
            Profile p = new Profile(2, "TestName", "TestConversion", "TestdataSource", "Testdatabase", 30);
            int expected = 2;

            //Act
            var actual = p.GetHashCode();

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}