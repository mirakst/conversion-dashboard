using DashboardBackend;
using DashboardBackend.Database;
using DashboardFrontend.DetachedWindows;
using DashboardFrontend.NewViewModels;
using DashboardFrontend.Settings;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

namespace DashboardFrontend
{
    public class DashboardController : BaseViewModel, IDashboardController
    {
        private readonly List<Timer> _timers;

        public DashboardController(IUserSettings userSettings, IDatabaseHandler databaseHandler)
        {
            UserSettings = userSettings;
            DatabaseHandler = databaseHandler;
            LoadUserSettings();
            _timers = new();
        }

        public event ConversionCreated OnConversionCreated;

        public IDatabaseHandler DatabaseHandler { get; private set; }
        public IUserSettings UserSettings { get; private set; }
        private Conversion? _conversion;
        public Conversion? Conversion
        {
            get => _conversion;
            set
            {
                _conversion = value;
                OnPropertyChanged(nameof(Conversion));
                if (value is not null)
                {
                    OnConversionCreated?.Invoke(value);
                }
            }
        }
        private bool _hasConversion;
        public bool HasConversion
        {
            get => _hasConversion;
            set
            {
                _hasConversion = value;
                OnPropertyChanged(nameof(HasConversion));
            }
        }
        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }

        public void OnChangeMonitoringStateRequested()
        {
            if (IsRunning is false)
            {
                if (UserSettings.ActiveProfile is null)
                {
                    Trace.WriteLine("Please create a profile");
                }
                else
                {
                    if (UserSettings.ActiveProfile.HasReceivedCredentials is false)
                    {
                        GetCredentials();
                    }
                    if (UserSettings.ActiveProfile.HasReceivedCredentials)
                    {
                        if (!UserSettings.ActiveProfile.HasEventListeners())
                        {
                            UserSettings.ActiveProfile.ProfileChanged += SetupNewConversion;
                        }
                        UserSettings.ActiveProfile.HasStartedMonitoring = true;
                        DatabaseHandler.Database = new SqlDatabase(UserSettings.ActiveProfile.ConnectionString);
                        SetupNewConversion();
                        StartMonitoring();
                    }
                }
            }
            else
            {
                UserSettings.ActiveProfile!.HasStartedMonitoring = false;
                StopMonitoring();
            }
        }

        public void SetupNewConversion()
        {
            if (IsRunning)
            {
                IsRunning = false;
                StopMonitoring();
            }
            Conversion = new();
            HasConversion = true;
            //////
            Conversion.AddExecution(new(1, DateTime.MinValue));
            Conversion.AddExecution(new(2, DateTime.MinValue));
        }

        private void LoadUserSettings()
        {
            UserSettings.Load(); // Should be handled through an interface as well...
        }

        private void GetCredentials()
        {
            ConnectDBDialog connectDBDialog = new(UserSettings);
            connectDBDialog.ShowDialog();
        }

        private void StartMonitoring()
        {
            if (Conversion is null)
            {
                throw new ArgumentNullException(nameof(Conversion), "Expected a non-null Conversion when starting monitoring");
            }
            IsRunning = true;
            DatabaseHandler.BuildHealthReport(Conversion.HealthReport);
            
            if (UserSettings.SynchronizeAllQueries)
            {
                Timer synchronizedTimer = new(x =>
                {
                    // Perform update for all components
                }, null, 0, UserSettings.AllQueryInterval * 1000);
                _timers.Add(synchronizedTimer);
            }
            else
            {
                Timer executionTimer = new(x => { }, null, 0, 1000);
                Timer logTimer = new(x => { TryUpdateLog(); }, null, 200, UserSettings.LoggingQueryInterval * 1000);
                Timer managerTimer = new(x => { }, null, 200, UserSettings.ManagerQueryInterval * 1000);
                Timer validationsTimer = new(x => { }, null, 200, UserSettings.ValidationQueryInterval * 1000);
                Timer healthReportTimer = new(x => { }, null, 200, UserSettings.HealthReportQueryInterval * 1000);
                _timers.AddRange(new List<Timer> { executionTimer, healthReportTimer, logTimer, validationsTimer, managerTimer });
            }
        }

        private void StopMonitoring()
        {
            IsRunning = false;
            foreach (Timer timer in _timers)
            {
                timer.Dispose();
            }
            _timers.Clear();
        }

        private void TryUpdateLog()
        {
            if (Conversion is null)
            {
                throw new ArgumentNullException(nameof(Conversion), "Conversion must not be null when monitoring");
            }
            if (!Conversion.Executions.Any())
            {
                // Assume no executions have been started or found yet.
                return; 
            }

            lock (Conversion.Executions)
            {
                foreach (Execution exec in Conversion.Executions)
                {
                    var newData = DatabaseHandler.GetLogMessagesFromExecutionSince(exec.Log.LastModified, exec.Id);
                    exec.Log.LastModified = DateTime.Now;
                    if (newData.Any())
                    {
                        // Ensure that data is only updated on the UI thread as required by WPF.
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            exec.Log.Messages.AddRange(newData);
                        });
                    }
                }
            }
        }
    }
}
