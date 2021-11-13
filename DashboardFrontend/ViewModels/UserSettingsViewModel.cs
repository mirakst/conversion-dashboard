using DashboardSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DashboardFrontend.ViewModels
{
    public class UserSettingsViewModel : INotifyPropertyChanged
    {
        public UserSettingsViewModel() 
        {
            Profiles = new ObservableCollection<Profile>(UserSettings.Profiles);
            ActiveProfile = UserSettings.ActiveProfile;
            LoggingQueryInterval = UserSettings.LoggingQueryInterval;
            HealthReportQueryInterval = UserSettings.HealthReportQueryInterval;
            ValidationQueryInterval = UserSettings.ValidationQueryInterval;
            ManagerQueryInterval = UserSettings.ManagerQueryInterval;
            AllQueryInterval = UserSettings.AllQueryInterval;
            SynchronizeAllQueries = UserSettings.SynchronizeAllQueries;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Profile> Profiles { get; set; }
        public int LoggingQueryInterval { get; set; }
        public int HealthReportQueryInterval { get; set; }
        public int ValidationQueryInterval { get; set; }
        public int ManagerQueryInterval { get; set; }
        public int AllQueryInterval { get; set; }
        public bool HasChangedActiveProfile { get; set; }

        private bool _synchronizeAllQueries;
        public bool SynchronizeAllQueries
        {
            get => _synchronizeAllQueries;
            set
            {
                _synchronizeAllQueries = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SynchronizeAllQueries)));
            }
        }
        private Profile? _activeProfile;
        public Profile? ActiveProfile
        {
            get => _activeProfile;
            set
            {
                _activeProfile = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveProfile)));
            }
        }
        public Profile? SelectedProfile { get; set; }
    }
}
