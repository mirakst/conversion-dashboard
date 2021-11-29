using System;
using System.Collections.Generic;
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
        private Log _log;
        private ValidationReport _validationReport;
        private HealthReport _healthReport;
        private readonly List<Timer> _timers;
        private readonly SynchronizationContext? _uiContext;
        public readonly Conversion _conversion;
        public readonly List<HealthReportViewModel> HealthReportViewModels = new();
        public readonly List<LogViewModel> LogViewModels = new();
        public readonly List<ValidationReportViewModel> ValidationReportViewModels = new();
        public readonly List<ManagerViewModel> ManagerViewModels = new();
        public UserSettings UserSettings { get; set; } = new();
        

        public Controller(MainWindowViewModel viewModel)
        {
            _uiContext = SynchronizationContext.Current;
            TryLoadUserSettings();
            _vm = viewModel;
            _conversion = new();
            _timers = new List<Timer>();
        }

        /// <summary>
        /// Initializes the view models in the <see cref="Controller"/>.
        /// </summary>
        public void InitializeViewModels(DataGrid dataGridValidations)
        {
            _vm.LogViewModel = new LogViewModel();
            LogViewModels.Add(_vm.LogViewModel);

            _vm.ValidationReportViewModel = new ValidationReportViewModel(dataGridValidations);
            ValidationReportViewModels.Add(_vm.ValidationReportViewModel);

            _vm.HealthReportViewModel = new HealthReportViewModel();
            HealthReportViewModels.Add(_vm.HealthReportViewModel);

            _vm.ManagerViewModel = new ManagerViewModel(_healthReport);
            ManagerViewModels.Add(_vm.ManagerViewModel);
        }

        public LogViewModel CreateLogViewModel()
        {
            LogViewModel result = new();
            result.UpdateData(_log);
            LogViewModels.Add(result);
            return result;
        }

        public ValidationReportViewModel CreateValidationReportViewModel()
        {
            ValidationReportViewModel result = new();
            result.UpdateData(_validationReport);
            ValidationReportViewModels.Add(result);
            return result;
        }

        public HealthReportViewModel CreateHealthReportViewModel()
        {
            HealthReportViewModel result = new();
            result.SystemLoadChart.UpdateData(_healthReport.Ram, _healthReport.Cpu);
            result.NetworkChart.UpdateData(_healthReport.Network);
            result.NetworkDeltaChart.UpdateData(_healthReport.Network);
            result.NetworkSpeedChart.UpdateData(_healthReport.Network);
            HealthReportViewModels.Add(result);
            return result;
        }

        public ManagerViewModel CreateManagerViewModel()
        {
            ManagerViewModel result = new(_healthReport);
            result.UpdateData(_conversion.ActiveExecution.Managers);
            ManagerViewModels.Add(result);
            return result;
        }

        /// <summary>
        /// Updates the messages in the log.
        /// </summary>
        public void UpdateLog(DateTime timestamp)
        {
            _log.LastModified = DateTime.Now;
            _log.Messages = DU.GetLogMessages(timestamp);
            foreach (var vm in LogViewModels)
            {
                _uiContext?.Send(x => vm.UpdateData(_log), null);
            }
        }

        /// <summary>
        /// Updates the validation tests in the validation report.
        /// </summary>
        public void UpdateValidationReport(DateTime timestamp)
        {
            _validationReport.LastModified = DateTime.Now;
            _validationReport.ValidationTests = DU.GetAfstemninger(timestamp);
            foreach (var vm in ValidationReportViewModels)
            {
                _uiContext?.Send(x => vm.UpdateData(_validationReport), null);
            }
        }

        /// <summary>
        /// Updates the readings in the health report.
        /// </summary>
        public void UpdateHealthReport(DateTime timestamp)
        {
            if (_healthReport.IsInitialized)
            {
                _healthReport.LastModified = DateTime.Now;
                DU.AddHealthReportReadings(_healthReport, timestamp);
                foreach (var vm in HealthReportViewModels)
                {
                    _uiContext?.Send(x => vm.SystemLoadChart.UpdateData(_healthReport.Ram, _healthReport.Cpu), null);
                    _uiContext?.Send(x => vm.NetworkChart.UpdateData(_healthReport.Ram, _healthReport.Cpu), null);

                }
            }
            else
            {
                DU.BuildHealthReport(_healthReport);
            }
        }

        public void UpdateManagerOverview()
        {
            DU.AddManagerReadings(_conversion.ActiveExecution);
            foreach (var vm in ManagerViewModels)
            {
                _uiContext?.Send(x => vm.UpdateData(_conversion.ActiveExecution.Managers), null);
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
                    if (!_conversion.IsInitialized)
                    {
                        _conversion.Executions = DU.GetExecutions();
                        _log = _conversion.ActiveExecution.Log;
                        _validationReport = _conversion.ActiveExecution.ValidationReport;
                        _healthReport = _conversion.HealthReport;
                        DU.AddManagers(_conversion.Executions);
                        DU.BuildHealthReport(_healthReport);
                        _conversion.IsInitialized = true;
                    }
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
            ConnectDBDialog dialogPopup = new(UserSettings);
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
                    Task.Run(() => UpdateManagerOverview());
                }, null, 500, UserSettings.AllQueryInterval * 1000));
            }
            else
            {
                _timers.Add(new Timer(x => UpdateHealthReport(_healthReport.LastModified), null, 500, UserSettings.HealthReportQueryInterval * 1000));
                _timers.Add(new Timer(x => UpdateLog(_log.LastModified), null, 500, UserSettings.LoggingQueryInterval * 1000));
                _timers.Add(new Timer(x => UpdateValidationReport(_validationReport.LastModified), null, 500, UserSettings.ValidationQueryInterval * 1000));
                _timers.Add(new Timer(x => UpdateManagerOverview(),null, 500, UserSettings.ManagerQueryInterval * 1000));
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
