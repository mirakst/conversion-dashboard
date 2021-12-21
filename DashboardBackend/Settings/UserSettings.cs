using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace DashboardBackend.Settings
{
    /// <summary>
    /// Contains all user settings for the Dashboard, and methods to save and load them.
    /// </summary>
    public class UserSettings : INotifyPropertyChanged, IUserSettings
    {
        private readonly string _folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/ConversionDashboard/";
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
        public int ActiveProfileId => ActiveProfile?.Id ?? 0; // For JSON serialization

        /// <inheritdoc/>
        public void OnSettingsChange()
        {
            SettingsChanged?.Invoke();
        }

        /// <inheritdoc/>
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
        
        /// <inheritdoc/>
        public void Save(IUserSettings settings)
        {
            OverwriteAll(settings);
            using FileStream stream = File.Open(_folderPath + _fileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            JsonSerializer.Serialize(stream, this);
        }

        /// <inheritdoc/>
        public void Load()
        {
            Directory.CreateDirectory(_folderPath);
            string rawJson = File.ReadAllText(_folderPath + _fileName);
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
