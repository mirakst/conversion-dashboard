﻿using DashboardBackend.Database;
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
        private readonly Queue<LogMessage> _logParseQueue = new();
        private readonly MainWindowViewModel _vm;
        private readonly List<Timer> _timers;
        public Conversion? Conversion { get; set; }
        public List<HealthReportViewModel> HealthReportViewModels { get; private set; } = new();
        public List<LogViewModel> LogViewModels { get; private set; } = new();
        public List<ValidationReportViewModel> ValidationReportViewModels { get; private set; } = new();
        public List<ManagerViewModel> ManagerViewModels { get; private set; } = new();
        public UserSettings UserSettings { get; set; } = new();

        /// <summary>
        /// Initializes the view models in the <see cref="Controller"/>.
        /// </summary>
        public void InitializeViewModels(ListView listViewLog)
        {
            _vm.LogViewModel = new LogViewModel(listViewLog);
            LogViewModels = new() { _vm.LogViewModel };

            _vm.ValidationReportViewModel = new ValidationReportViewModel();
            ValidationReportViewModels = new() { _vm.ValidationReportViewModel };

            _vm.HealthReportViewModel = new HealthReportViewModel();
            HealthReportViewModels = new() { _vm.HealthReportViewModel };

            _vm.ManagerViewModel = new ManagerViewModel();
            ManagerViewModels = new() { _vm.ManagerViewModel };
        }

        public LogViewModel CreateLogViewModel()
        {
            LogViewModel result = new();
            if (Conversion != null && Conversion.Executions.Any())
            {
                result.UpdateData(Conversion.ActiveExecution.Log);
            }
            LogViewModels.Add(result);
            return result;
        }

        public ValidationReportViewModel CreateValidationReportViewModel()
        {
            ValidationReportViewModel result = new();
            if (Conversion != null && Conversion.Executions.Any())
            {
                result.UpdateData(Conversion.ActiveExecution.ValidationReport);
            }
            ValidationReportViewModels.Add(result);
            return result;
        }

        public HealthReportViewModel CreateHealthReportViewModel()
        {
            HealthReportViewModel result = new();
            if (Conversion?.HealthReport != null)
            {
                result.SystemLoadChart.UpdateData(Conversion.HealthReport.Ram, Conversion.HealthReport.Cpu);
                result.NetworkChart.UpdateData(Conversion.HealthReport.Network);
                result.NetworkDeltaChart.UpdateData(Conversion.HealthReport.Network);
                result.NetworkSpeedChart.UpdateData(Conversion.HealthReport.Network);
            }
            HealthReportViewModels.Add(result);
            return result;
        }

        public ManagerViewModel CreateManagerViewModel()
        {
            ManagerViewModel result = new();
            if (Conversion != null && Conversion.Executions.Any())
            {
                result.UpdateData(Conversion.ActiveExecution.Managers);
            }
            ManagerViewModels.Add(result);
            return result;
        }

        /// <summary>
        /// Updates the messages in the log.
        /// </summary>
        public void UpdateLog()
        {
            if (IsUpdatingLog || Conversion?.ActiveExecution?.Log is null)
            {
                return;
            }
            IsUpdatingLog = true;

            List<LogMessage> newData = DU.GetLogMessages(Conversion.LastLogUpdate);
            Conversion.LastLogUpdate = DateTime.Now;

            if (newData.Count > 0)
            {
                newData.ForEach(m =>
                {
                    Execution? exec = Conversion.Executions.Find(e => e.Id == m.ExecutionId);
                    if (exec is null)
                    {
                        // The first log message of an execution was logged before the execution was created
                        exec = new Execution(m.ExecutionId, m.Date);
                        Conversion.AddExecution(exec);
                    }
                    exec.Log.Messages.Add(m);
                    _logParseQueue.Enqueue(m);
                });

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
        /// Attempts to find the manager associated with a "Starting manager: "-message, and assigns its context ID if it has not already been set.
        /// The method also updates the status of managers when possible.
        /// </summary>
        /// <param name="message">The log message to parse.</param>
        private void ParseLogMessage(LogMessage message)
        {
            if (Conversion?.Executions.Find(e => e.Id == message.ExecutionId) is Execution exec)
            {
                if (message.Content.StartsWith("Starting manager:"))
                {
                    Match match = Regex.Match(message.Content, @"^Starting manager: (?<Name>[\w.]*)");
                    if (match.Success)
                    {
                        string name = match.Groups["Name"].Value;
                        // Find all managers with the name parsed from the log message
                        List<Manager> mgrs = Conversion.AllManagers.FindAll(m => m.Name == name);
                        if (mgrs.Any())
                        {
                            // Check if any of the managers are created from ENGINE_PROPERTIES (context ID=0)
                            if (mgrs.Find(m => m.ContextId == 0) is Manager manager)
                            {
                                manager.ContextId = message.ContextId;
                                if (!exec.Managers.Contains(manager))
                                {
                                    exec.Managers.Add(manager);
                                }
                            }
                        }
                        // If a manager with the found name doesn't exist, create it
                        else
                        {
                            Manager manager = new()
                            {
                                Name = name,
                                ContextId = message.ContextId,
                                Status = Manager.ManagerStatus.Running
                            };
                            exec.Managers.Add(manager);
                            Conversion.AllManagers.Add(manager);
                        }
                    }
                }
                // Check if a manager has finished its execution
                else if (message.Content == "Manager execution done.")
                {
                    if (exec.Managers.Find(m => m.ContextId == message.ContextId) is Manager mgr)
                    {
                        mgr.Status = Manager.ManagerStatus.Ok;
                        if (!mgr.EndTime.HasValue)
                        {
                            mgr.EndTime = message.Date;
                        }
                    }
                }
                // Check if an execution might have ended
                else if (message.Content.StartsWith("Program closing due to the following error:")
                    || message.Content == "Exiting from GuiManager..."
                    || message.Content == "No managers left to start automatically for BATCH"
                    || message.Content == "Deploy is finished!!")
                {
                    exec.Status = Execution.ExecutionStatus.Finished;
                    exec.EndTime = message.Date;
                }
            }
        }

        /// <summary>
        /// Updates the validation tests in the validation report.
        /// </summary>
        public void UpdateValidationReport()
        {
            if(IsUpdatingValidations || Conversion?.ActiveExecution?.ValidationReport is null)
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
            if (Conversion?.HealthReport is null)
            {
                return;
            }
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
        }

        /// <summary>
        /// Updates the list of managers in the current Conversion and adds them to their associated executions.
        /// </summary>
        public void UpdateManagerOverview()
        {
            if (Conversion?.ActiveExecution is null)
            {
                return;
            }

            DU.GetAndUpdateManagers(Conversion.LastManagerUpdate, Conversion.AllManagers);
            Conversion.LastManagerUpdate = DateTime.Now;

            while (_logParseQueue.Any())
            {
                ParseLogMessage(_logParseQueue.Dequeue());
            }

            // Check for any manager values that can be updated
            Conversion.AllManagers.ForEach(m =>
            {
                //List<CpuLoad> cpuReadings = Conversion.HealthReport.Cpu.Readings.Where(r => r.Date >= m.StartTime && r.Date <= m.EndTime).ToList();
                //List<RamLoad> ramReadings = Conversion.HealthReport.Ram.Readings.Where(r => r.Date >= m.StartTime && r.Date <= m.EndTime).ToList();
                //m.AddReadings(cpuReadings, ramReadings);
            });

            foreach (var vm in ManagerViewModels)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    vm.UpdateData(Conversion.ActiveExecution.Managers);
                });
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
                    if (!UserSettings.ActiveProfile.HasEventListeners())
                    {
                        UserSettings.ActiveProfile.ProfileChanged += Reset;
                    }
                    Task task = new(StartMonitoring);
                    task.Start();
                    UserSettings.ActiveProfile.HasStartedMonitoring = true;
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
        /// Updates the list of executions in the current conversion.
        /// </summary>
        private void UpdateExecutions()
        {
            if (IsUpdatingExecutions || Conversion is null)
            {
                return;
            }
            IsUpdatingExecutions = true;
            List<Execution> result = DU.GetExecutions(Conversion.LastExecutionUpdate);
            Conversion.LastExecutionUpdate = DateTime.Now;
            if (result.Count > 0)
            {
                foreach(Execution newExec in result)
                {
                    if (!Conversion.Executions.Any(e => e.Id == newExec.Id))
                    {
                        Conversion.AddExecution(newExec);
                    }
                }
            }
            IsUpdatingExecutions = false;
        }

        /// <summary>
        /// Sets up the necessary query timers that gather and update information in the system.
        /// </summary>
        private void StartMonitoring()
        {
            _vm.IsRunning = true;
            bool shouldUpdateLog = false;
            bool shouldUpdateExecutions = false;
            bool shouldUpdateHealthReport = false;
            bool shouldUpdateValidations = false;
            bool shouldUpdateManagers = false;

            Timer logTimer = new(x => shouldUpdateLog = true, null, 0, UserSettings.LoggingQueryInterval * 1000);
            Timer execTimer = new(x => shouldUpdateExecutions = true, null, 0, 10000);
            Timer healthReportTimer = new(x => shouldUpdateHealthReport = true, null, 0, UserSettings.HealthReportQueryInterval * 1000);
            Timer validationsTimer = new(x => shouldUpdateValidations = true, null, 0, UserSettings.ValidationQueryInterval * 1000);
            Timer managerTimer = new(x => shouldUpdateManagers = true, null, 0, UserSettings.ManagerQueryInterval * 1000);

            UpdateExecutions();
            DU.BuildHealthReport(Conversion?.HealthReport);

            // mål performance for hver blok
            while (_vm.IsRunning)
            {
                if (shouldUpdateLog)
                {
                    Trace.WriteLine("Updating log");
                    UpdateLog();
                    shouldUpdateLog = false;
                }
                if (shouldUpdateManagers)
                {
                    Trace.WriteLine("Updating managers");
                    UpdateManagerOverview();
                    shouldUpdateManagers = false;
                }
                if (shouldUpdateValidations)
                {
                    Trace.WriteLine("Updating validations");
                    UpdateValidationReport();
                    shouldUpdateValidations = false;
                }
                if (shouldUpdateHealthReport)
                {
                    Trace.WriteLine("Updating health report");
                    UpdateHealthReport();
                    shouldUpdateHealthReport = false;
                }
                if (shouldUpdateExecutions)
                {
                    Trace.WriteLine("Updating executions yo");
                    UpdateExecutions();
                    shouldUpdateExecutions = false;
                }
            }

            //_timers.Add(new Timer(x => { UpdateExecutions(); }, null, 0, 10000));

            //if (UserSettings.SynchronizeAllQueries)
            //{
            //    _timers.Add(new Timer(x =>
            //    {
            //        Task.Run(() => UpdateHealthReport());
            //        Task.Run(() => UpdateLog());
            //        Task.Run(() => UpdateValidationReport());
            //        Task.Run(() => UpdateManagerOverview());
            //    }, null, 1000, UserSettings.AllQueryInterval * 1000));
            //}
            //else
            //{
            //    _timers.Add(new Timer(x => UpdateHealthReport(), null, 1000, UserSettings.HealthReportQueryInterval * 1000));
            //    _timers.Add(new Timer(x => UpdateValidationReport(), null, 1000, UserSettings.ValidationQueryInterval * 1000));
            //    _timers.Add(new Timer(x => UpdateLog(), null, 1000, UserSettings.LoggingQueryInterval * 1000));
            //    _timers.Add(new Timer(x => UpdateManagerOverview(), null, 1000, UserSettings.ManagerQueryInterval * 1000));
            //}
            //_vm.IsRunning = true;
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

        internal void Reset()
        {
            StopMonitoring();
            Conversion = new();
            InitializeViewModels(LogViewModels[0].LogListView);
            _vm.UpdateView();
        }
    }
}