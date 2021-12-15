using System.Collections.Generic;

namespace DashboardBackend.Settings
{
    public delegate void SettingsChanged();

    /// <summary>
    /// Contains properties and method signatures for use in configuration of the Dashboard.
    /// </summary>
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

        /// <summary>
        /// Invokes the <see cref="SettingsChanged"/> event.
        /// </summary>
        void OnSettingsChange();

        /// <summary>
        /// Checks if the <see cref="SettingsChanged"/> event has listeners.
        /// </summary>
        /// <returns>True, if it has listeners.</returns>
        bool HasEventListeners();

        /// <summary>
        /// Overwrites all settings with the values in the specified <see cref="IUserSettings"/> object and saves them to the file system.
        /// </summary>
        /// <param name="settings">The settings to save.</param>
        void Save(IUserSettings settings);
        /// <summary>
        /// Loads user settings from the file system.
        /// </summary>
        void Load();
    }
}