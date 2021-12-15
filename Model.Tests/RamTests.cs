using Xunit;

namespace Model.Tests
{
    public class RamTests
    {
        [Fact]
        public void Equals_DiffrentRamWithSameParameters_ReturnTrue()
        {
            var expected = new Ram(123);
            var actual = new Ram(123);

            Assert.Equal(expected, actual);
        }
    }
}