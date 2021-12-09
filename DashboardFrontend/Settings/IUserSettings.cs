using System.Collections.Generic;

namespace DashboardFrontend.Settings
{
    public interface IUserSettings
    {
        IList<Profile> Profiles { get; set; }
        Profile? ActiveProfile { get; set; }
        int LoggingQueryInterval { get; set; }
        int HealthReportQueryInterval { get; set; }
        int ValidationQueryInterval { get; set; }
        int ManagerQueryInterval { get; set; }
        int AllQueryInterval { get; set; }
        bool SynchronizeAllQueries { get; set; }
        bool HasActiveProfile { get; }

        void Save(IUserSettings userSettings);
        void Load();
    }
}