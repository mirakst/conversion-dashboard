using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DashboardBackend.Database;
using DashboardFrontend.DetachedWindows;
using DashboardFrontend.Settings;
using DashboardFrontend.ViewModels;
using DU = DashboardBackend.DataUtilities;
using Model;

namespace DashboardFrontend
{
    public class Controller
    {
        private readonly MainWindowViewModel _vm;
        private readonly Log _log;
        private readonly ValidationReport _validationReport;
        private HealthReport _healthReport = null!;
        private readonly List<Timer> _timers;
        private readonly SynchronizationContext? _uiContext;
        public UserSettings UserSettings { get; set; } = new UserSettings();

        public Controller(MainWindowViewModel viewModel)
        {
            _uiContext = SynchronizationContext.Current;
            TryLoadUserSettings();
            _vm = viewModel;
            _log = new Log();
            _validationReport = new ValidationReport();
            _timers = new List<Timer>();
        }

        /// <summary>
        /// Initializes the view models in the controller.
        /// </summary>
        public void Initialize(DataGrid dataGridValidations)
        {
            _vm.LogViewModel = new LogViewModel();
            _vm.ValidationReportViewModel = new ValidationReportViewModel(dataGridValidations);
            _vm.LiveChartViewModel = new LiveChartViewModel(new PerformanceChart());
        }

        /// <summary>
        /// Updates the messages in the log.
        /// </summary>
        public void UpdateLog(DateTime timestamp)
        {
            _log.Messages = DU.GetLogMessages(timestamp);
            _uiContext?.Send(x => _vm.LogViewModel.UpdateData(_log), null);
        }

        /// <summary>
        /// Updates the validation tests in the validation report.
        /// </summary>
        public void UpdateValidationReport(DateTime timestamp)
        {
            _validationReport.ValidationTests = DU.GetAfstemninger(timestamp);
            _uiContext?.Send(x => _vm.ValidationReportViewModel.UpdateData(_validationReport), null);
        }

        /// <summary>
        /// Updates the readings in the health report.
        /// </summary>
        public void UpdateHealthReport(DateTime timestamp)
        {
            if (_healthReport.IsInitialized)
            {
                DU.AddHealthReportReadings(_healthReport, timestamp);
                _uiContext?.Send(x => _vm.LiveChartViewModel.UpdateData(_healthReport.Ram, _healthReport.Cpu), null);
            }
            else
            {
                _healthReport = DU.BuildHealthReport();
            }
        }

        /// <summary>
        /// Ensures that there is an active profile with database credentials, and then starts the monitoring process.
        /// If the process is already running, it is ended.
        /// </summary>
        public void OnStartPressed()
        {
            if (UserSettings.ActiveProfile is null)
            {
                MessageBox.Show("Please select a profile.");
            }
            else if (!_vm.IsRunning)
            {
                if (!UserSettings.ActiveProfile.HasReceivedCredentials)
                {
                    GetCredentials();
                }
                if (UserSettings.ActiveProfile.HasReceivedCredentials)
                {
                    DU.DatabaseHandler = new SqlDatabase(UserSettings.ActiveProfile.ConnectionString);
                    _healthReport = DU.BuildHealthReport();
                    StartMonitoring();
                }
            }
            else
            {
                StopMonitoring();
            }
        }

        /// <summary>
        /// Gets database credentials for the current profile and builds its connection string.
        /// </summary>
        private void GetCredentials()
        {
            ConnectDBDialog dialogPopup = new ConnectDBDialog(UserSettings);
            dialogPopup.ShowDialog();
        }

        /// <summary>
        /// Sets up the necessary query timers that gather and update information in the system.
        /// </summary>
        private void StartMonitoring()
        {
            if (UserSettings.SynchronizeAllQueries)
            {
                _timers.Add(new Timer(x =>
                {
                    Task.Run(() => UpdateHealthReport(_healthReport.LastModified));
                    Task.Run(() => UpdateLog(_log.LastModified));
                    Task.Run(() => UpdateValidationReport(_validationReport.LastModified));
                    //Task.Run(() => QueryManagers());
                }, null, 500, UserSettings.AllQueryInterval * 1000));
            }
            else
            {
                _timers.Add(new Timer(x => UpdateHealthReport(_healthReport.LastModified), null, 500, UserSettings.HealthReportQueryInterval * 1000));
                _timers.Add(new Timer(x => UpdateLog(_log.LastModified), null, 500, UserSettings.LoggingQueryInterval * 1000));
                //_timers.Add(new(x => QueryManagers(), null, 500, UserSettings.ManagerQueryInterval * 1000));
                _timers.Add(new Timer(x => UpdateValidationReport(_validationReport.LastModified), null, 500, UserSettings.ValidationQueryInterval * 1000));
            }
            _vm.IsRunning = true;
        }

        /// <summary>
        /// Stops the periodic monitoring functions.
        /// </summary>
        private void StopMonitoring()
        {
            foreach (Timer timer in _timers)
            {
                timer.Dispose();
            }
            _timers.Clear();
            _vm.IsRunning = false;
        }

        /// <summary>
        /// Attempts to load user settings, catching potential exceptions if loading fails and displaying an error.
        /// </summary>
        private void TryLoadUserSettings()
        {
            try
            {
                UserSettings.LoadFromFile();
            }
            catch (System.IO.FileNotFoundException)
            {
                // Configuration file was not found, possibly first time setup
            }
            catch (System.Text.Json.JsonException ex)
            {
                DisplayGeneralError("Failed to parse contents of UserSettings.json", ex);
            }
            catch (System.IO.IOException ex)
            {
                DisplayGeneralError("An unexpected problem occurred while loading user settings", ex);
            }
        }
        private static void DisplayGeneralError(string message, Exception ex)
        {
            MessageBox.Show($"{message}\n\nDetails\n{ex.Message}");
        }
    }
}
