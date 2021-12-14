using DashboardBackend.Tests.Database;
using Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace DashboardBackend.Tests
{
    public class TestManagerScore
    {
        //[Fact]
        //public void GetManagerScore_GetsManagersScoreAndAssignsThemToEachManager_ReturnTrue()
        //{
        //    DataUtilities.DatabaseHandler = new TestDatabase();
        //    var expected = new double[] { 2.75, 2.75, 3.5};

        //    List<Manager> actual = new()
        //    {
        //        new Manager()
        //        {
        //            Name = "ManagerOne",
        //            ContextId = 1,
        //            RowsRead = 60,
        //            RowsWritten = 60,
        //            StartTime = DateTime.Parse("01-01-2020 12:00:00"),
        //            Runtime = TimeSpan.Parse("00:01:00"),
        //            EndTime = DateTime.Parse("01-01-2020 12:01:00")
        //        },
        //        new Manager()
        //        {
        //            Name = "ManagerTwo",
        //            ContextId = 1,
        //            RowsRead = 60,
        //            RowsWritten = 60,
        //            StartTime = DateTime.Parse("01-01-2020 12:00:00"),
        //            Runtime = TimeSpan.Parse("00:01:00"),
        //            EndTime = DateTime.Parse("01-01-2020 12:01:00")
        //        },
        //        new Manager()
        //        {
        //            Name = "ManagerThree",
        //            ContextId = 1,
        //            RowsRead = 120,
        //            RowsWritten = 120,
        //            StartTime = DateTime.Parse("01-01-2020 12:00:00"),
        //            Runtime = TimeSpan.Parse("00:01:00"),
        //            EndTime = DateTime.Parse("01-01-2020 12:01:00")
        //        }
        //    };

        //    ValidationReport validationReport = new();
        //    validationReport.ValidationTests = new()
        //    {
        //        new ValidationTest
        //        (
        //            DateTime.Parse("01-01-2020 12:00:00"),
        //            "validationTestOne",
        //            ValidationTest.ValidationStatus.Ok,
        //            "ManagerOne",
        //            30,
        //            30,
        //            null,
        //            string.Empty,
        //            string.Empty
        //        ),
        //        new ValidationTest
        //        (
        //            DateTime.Parse("01-01-2020 12:00:00"),
        //            "validationTestTwo",
        //            ValidationTest.ValidationStatus.Ok,
        //            "ManagerTwo",
        //            30,
        //            30,
        //            null,
        //            string.Empty,
        //            string.Empty
        //        ),
        //        new ValidationTest
        //        (
        //            DateTime.Parse("01-01-2020 12:00:00"),
        //            "validationTestThree",
        //            ValidationTest.ValidationStatus.Ok,
        //            "ManagerThree",
        //            30,
        //            30,
        //            null,
        //            string.Empty,
        //            string.Empty
        //        ),
        //        new ValidationTest
        //        (
        //            DateTime.Parse("01-01-2020 12:00:30"),
        //            "validationTestOne",
        //            ValidationTest.ValidationStatus.Failed,
        //            "ManagerOne",
        //            30,
        //            30,
        //            null,
        //            string.Empty,
        //            string.Empty
        //        ),
        //        new ValidationTest
        //        (
        //            DateTime.Parse("01-01-2020 12:00:30"),
        //            "validationTestTwo",
        //            ValidationTest.ValidationStatus.Failed,
        //            "ManagerTwo",
        //            30,
        //            30,
        //            null,
        //            string.Empty,
        //            string.Empty
        //        ),
        //        new ValidationTest
        //        (
        //            DateTime.Parse("01-01-2020 12:00:30"),
        //            "validationTestThree",
        //            ValidationTest.ValidationStatus.Failed,
        //            "ManagerThree",
        //            30,
        //            30,
        //            null,
        //            string.Empty,
        //            string.Empty
        //        ),
        //    };

        //    new ManagerScore(validationReport, actual);

        //    Assert.True(expected[0] == actual[0].Score 
        //                && expected[1] == actual[1].Score
        //                && expected[2] == actual[2].Score);
        //}

        //[Fact]
        //public void GetManagerScore_GetsManagersScoreAndAssignsThemToEachManagerNoValidationTests_ReturnTrue()
        //{
        //    DataUtilities.DatabaseHandler = new TestDatabase();
        //    var expected = new double[] { 0.75, 0.75, 1.5 };

        //    List<Manager> actual = new()
        //    {
        //        new Manager()
        //        {
        //            Name = "ManagerOne",
        //            ContextId = 1,
        //            RowsRead = 60,
        //            RowsWritten = 60,
        //            StartTime = DateTime.Parse("01-01-2020 12:00:00"),
        //            Runtime = TimeSpan.Parse("00:01:00"),
        //            EndTime = DateTime.Parse("01-01-2020 12:01:00")
        //        },
        //        new Manager()
        //        {
        //            Name = "ManagerTwo",
        //            ContextId = 1,
        //            RowsRead = 60,
        //            RowsWritten = 60,
        //            StartTime = DateTime.Parse("01-01-2020 12:00:00"),
        //            Runtime = TimeSpan.Parse("00:01:00"),
        //            EndTime = DateTime.Parse("01-01-2020 12:01:00")
        //        },
        //        new Manager()
        //        {
        //            Name = "ManagerThree",
        //            ContextId = 1,
        //            RowsRead = 120,
        //            RowsWritten = 120,
        //            StartTime = DateTime.Parse("01-01-2020 12:00:00"),
        //            Runtime = TimeSpan.Parse("00:01:00"),
        //            EndTime = DateTime.Parse("01-01-2020 12:01:00")
        //        }
        //    };

        //    ValidationReport validationReport = new();
        //    validationReport.ValidationTests = new();

        //    new ManagerScore(validationReport, actual);

        //    Assert.True(expected[0] == actual[0].Score
        //                && expected[1] == actual[1].Score
        //                && expected[2] == actual[2].Score);
        //}
    }
}
