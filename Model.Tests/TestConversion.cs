using Xunit;

namespace Model.Tests
{
    public class TestConversion
    {
        [Fact]
        public void Equals_DifferentConversionsWithNoParameters_ReturnsTrue()
        {
            var expected = new Conversion();

            var actual = new Conversion();

            Assert.Equal(expected, actual);
        }
    }
}
