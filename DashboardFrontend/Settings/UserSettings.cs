using DashboardFrontend.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace DashboardFrontend.Settings
{
    public delegate void SettingsChanged();
    public class UserSettings : BaseViewModel, IUserSettings
    {
        public UserSettings()
        {
        }

        [JsonConstructor]
        public UserSettings(IList<Profile> profiles, int loggingQueryInterval, int validationQueryInterval, int managerQueryInterval, int allQueryInterval, bool synchronizeAllQueries, int activeProfileId)
        {
            Profiles = profiles;
            LoggingQueryInterval = loggingQueryInterval;
            ValidationQueryInterval = validationQueryInterval;
            ManagerQueryInterval = managerQueryInterval;
            AllQueryInterval = allQueryInterval;
            SynchronizeAllQueries = synchronizeAllQueries;
            ActiveProfile = Profiles.FirstOrDefault(p => p.Id == activeProfileId);
        }

        private readonly string _fileName = "UserSettings.json";

        public IList<Profile> Profiles { get; set; } = new List<Profile>();
        private Profile? _activeProfile;
        [JsonIgnore]
        public Profile? ActiveProfile
        {
            get => _activeProfile;
            set
            {
                _activeProfile = value;
                OnPropertyChanged(nameof(ActiveProfile));
            }
        }
        public int LoggingQueryInterval { get; set; } = 1; // seconds
        public int HealthReportQueryInterval { get; set; } = 30;
        public int ValidationQueryInterval { get; set; } = 5;
        public int ManagerQueryInterval { get; set; } = 5;
        public int AllQueryInterval { get; set; } = 2;
        public bool SynchronizeAllQueries { get; set; } = false;
        [JsonIgnore]
        public bool HasActiveProfile => ActiveProfile is not null;
        // For JSON serialization
        public int ActiveProfileId => ActiveProfile?.Id ?? 0;
        public event SettingsChanged SettingsChanged;

        /// <summary>
        /// Invokes the <see cref="SettingsChanged"/> event.
        /// </summary>
        public void OnSettingsChange()
        {
            SettingsChanged?.Invoke();
        }

        /// <summary>
        /// Checks if the <see cref="SettingsChanged"/> event has listeners.
        /// </summary>
        /// <returns>True, if it has listeners.</returns>
        public bool HasEventListeners()
        {
            return SettingsChanged != null;
        }

        /// <summary>
        /// Overwrites all settings with new values.
        /// </summary>
        /// <param name="settings">The new settings to use.</param>
        private void OverwriteAll(IUserSettings settings)
        {
            Profiles = new List<Profile>(settings.Profiles);
            ActiveProfile = settings.ActiveProfile;
            LoggingQueryInterval = settings.LoggingQueryInterval;
            HealthReportQueryInterval = settings.HealthReportQueryInterval;
            ValidationQueryInterval = settings.ValidationQueryInterval;
            ManagerQueryInterval = settings.ManagerQueryInterval;
            AllQueryInterval = settings.AllQueryInterval;
            SynchronizeAllQueries = settings.SynchronizeAllQueries;
            OnSettingsChange();
        }
        
        /// <summary>
        /// Overwrites all settings and saves new settings to a file.
        /// </summary>
        /// <param name="settings">The new settings.</param>
        public void OverwriteAllAndSave(IUserSettings settings)
        {
            OverwriteAll(settings);
            SaveToFile();
        }

        /// <summary>
        /// Saves settings to a file.
        /// </summary>
        private void SaveToFile()
        {
            using FileStream stream = File.Open(_fileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            JsonSerializer.Serialize(stream, this);
        }

        /// <summary>
        /// Loads settings from Json file.
        /// </summary>
        public void LoadFromFile()
        {
            string rawJson = File.ReadAllText(_fileName);
            UserSettings? loadedSettings = JsonSerializer.Deserialize<UserSettings>(rawJson);
            if (loadedSettings != null)
            {
                OverwriteAll(loadedSettings);
            }
        }
    }
}
