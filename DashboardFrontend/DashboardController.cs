using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

using DashboardBackend;
using DashboardBackend.Database;

using DashboardFrontend.DetachedWindows;
using DashboardFrontend.NewViewModels;
using DashboardFrontend.Settings;

using Model;

namespace DashboardFrontend
{
    public class DashboardController : BaseViewModel, IDashboardController
    {
        private readonly List<Timer> _timers;
        private LogMessageParser _logMessageParser;

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
            _logMessageParser = new(Conversion);
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

        /// <summary>
        /// Queries any new Log data through the <see cref="DatabaseHandler"/>, parses it (which involves creating and/or updating executions and managers), and adds it to the Log of the associated execution.
        /// </summary>
        /// <remarks>Any changes that will affect the UI are done one the UI thread (as dictated by WPF), and collections at risk of being modified during this method's execution are locked.</remarks>
        /// <exception cref="ArgumentNullException"></exception>
        private void TryUpdateLog()
        {
            if (Conversion is null)
            {
                throw new ArgumentNullException(nameof(Conversion), "Conversion must not be null when monitoring");
            }

            var newData = DatabaseHandler.GetLogMessagesSince(Conversion.LastLogQuery);
            Conversion.LastLogQuery = DateTime.Now;

            var managers = _logMessageParser.Parse(newData);
            // AllManagers list may not be necessary
            //lock (Conversion.AllManagers)
            //{
            //    Conversion.AllManagers.AddRange(managers);
            //}

            foreach (Execution exec in Conversion.Executions)
            {
                if (managers.Any())
                {
                    var newExecManagers = managers.Where(m => m.ExecutionId == exec.Id).ToList();
                    lock (exec.Managers)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            exec.AddManagers(newExecManagers);
                        });
                    }
                }
                if (newData.Any())
                {
                    var newExecMessages = newData.Where(m => m.ExecutionId == exec.Id);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        exec.Log.Messages.AddRange(newExecMessages);
                    });
                }
            }
        }
    }
}
