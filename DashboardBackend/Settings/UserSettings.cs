using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.ComponentModel;

namespace DashboardBackend.Settings
{
    public delegate void SettingsChanged();
        
    public class UserSettings : INotifyPropertyChanged, IUserSettings
    {
        private readonly string _fileName = "UserSettings.json";

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

        public event SettingsChanged SettingsChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public IList<Profile> Profiles { get; set; } = new List<Profile>();
        private Profile _activeProfile;
        [JsonIgnore]
        public Profile ActiveProfile
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
        public void OnSettingsChange()
        {
            SettingsChanged?.Invoke();
        }

        public bool HasEventListeners()
        {
            return SettingsChanged != null;
        }

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
        
        public void Save(IUserSettings settings)
        {
            OverwriteAll(settings);
            SaveToFile();
        }

        private void SaveToFile()
        {
            using FileStream stream = File.Open(_fileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            JsonSerializer.Serialize(stream, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="JsonException"/>
        public void Load()
        {
            string rawJson = File.ReadAllText(_fileName);
            UserSettings loadedSettings = JsonSerializer.Deserialize<UserSettings>(rawJson);
            if (loadedSettings != null)
            {
                OverwriteAll(loadedSettings);
            }
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
