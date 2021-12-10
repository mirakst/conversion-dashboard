using System.Collections.Generic;
using DashboardBackend.Settings;

namespace IntegrationTests
{
    public class FakeUserSettings : IUserSettings
    {
        public IList<Profile> Profiles { get; set; } = new List<Profile>();
        public Profile? ActiveProfile { get; set; }
        public int LoggingQueryInterval { get; set; } = 1;
        public int HealthReportQueryInterval { get; set; } = 30;
        public int ValidationQueryInterval { get; set; } = 5;
        public int ManagerQueryInterval { get; set; } = 5;
        public int AllQueryInterval { get; set; } = 2;
        public bool SynchronizeAllQueries { get; set; }

        public bool HasActiveProfile => ActiveProfile is not null;

        public bool HasLoaded { get; set; }
        public bool HasSaved { get; set; }

        public void Load()
        {
            HasLoaded = true;
        }

        public void Save(IUserSettings userSettings)
        {
            HasSaved = true;
        }
    }
}