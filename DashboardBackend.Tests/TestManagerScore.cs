using DashboardBackend.Tests.Database;
using Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace DashboardBackend.Tests
{
    public class TestManagerScore
    {
        [Fact]
        public void GetManagerScore_GetsManagersScoreAndAssignsThemToEachManager_ReturnTrue()
        {
            DataUtilities.DatabaseHandler = new TestDatabase();
            List<Manager> expected = new()
            {
                new Manager
                (
                    1,
                    1,
                    "ManagerOne",
                    DateTime.Parse("01-01-2020 12:00:00"),
                    DateTime.Parse("01-01-2020 12:01:00")
                ),
                new Manager
                (
                    2,
                    1,
                    "ManagerTwo",
                    DateTime.Parse("01-01-2020 12:00:00"),
                    DateTime.Parse("01-01-2020 12:01:00")
                ),
                new Manager
                (
                    3,
                    1,
                    "ManagerThree",
                    DateTime.Parse("01-01-2020 12:00:00"),
                    DateTime.Parse("01-01-2020 12:01:00")
                ),
            };

            foreach (Manager manager in expected)
            {
                manager.Score = 1;
            }

            List<Manager> actual = new()
            {
                new Manager
                (
                    1,
                    1,
                    "ManagerOne",
                    DateTime.Parse("01-01-2020 12:00:00"),
                    DateTime.Parse("01-01-2020 12:01:00")
                ),
                new Manager
                (
                    2,
                    1,
                    "ManagerTwo",
                    DateTime.Parse("01-01-2020 12:00:00"),
                    DateTime.Parse("01-01-2020 12:01:00")
                ),
                new Manager
                (
                    3,
                    1,
                    "ManagerThree",
                    DateTime.Parse("01-01-2020 12:00:00"),
                    DateTime.Parse("01-01-2020 12:01:00")
                ),
            };

            foreach (Manager manager in actual)
            {
                manager.RowsRead = 30;
                manager.RowsWritten = 30;
            }

            ValidationReport validationReport = new();
            validationReport.ValidationTests = new()
            {
                new ValidationTest
                (
                    DateTime.Parse("01-01-2020 12:00:00"),
                    "validationTestOne",
                    ValidationTest.ValidationStatus.Ok,
                    "ManagerOne",
                    30,
                    30,
                    null,
                    string.Empty,
                    string.Empty
                ),
                new ValidationTest
                (
                    DateTime.Parse("01-01-2020 12:00:00"),
                    "validationTestTwo",
                    ValidationTest.ValidationStatus.Ok,
                    "ManagerTwo",
                    30,
                    30,
                    null,
                    string.Empty,
                    string.Empty
                ),
                new ValidationTest
                (
                    DateTime.Parse("01-01-2020 12:00:00"),
                    "validationTestThree",
                    ValidationTest.ValidationStatus.Ok,
                    "ManagerThree",
                    30,
                    30,
                    null,
                    string.Empty,
                    string.Empty
                ),
                new ValidationTest
                (
                    DateTime.Parse("01-01-2020 12:00:30"),
                    "validationTestOne",
                    ValidationTest.ValidationStatus.Failed,
                    "ManagerOne",
                    30,
                    30,
                    null,
                    string.Empty,
                    string.Empty
                ),
                new ValidationTest
                (
                    DateTime.Parse("01-01-2020 12:00:30"),
                    "validationTestTwo",
                    ValidationTest.ValidationStatus.Failed,
                    "ManagerTwo",
                    30,
                    30,
                    null,
                    string.Empty,
                    string.Empty
                ),
                new ValidationTest
                (
                    DateTime.Parse("01-01-2020 12:00:30"),
                    "validationTestThree",
                    ValidationTest.ValidationStatus.Failed,
                    "ManagerThree",
                    30,
                    30,
                    null,
                    string.Empty,
                    string.Empty
                ),
            };

            new ManagerScore(validationReport, actual);

            Assert.Equal(expected, actual);
        }
    }
}
