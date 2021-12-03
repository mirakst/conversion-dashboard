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

        [Fact]
        public void ToString_SameRam_ReturnTrue()
        {
            var ram = new Ram(123);
            var expected = "TOTAL MEMORY: 123 bytes";

            var actual = ram.ToString();

            Assert.Equal(expected, actual);
        }
    }
}