using System.Collections.Generic;

namespace DashboardBackend.Settings
{
    public delegate void SettingsChanged();

    public interface IUserSettings
    {
        public event SettingsChanged SettingsChanged;

        IList<Profile> Profiles { get; set; }
        Profile ActiveProfile { get; set; }
        int LoggingQueryInterval { get; set; }
        int HealthReportQueryInterval { get; set; }
        int ValidationQueryInterval { get; set; }
        int ManagerQueryInterval { get; set; }
        int AllQueryInterval { get; set; }
        bool SynchronizeAllQueries { get; set; }
        bool HasActiveProfile { get; }

        bool HasEventListeners();
        void OnSettingsChange();
        void Save(IUserSettings settings);
        void Load();
    }
}