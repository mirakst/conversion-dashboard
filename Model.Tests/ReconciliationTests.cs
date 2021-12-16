using System;
using Xunit;

namespace Model.Tests
{
    public class ReconciliationTests
    {
        [Fact]
        public void Equals_DiffrentReconciliationWithSameParameters_ReturnTrue()
        {
            var expected = new Reconciliation(DateTime.Parse("01-01-2020 12:00:00"),
                                              "Name",
                                              Reconciliation.ReconciliationStatus.Ok,
                                              "Manager name",
                                              null,
                                              null,
                                              null,
                                              "SRC SQL",
                                              "DST SQL");

            var actual = new Reconciliation(DateTime.Parse("01-01-2020 12:00:00"),
                                              "Name",
                                              Reconciliation.ReconciliationStatus.Ok,
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