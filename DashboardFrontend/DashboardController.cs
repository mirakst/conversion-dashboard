using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

using DashboardBackend;
using DashboardBackend.Settings;

using DashboardFrontend.DetachedWindows;
using DashboardFrontend.NewViewModels;

using Model;

namespace DashboardFrontend
{
    public class DashboardController : BaseViewModel, IDashboardController
    {
        private readonly List<Timer> _timers;
        private LogMessageParser _logMessageParser;
        private readonly System.Windows.Threading.Dispatcher _dispatcher;

        public DashboardController(IUserSettings userSettings)
        {
            _dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            UserSettings = userSettings;
            LoadUserSettings();
        }

        public DashboardController(IUserSettings userSettings, IDatabaseHandler databaseHandler) : this(userSettings)
        {
            DatabaseHandler = databaseHandler;
            _timers = new();
        }

        public event ConversionCreated OnConversionCreated;

        public IDatabaseHandler DatabaseHandler { get; set; }
        public IUserSettings UserSettings { get; set; }
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

        public void ChangeMonitoringState()
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
                        DatabaseHandler.SetupDatabase(UserSettings.ActiveProfile);
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

        public void StartMonitoring()
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
                // For testing, these timers should invoke some event which we can subscribe to, in order to fully test the integration.
                // Even better would be to turn these callback functions into a seperate class...
                Timer executionTimer = new(x => { }, null, 0, 1000);
                Timer logTimer = new(x => { TryUpdateLog(); }, null, 200, UserSettings.LoggingQueryInterval * 1000);
                Timer managerTimer = new(x => { }, null, 200, UserSettings.ManagerQueryInterval * 1000);
                Timer validationsTimer = new(x => { }, null, 200, UserSettings.ValidationQueryInterval * 1000);
                Timer healthReportTimer = new(x => { }, null, 200, UserSettings.HealthReportQueryInterval * 1000);
                _timers.AddRange(new List<Timer> { executionTimer, healthReportTimer, logTimer, validationsTimer, managerTimer });
            }
        }

        public void StopMonitoring()
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
        public void TryUpdateLog()
        {
            if (Conversion is null)
            {
                throw new ArgumentNullException(nameof(Conversion), "Conversion must not be null when monitoring");
            }

            IList<LogMessage>? newData = DatabaseHandler.GetLogMessagesSince(Conversion.LastLogQuery);
            Conversion.LastLogQuery = DateTime.Now;

            List<Manager>? managers = _logMessageParser.Parse(newData);
            // AllManagers list may not be necessary
            //lock (Conversion.AllManagers)
            //{
            //    Conversion.AllManagers.AddRange(managers);
            //}

            foreach (Execution exec in Conversion.Executions)
            {
                if (managers.Any())
                {
                    List<Manager>? newExecManagers = managers.Where(m => m.ExecutionId == exec.Id).ToList();
                    lock (exec.Managers)
                    {
                        _dispatcher.Invoke(() =>
                        {
                            exec.AddManagers(newExecManagers);
                        });
                    }
                }
                if (newData.Any())
                {
                    IEnumerable<LogMessage>? newExecMessages = newData.Where(m => m.ExecutionId == exec.Id);
                    _dispatcher.Invoke(() =>
                    {
                        exec.Log.Messages.AddRange(newExecMessages);
                    });
                }
            }
        }

        private void TryUpdateExecutions()
        {

        }
    }
}
