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
using System.Diagnostics;
using System.Windows.Threading;
using System.ComponentModel;
using System.Linq;

namespace DashboardFrontend
{
    public class Controller
    {
        public Controller(MainWindowViewModel viewModel)
        {
            TryLoadUserSettings();
            _vm = viewModel;
            Conversion = new();
            _timers = new List<Timer>();
        }

        public bool IsUpdatingExecutions { get; private set; }
        public bool IsUpdatingLog { get; private set; }
        public bool IsUpdatingValidations { get; private set; }
        public bool IsUpdatingManagers { get; private set; }
        public bool IsUpdatingHealthReport { get; private set; }

        private readonly MainWindowViewModel _vm;
        private readonly List<Timer> _timers;
        public Conversion Conversion { get; set; }
        public readonly List<HealthReportViewModel> HealthReportViewModels = new();
        public readonly List<LogViewModel> LogViewModels = new();
        public readonly List<ValidationReportViewModel> ValidationReportViewModels = new();
        public readonly List<ManagerViewModel> ManagerViewModels = new();
        public UserSettings UserSettings { get; set; } = new();

        /// <summary>
        /// Initializes the view models in the <see cref="Controller"/>.
        /// </summary>
        public void InitializeViewModels(ListView listViewLog)
        {
            _vm.LogViewModel = new LogViewModel(listViewLog);
            LogViewModels.Add(_vm.LogViewModel);

            _vm.ValidationReportViewModel = new ValidationReportViewModel();
            ValidationReportViewModels.Add(_vm.ValidationReportViewModel);

            _vm.HealthReportViewModel = new HealthReportViewModel();
            HealthReportViewModels.Add(_vm.HealthReportViewModel);

            _vm.ManagerViewModel = new ManagerViewModel(Conversion.HealthReport);
            ManagerViewModels.Add(_vm.ManagerViewModel);
        }

        public LogViewModel CreateLogViewModel()
        {
            LogViewModel result = new();
            result.UpdateData(Conversion.ActiveExecution.Log);
            LogViewModels.Add(result);
            return result;
        }

        public ValidationReportViewModel CreateValidationReportViewModel()
        {
            ValidationReportViewModel result = new();
            result.UpdateData(Conversion.ActiveExecution.ValidationReport);
            ValidationReportViewModels.Add(result);
            return result;
        }

        public HealthReportViewModel CreateHealthReportViewModel()
        {
            HealthReportViewModel result = new();
            result.SystemLoadChart.UpdateData(Conversion.HealthReport.Ram, Conversion.HealthReport.Cpu);
            result.NetworkChart.UpdateData(Conversion.HealthReport.Network);
            result.NetworkDeltaChart.UpdateData(Conversion.HealthReport.Network);
            result.NetworkSpeedChart.UpdateData(Conversion.HealthReport.Network);
            HealthReportViewModels.Add(result);
            return result;
        }

        public ManagerViewModel CreateManagerViewModel()
        {
            ManagerViewModel result = new(Conversion.HealthReport);
            result.UpdateData(Conversion.ActiveExecution.Managers);
            ManagerViewModels.Add(result);
            return result;
        }

