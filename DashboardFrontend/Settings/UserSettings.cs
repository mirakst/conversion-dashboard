using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DashboardFrontend.Settings
{
    public class UserSettings : IUserSettings
    {
        public UserSettings()
        {
        }

        private readonly string _fileName = "UserSettings.json";

        public IList<Profile> Profiles { get; set; } = new List<Profile>();
        public Profile? ActiveProfile { get; set; }
        public int LoggingQueryInterval { get; set; } = 15; // seconds
        public int HealthReportQueryInterval { get; set; } = 30;
        public int ValidationQueryInterval { get; set; } = 120;
        public int ManagerQueryInterval { get; set; } = 60;
        public int AllQueryInterval { get; set; } = 30;
        public bool SynchronizeAllQueries { get; set; } = false;
        public bool HasActiveProfile => ActiveProfile is not null;

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
        }
        
        public void OverwriteAllAndSave(IUserSettings settings)
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
