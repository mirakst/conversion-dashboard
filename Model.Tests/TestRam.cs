using Xunit;

namespace Model.Tests
{
    public class TestRam
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