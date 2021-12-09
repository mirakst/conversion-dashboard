using DashboardFrontend.Settings;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DashboardFrontend.Tests
{
    public class TestUserSettings
    {
        [Fact]
        public void TestOverWriteAll()
        {
       
            //Arrange
            UserSettings expected = new UserSettings();
            expected.Profiles = new List<Profile> { new Profile("TestName", "TestConversion", "TestDataSource", "TestDatabase", 0) };
            expected.ActiveProfile = new Profile("TestName", "TestConversion", "TestDataSource", "TestDatabase", 0);
            expected.LoggingQueryInterval = 200;
            expected.HealthReportQueryInterval = 200;
            expected.ValidationQueryInterval = 200;
            expected.ManagerQueryInterval = 200;
            expected.AllQueryInterval = 200;
            expected.SynchronizeAllQueries = false;
            UserSettings actual = new UserSettings();

            //Act
            expected.OverwriteAllAndSave(actual);

            //Assert
            Assert.True(expected.Profiles.SequenceEqual(actual.Profiles)
                && expected.ActiveProfile == actual.ActiveProfile
                && expected.LoggingQueryInterval == actual.LoggingQueryInterval
                && expected.HealthReportQueryInterval == actual.HealthReportQueryInterval
                && expected.ValidationQueryInterval == actual.ValidationQueryInterval
                && expected.ManagerQueryInterval == actual.ManagerQueryInterval
                && expected.AllQueryInterval == actual.AllQueryInterval
                && expected.SynchronizeAllQueries == actual.SynchronizeAllQueries);

        }
    }
}