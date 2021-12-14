using Xunit;
using DashboardFrontend.ValueConverters;

namespace DashboardFrontend.Tests
{
    public class TestSqlNotEmptyToBoolConverter
    {
        [Fact]
        public void Convert_NullStringAsInput_ReturnsFalse()
        {
            //Arrange
            SqlNotEmptyToBoolConverter SNETBC = new SqlNotEmptyToBoolConverter();
            string s = null;

            //Act
            bool result = (bool)SNETBC.Convert(s, null, null, null);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Convert_NonEmptyStringAsInput_ReturnsTrue()
        {
            //Arrange
            SqlNotEmptyToBoolConverter SNETBC = new SqlNotEmptyToBoolConverter();
            string s = "Test";

            //Act
            bool result = (bool)SNETBC.Convert(s, null, null, null);

            //Assert
            Assert.True(result);
        }
    }
}
