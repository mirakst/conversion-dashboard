using System;
using Xunit;

namespace Model.Tests
{
    public class TestValidationTest
    {
        [Fact]
        public void Equals_DiffrentValidationTestWithSameParameters_ReturnTrue()
        {
            var expected = new ValidationTest(DateTime.Parse("01-01-2020 12:00:00"),
                                              "Name",
                                              ValidationTest.ValidationStatus.Ok,
                                              "Manager name",
                                              null,
                                              null,
                                              null,
                                              "SRC SQL",
                                              "DST SQL");

            var actual = new ValidationTest(DateTime.Parse("01-01-2020 12:00:00"),
                                              "Name",
                                              ValidationTest.ValidationStatus.Ok,
                                              "Manager name",
                                              null,
                                              null,
                                              null,
                                              "SRC SQL",
                                              "DST SQL");

            Assert.Equal(expected, actual);
        }
    }
}