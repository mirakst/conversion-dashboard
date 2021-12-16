using DashboardBackend.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DashboardFrontend.ViewModels
{
    public class UserSettingsViewModel : BaseViewModel, IUserSettings
    {
        public UserSettingsViewModel(IUserSettings userSettings)
        {
            Profiles = new ObservableCollection<Profile>(userSettings.Profiles);
            ActiveProfile = userSettings.ActiveProfile;
            LoggingQueryInterval = userSettings.LoggingQueryInterval;
            HealthReportQueryInterval = userSettings.HealthReportQueryInterval;
            ReconciliationQueryInterval = userSettings.ReconciliationQueryInterval;
            ManagerQueryInterval = userSettings.ManagerQueryInterval;
            AllQueryInterval = userSettings.AllQueryInterval;
            SynchronizeAllQueries = userSettings.SynchronizeAllQueries;
            SelectedProfile = ActiveProfile;
        }

        public IList<Profile> Profiles { get; set; }
        public int LoggingQueryInterval { get; set; }
        public int HealthReportQueryInterval { get; set; }
        public int ReconciliationQueryInterval { get; set; }
        public int ManagerQueryInterval { get; set; }
        public int AllQueryInterval { get; set; }
        public bool HasActiveProfile => ActiveProfile != null;
        public bool HasChangedActiveProfile { get; set; }
        public Profile? SelectedProfile { get; set; }

        private bool _synchronizeAllQueries;
        public bool SynchronizeAllQueries
        {
            get => _synchronizeAllQueries;
            set
            {
                _synchronizeAllQueries = value;
                OnPropertyChanged(nameof(SynchronizeAllQueries));
            }
        }
        private Profile? _activeProfile;

        public event SettingsChanged SettingsChanged;

        public Profile? ActiveProfile
        {
            get => _activeProfile;
            set
            {
                _activeProfile = value;
                OnPropertyChanged(nameof(ActiveProfile));
            }
        }

        public bool HasEventListeners()
        {
            return false;
        }

        public void OnSettingsChange()
        {
            
        }

        public void Save(IUserSettings settings)
        {

        }

        public void Load()
        {

        }
    }
}
