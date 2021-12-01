using DashboardBackend.Database;
using DashboardFrontend.DetachedWindows;
using DashboardFrontend.Settings;
using DashboardFrontend.ViewModels;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DU = DashboardBackend.DataUtilities;

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
            result.UpdateData(Conversion.ActiveExecution.ManagerDict);
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
                Trace.WriteLine("updated log");
                foreach (LogMessage message in newData)
                {
                    ParseLogMessage(message);
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

        private void ParseLogMessage(LogMessage message)
        {
            Execution? exec = Conversion.Executions.Find(e => e.Id == message.ExecutionId);
            if (exec != null)
            {
                if (message.Content.StartsWith("Starting manager:"))
                {
                    // If the manager has already been set up, simply change its status
                    if (exec.ManagerDict.ContainsKey(message.ContextId))
                    {
                        exec.ManagerDict[message.ContextId].Status = Manager.ManagerStatus.Running;
                    }
                    // Otherwise, try to find the manager in the conversion's list of managers
                    // If it is not there either, it will be created
                    else
                    {
                        Match match = Regex.Match(message.Content, @"^Starting manager: (?<Name>[\w.]*)");
                        if (match.Success)
                        {
                            string name = match.Groups["Name"].Value;
                            Manager? manager = Conversion.AllManagers.Find(m => m.Name == name);
                            if (manager is not null)
                            {
                                manager.Status = Manager.ManagerStatus.Running;
                                exec.ManagerDict.Add(message.ContextId, manager);
                            }
                            else
                            {
                                manager = new() { Name = name, Status = Manager.ManagerStatus.Running };
                                exec.ManagerDict.Add(message.ContextId, manager);
                                Conversion.AllManagers.Add(manager);
                            }
                        }
                    }
                }
                else if (message.Content == "Manager execution done." && exec.ManagerDict.ContainsKey(message.ContextId))
                {
                    exec.ManagerDict[message.ContextId].Status = Manager.ManagerStatus.Ok;
                }
                exec.Log.Messages.Add(message);
            }

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

        /// <summary>
        /// Updates the list of managers in the current Conversion and adds them to their associated executions.
        /// </summary>
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
                    //Check if we can assign context ID's to any of the managers
                    //LogMessage? managerStartedMessage = execution.Log.Messages.Find(m => Regex.IsMatch(m.Content, $"^Starting manager: {manager.Name}.*$"));
                    //if (managerStartedMessage != null)
                    //{
                    //    Execution? exec = Conversion.Executions.Find(e => e.Id == managerStartedMessage.ExecutionId);
                    //    if (exec != null && !exec.ManagerDict.ContainsKey(managerStartedMessage.ContextId))
                    //    {
                    //        exec.ManagerDict[managerStartedMessage.ContextId] = manager;
                    //    }
                    //}

                    // Check for health report data associated with the manager
                    List<CpuLoad> cpuReadings = Conversion.HealthReport.Cpu.Readings.Where(r => r.Date >= manager.StartTime && r.Date <= manager.EndTime).ToList();
                    List<RamLoad> ramReadings = Conversion.HealthReport.Ram.Readings.Where(r => r.Date >= manager.StartTime && r.Date <= manager.EndTime).ToList();
                }
            }
            Trace.WriteLine("updated managers");


            foreach (var vm in ManagerViewModels)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    vm.UpdateData(Conversion.ActiveExecution.ManagerDict);
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