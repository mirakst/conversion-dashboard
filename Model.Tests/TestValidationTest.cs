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

        [Fact]
        public void ToString_SameValidationTest_ReturnsTrue()
        {
            var validationTest = new ValidationTest(DateTime.Parse("01-01-2020 12:00:00"),
                                              "Name",
                                              ValidationTest.ValidationStatus.Ok,
                                              "Manager name",
                                              null,
                                              null,
                                              null,
                                              "SRC SQL",
                                              "DST SQL");
            var expected = "(01-01-2020 12:00:00) Name: Ok\n" +
                           "[src=,dst=,toolkit=]\n" +
                           "Src sql: SRC SQL\n" +
                           "Dst sql: DST SQL";

            var actual = validationTest.ToString();

            Assert.Equal(expected, actual);
        }
    }
}