using DashboardBackend.Settings;
using System.Collections.Generic;
using Xunit;

namespace DashboardFrontend.Tests
{
    public class UserSettingsTests
    {
        [Fact]
        public void Save_OverwritesAllValuesSuccessfully()
        {
            //Arrange
            UserSettings newSettings = new UserSettings();
            newSettings.Profiles = new List<Profile> { new Profile("TestName", "TestConversion", "TestDataSource", "TestDatabase", 0) };
            newSettings.ActiveProfile = new Profile("TestName", "TestConversion", "TestDataSource", "TestDatabase", 0);
            newSettings.LoggingQueryInterval = 200;
            newSettings.HealthReportQueryInterval = 200;
            newSettings.ReconciliationQueryInterval = 200;
            newSettings.ManagerQueryInterval = 200;
            newSettings.AllQueryInterval = 200;
            newSettings.SynchronizeAllQueries = false;
            UserSettings result = new();

            //Act
            result.Save(newSettings);

            //Assert
            Assert.NotNull(result);
            var profile = Assert.Single(result.Profiles);
            Assert.NotNull(profile);
            Assert.Equal("TestName", profile.Name);
            Assert.Equal(result.ActiveProfile, newSettings.ActiveProfile);
            Assert.Equal(result.LoggingQueryInterval, newSettings.LoggingQueryInterval);
            Assert.Equal(result.HealthReportQueryInterval, newSettings.HealthReportQueryInterval);
            Assert.Equal(result.ReconciliationQueryInterval, newSettings.ReconciliationQueryInterval);
            Assert.Equal(result.ManagerQueryInterval, newSettings.ManagerQueryInterval);
            Assert.Equal(result.AllQueryInterval, newSettings.AllQueryInterval);
            Assert.Equal(result.SynchronizeAllQueries, newSettings.SynchronizeAllQueries);
        }
    }
}