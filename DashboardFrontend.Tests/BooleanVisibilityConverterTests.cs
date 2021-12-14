using Xunit;
using DashboardFrontend.ValueConverters;
using System.Windows;
using System.Windows.Data;
namespace DashboardFrontend.Tests
{
    public class BooleanVisibilityConverterTests
    {
        [Fact]  
        public void Convert_TrueAsInput_ReturnsVisible()
        {
            //Arrange
            BooleanToVisibilityConverter BTVC = new BooleanToVisibilityConverter();
            Visibility expected = Visibility.Visible;

            //Act
            Visibility actual = (Visibility)BTVC.Convert(true, null, null, null);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_FalseAsInput_ReturnsCollapsed()
        {
            //Arrange
            BooleanToVisibilityConverter BTVC = new BooleanToVisibilityConverter();
            Visibility expected = Visibility.Collapsed;

            //Act
            Visibility actual = (Visibility)BTVC.Convert(false, null, null, null);

            //Assert
            Assert.Equal(expected, actual);
        }

    }

}
