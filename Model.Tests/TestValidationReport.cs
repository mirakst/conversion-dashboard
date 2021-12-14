using Xunit;

namespace Model.Tests
{
    public class TestValidationReport
    {
        [Fact]
        public void Equals_DiffrentValidationReportWithSameParameters_ReturnTrue()
        {
            var expected = new ValidationReport();
            var actual = new ValidationReport();

            Assert.Equal(expected, actual);
        }
    }
}