        /// <summary>
        /// Updates the messages in the log.
        /// </summary>
        public void UpdateLog()
        {
            if (IsUpdatingLog || Conversion.ActiveExecution?.Log is null)
            {
                return;
            }
            IsUpdatingLog = true;
            List<LogMessage> newData = DU.GetLogMessages(Conversion.LastLogUpdate);
            Conversion.LastLogUpdate = DateTime.Now;

            if (newData.Count > 0)
            {
                foreach (Execution execution in Conversion.Executions)
                {
                    execution.Log.Messages = newData.Where(m => m.ExecutionId == execution.Id).ToList();
                }

                foreach (LogViewModel vm in LogViewModels)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        vm.UpdateData(Conversion.ActiveExecution.Log);
                    });
                }
            }
            IsUpdatingLog = false;
        }

        /// <summary>
        /// Updates the validation tests in the validation report.
        /// </summary>
        public void UpdateValidationReport()
        {
            if(IsUpdatingValidations || Conversion.ActiveExecution?.ValidationReport is null)
            {
                return;
            }
            IsUpdatingValidations = true;
            List<ValidationTest> newData = DU.GetAfstemninger(Conversion.ActiveExecution.ValidationReport.LastModified);
            Conversion.ActiveExecution.ValidationReport.LastModified = DateTime.Now;
            if (newData.Count > 0)
            {
                Conversion.ActiveExecution.ValidationReport.ValidationTests = newData;
                foreach (var vm in ValidationReportViewModels)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        vm.UpdateData(Conversion.ActiveExecution.ValidationReport);
                    });
                }
            }
            IsUpdatingValidations = false;
        }

        /// <summary>
        /// Updates the readings in the health report.
        /// </summary>
        public void UpdateHealthReport()
        {
            if (IsUpdatingHealthReport || Conversion.HealthReport is null)
            {
                return;
            }
            IsUpdatingHealthReport = true;
            if (Conversion.HealthReport.IsInitialized)
            {
                DU.AddHealthReportReadings(Conversion.HealthReport, Conversion.HealthReport.LastModified);
                foreach (var vm in HealthReportViewModels)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        vm.SystemLoadChart.UpdateData(Conversion.HealthReport.Ram, Conversion.HealthReport.Cpu);
                        vm.NetworkChart.UpdateData(Conversion.HealthReport.Network);
                        vm.NetworkDeltaChart.UpdateData(Conversion.HealthReport.Network);
                        vm.NetworkSpeedChart.UpdateData(Conversion.HealthReport.Network);
                    });
                }
            }
            else
            {
                DU.BuildHealthReport(Conversion.HealthReport);
            }
            IsUpdatingHealthReport = false;
        }

        public void UpdateManagerOverview()
        {
            if (IsUpdatingManagers || Conversion.ActiveExecution is null)
            {
                return;
            }
            IsUpdatingManagers = true;

            foreach (var execution in Conversion.Executions)
            {
                // Get all managers since the last update
                List<Manager> managers = DU.GetAndUpdateManagers(Conversion.LastManagerUpdate, Conversion.AllManagers);
                Conversion.LastManagerUpdate = DateTime.Now;

                foreach (Manager manager in managers)
                {
                    // Check if we can assign context ID's to any of the managers
                    LogMessage? managerStartedMessage = execution.Log.Messages.Find(m => m.Content.Split(',')[0] == $"Starting manager: {manager.Name}");
                    if (managerStartedMessage != null)
                    {
                        manager.ContextId = managerStartedMessage.ContextId;
                        Execution? exec = Conversion.Executions.Find(e => e.Id == managerStartedMessage.ExecutionId);
                        if (exec != null && !exec.Managers.Contains(manager))
                        {
                            exec.Managers.Add(manager);
                        }
                    }

                    // Check for health report data associated with the manager
                    List<CpuLoad> cpuReadings = Conversion.HealthReport.Cpu.Readings.Where(r => r.Date >= manager.StartTime && r.Date <= manager.EndTime).ToList();
                    List<RamLoad> ramReadings = Conversion.HealthReport.Ram.Readings.Where(r => r.Date >= manager.StartTime && r.Date <= manager.EndTime).ToList();
                }
            }

            foreach (var vm in ManagerViewModels)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    vm.UpdateData(Conversion.ActiveExecution.Managers);
                });
            }
            IsUpdatingManagers = false;
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

        private void UpdateExecutions()
        {
            if (IsUpdatingExecutions)
            {
                return;
            }
            IsUpdatingExecutions = true;
            List<Execution> result = DU.GetExecutions(Conversion.LastExecutionUpdate);
            Conversion.LastExecutionUpdate = DateTime.Now;
            if (result.Count > 0)
            {
                Conversion.Executions.AddRange(result);
            }
            IsUpdatingExecutions = false;
        }

        /// <summary>
        /// Sets up the necessary query timers that gather and update information in the system.
        /// </summary>
        private void StartMonitoring()
        {
            _timers.Add(new Timer(x => { UpdateExecutions(); }, null, 0, 5000));
            _timers.Add(new Timer(x => { UpdateHealthReport(); }, null, 500, 5000));

            if (UserSettings.SynchronizeAllQueries)
            {
                _timers.Add(new Timer(x =>
                {
                    Task.Run(() => UpdateHealthReport());
                    Task.Run(() => UpdateLog());
                    Task.Run(() => UpdateValidationReport());
                    Task.Run(() => UpdateManagerOverview());
                }, null, 1000, UserSettings.AllQueryInterval * 1000));
            }
            else
            {
                _timers.Add(new Timer(x => UpdateHealthReport(), null, 1000, UserSettings.HealthReportQueryInterval * 1000));
                _timers.Add(new Timer(x => UpdateLog(), null, 1000, UserSettings.LoggingQueryInterval * 1000));
                _timers.Add(new Timer(x => UpdateValidationReport(), null, 1000, UserSettings.ValidationQueryInterval * 1000));
                _timers.Add(new Timer(x => UpdateManagerOverview(), null, 1000, UserSettings.ManagerQueryInterval * 1000));
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