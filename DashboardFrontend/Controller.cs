using DashboardBackend.Database;
using DashboardFrontend.DetachedWindows;
using DashboardFrontend.Settings;
using DashboardFrontend.ViewModels;
using Model;
using System;
using System.Collections.Generic;
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
        public Controller()
        {
            Conversion = new();
            LogParseQueue = new();
        }

        public Controller(MainWindowViewModel viewModel) : this()
        {
            _vm = viewModel;
            _timers = new List<Timer>();
            HealthReportViewModels = new();
            LogViewModels = new();
            ValidationReportViewModels = new();
            ManagerViewModels = new();
            UserSettings = new();
            TryLoadUserSettings();
        }

        private enum DashboardStatus
        {
            Idle, UpdatingManagers, UpdatingLog, UpdatingExecutions, UpdatingHealthReport, UpdatingValidations, 
        }
        private readonly Dictionary<DashboardStatus, string> _statusMessages = new()
        {
            { DashboardStatus.Idle, "" },
            { DashboardStatus.UpdatingLog, "Querying log..." },
            { DashboardStatus.UpdatingManagers, "Querying managers..." },
            { DashboardStatus.UpdatingExecutions, "Querying executions..." },
            { DashboardStatus.UpdatingHealthReport, "Querying Health Report..." },
            { DashboardStatus.UpdatingValidations, "Querying validations..." },
        };

        private readonly MainWindowViewModel _vm;
        private readonly List<Timer> _timers;

        public Queue<LogMessage> LogParseQueue { get; private set; }
        public bool ShouldUpdateExecutions { get; private set; }
        public bool ShouldUpdateLog { get; private set; }
        public bool ShouldUpdateValidations { get; private set; }
        public bool ShouldUpdateManagers { get; private set; }
        public bool ShouldUpdateHealthReport { get; private set; }
        public Conversion? Conversion { get; set; }
        public List<HealthReportViewModel> HealthReportViewModels { get; private set; }
        public List<LogViewModel> LogViewModels { get; private set; }
        public List<ValidationReportViewModel> ValidationReportViewModels { get; private set; }
        public List<ManagerViewModel> ManagerViewModels { get; private set; }
        public UserSettings UserSettings { get; set; }

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
                result.UpdateData(Conversion.Executions);
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

        public void UpdateEstimatedManagerCounts()
        {
            if (Conversion != null)
            {
                foreach (Execution exec in Conversion.Executions.Where(e => e.EstimatedManagerCount == 0))
                {
                    int estimatedMgrCount = DU.GetEstimatedManagerCount(exec.Id);
                    if (estimatedMgrCount == 0)
                    {
                        if (exec.Log?.Messages.Find(m => m.Content.StartsWith("Loaded managerclasses:")) is LogMessage msg)
                        {
                            estimatedMgrCount = msg.Content.Split("\n").Length - 1;
                        }
                    }
                    exec.EstimatedManagerCount = estimatedMgrCount;
                }
            }

        }

        /// <summary>
        /// Updates the messages in the log.
        /// </summary>
        public void UpdateLog()
        {
            if (Conversion is null)
            {
                return;
            }
            SetStatusMessage(DashboardStatus.UpdatingLog);

            List<LogMessage> newData = DU.GetLogMessages(Conversion.LastLogQuery);
            Conversion.LastLogQuery = DateTime.Now;

            if (newData.Count > 0)
            {
                newData.ForEach(m =>
                {
                    Execution? exec = Conversion.Executions.Find(e => e.Id == m.ExecutionId);
                    if (exec is null)
                    {
                        // The first log message of an execution was logged before the execution was created
                        exec = new Execution(m.ExecutionId, m.Date);
                        if (_vm is not null)
                        {
                            exec.OnExecutionProgressUpdated += _vm.UpdateExecutionProgress;
                        }
                        Conversion.AddExecution(exec);
                    }
                    exec.Log.Messages.Add(m);
                    LogParseQueue.Enqueue(m);
                });
                Conversion.LastLogUpdated = DateTime.Now;
            }
            ClearStatusMessage(DashboardStatus.UpdatingLog);
        }

        /// <summary>
        /// Attempts to find the manager associated with a "Starting manager: "-message, and assigns its context ID if it has not already been set.
        /// The method also updates the status of managers when possible.
        /// </summary>
        /// <param name="message">The log message to parse.</param>
        public void ParseLogMessage(LogMessage message)
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
                                    exec.AddManager(manager);
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
                            exec.AddManager(manager);
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
            if(Conversion is null)
            {
                return;
            }
            SetStatusMessage(DashboardStatus.UpdatingValidations);
          
            List<ValidationTest> newData = DU.GetAfstemninger(Conversion.LastValidationsQuery);
            Conversion.LastValidationsQuery = DateTime.Now;

            if (newData.Any())
            {
                newData.ForEach(v => 
                {
                    if (Conversion.AllManagers.Find(m => m.Name.Contains(v.ManagerName) && v.Date < m.EndTime) is Manager mgr)
                    {
                        mgr.Validations.Add(v);
                    }
                });
                Conversion.LastValidationsUpdated = DateTime.Now;
            }
            ClearStatusMessage(DashboardStatus.UpdatingValidations);
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
            SetStatusMessage(DashboardStatus.UpdatingHealthReport);

            if (Conversion.HealthReport.IsInitialized)
            {
                int dataCount = DU.AddHealthReportReadings(Conversion.HealthReport, Conversion.HealthReport.LastModified);
                if (dataCount > 0)
                {
                    Conversion.LastHealthReportUpdated = DateTime.Now;
                }
            }
            else
            {
                DU.BuildHealthReport(Conversion.HealthReport);
            }
            ClearStatusMessage(DashboardStatus.UpdatingHealthReport);
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
            SetStatusMessage(DashboardStatus.UpdatingManagers);

            int managerCount = DU.GetAndUpdateManagers(Conversion.LastManagerQuery, Conversion.AllManagers);
            Conversion.LastManagerQuery = DateTime.Now;

            while (LogParseQueue.Any())
            {
                ParseLogMessage(LogParseQueue.Dequeue());
                managerCount = 1;
            }

            // Check for any health report readings
            if (Conversion.HealthReport?.Cpu is not null && Conversion.HealthReport?.Ram is not null)
            {
                Conversion.AllManagers.ForEach(m =>
                {
                    List<CpuLoad> cpuReadings = Conversion.HealthReport.Cpu.Readings.Where(r => r.Date >= m.StartTime && r.Date <= m.EndTime).ToList();
                    List<RamLoad> ramReadings = Conversion.HealthReport.Ram.Readings.Where(r => r.Date >= m.StartTime && r.Date <= m.EndTime).ToList();
                    m.AddReadings(cpuReadings, ramReadings);
                });
            }

            if (managerCount > 0)
            {
                Conversion.LastManagerUpdated = DateTime.Now;
            }
        }

        /// <summary>
        /// Updates the list of executions in the current conversion.
        /// </summary>
        private void UpdateExecutions()
        {
            if (Conversion is null)
            {
                return;
            }
            SetStatusMessage(DashboardStatus.UpdatingExecutions);
          
            List<Execution> result = DU.GetExecutions(Conversion.LastExecutionQuery);
            Conversion.LastExecutionQuery = DateTime.Now;
            if (result.Count > 0)
            {
                foreach(Execution newExec in result)
                {
                    if (!Conversion.Executions.Any(e => e.Id == newExec.Id))
                    {
                        newExec.OnExecutionProgressUpdated += _vm.UpdateExecutionProgress;
                        Conversion.AddExecution(newExec);
                    }
                }
            }
            UpdateEstimatedManagerCounts();
            ClearStatusMessage(DashboardStatus.UpdatingExecutions);
        }

        private async void ClearStatusMessage(DashboardStatus status, int delay = 1000)
        {
            if (_vm is not null)
            {
                await Task.Delay(delay);
                if (_vm.CurrentStatus == _statusMessages[status])
                {
                    _vm.CurrentStatus = _statusMessages[DashboardStatus.Idle];
                }
            }

        }

        private void SetStatusMessage(DashboardStatus status)
        {
            if (_vm is not null)
            {
                _vm.CurrentStatus = _statusMessages[status];
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
                    Task monitoring = new(StartMonitoring);
                    Task updateViews = new(UpdateViews);
                    monitoring.Start();
                    updateViews.Start();
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
        /// Gathers initial information (Executions, health report info) and then starts the query timers that determine when to fetch data for the individual parts of the Conversion.
        /// The main loop ensures that data is processed when appropriate, as determined by the timers.
        /// </summary>
        /// <remarks><see cref="Thread.Sleep(int)"/> is used to drastically reduce the number of times the loop is executed, lowering the overall CPU load.</remarks>
        private void StartMonitoring()
        {
            _vm.IsRunning = true;
            UpdateExecutions();
            DU.BuildHealthReport(Conversion?.HealthReport);
            SetupTimers();

            while (_vm.IsRunning)
            {
                if (ShouldUpdateLog)
                {
                    UpdateLog();
                    ShouldUpdateLog = false;
                }
                if (ShouldUpdateManagers)
                {
                    UpdateManagerOverview();
                    ShouldUpdateManagers = false;
                }
                if (ShouldUpdateValidations)
                {
                    UpdateValidationReport();
                    ShouldUpdateValidations = false;
                }
                if (ShouldUpdateHealthReport)
                {
                    UpdateHealthReport();
                    ShouldUpdateHealthReport = false;
                }
                if (ShouldUpdateExecutions)
                {
                    UpdateExecutions();
                    ShouldUpdateExecutions = false;
                }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Runs continuously while monitoring, and ensures that the appropriate viewmodels are updated when new data is added to the Conversion. 
        /// </summary>
        /// <remarks><see cref="Thread.Sleep(int)"/> is used to drastically reduce the number of times the loop is executed, lowering the overall CPU load.</remarks>
        private void UpdateViews()
        {
            while (_vm.IsRunning)
            {
                if (Conversion?.ActiveExecution is null)
                {
                    continue;
                }
                foreach (ValidationReportViewModel vm in ValidationReportViewModels)
                {
                    if (vm.LastUpdated <= Conversion.LastValidationsUpdated)
                    {
                        vm.LastUpdated = DateTime.Now;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            vm.UpdateData(Conversion.Executions);
                        });
                    }
                }
                foreach (LogViewModel vm in LogViewModels)
                {
                    if (vm.LastUpdated <= Conversion.LastLogUpdated)
                    {
                        vm.LastUpdated = DateTime.Now;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            vm.UpdateData(Conversion.ActiveExecution.Log);
                        });
                    }
                }
                foreach (ManagerViewModel vm in ManagerViewModels)
                {
                    if (vm.LastUpdated <= Conversion?.LastManagerUpdated)
                    {
                        vm.LastUpdated = DateTime.Now;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            vm.UpdateData(Conversion.ActiveExecution.Managers);
                        });
                    }
                }
                foreach (HealthReportViewModel vm in HealthReportViewModels)
                {
                    if (vm.LastUpdated <= Conversion.LastHealthReportUpdated)
                    {
                        vm.LastUpdated = DateTime.Now;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            vm.SystemLoadChart.UpdateData(Conversion.HealthReport.Ram, Conversion.HealthReport.Cpu);
                            vm.NetworkChart.UpdateData(Conversion.HealthReport.Network);
                            vm.NetworkDeltaChart.UpdateData(Conversion.HealthReport.Network);
                            vm.NetworkSpeedChart.UpdateData(Conversion.HealthReport.Network);
                        });
                    }
                }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Creates and adds query timers which start immediately to the <see cref="_timers"/> list.
        /// </summary>
        private void SetupTimers()
        {
            if (UserSettings.SynchronizeAllQueries)
            {
                _timers.Add(new Timer(x =>
                {
                    ShouldUpdateLog = true;
                    ShouldUpdateExecutions = true;
                    ShouldUpdateHealthReport = true;
                    ShouldUpdateValidations = true;
                    ShouldUpdateManagers = true;

                }, null, 0, UserSettings.AllQueryInterval * 1000));
            }
            else
            {
                _timers.Add(new Timer(x => ShouldUpdateLog = true, null, 0, UserSettings.LoggingQueryInterval * 1000));
                _timers.Add(new Timer(x => ShouldUpdateExecutions = true, null, 0, 10000));
                _timers.Add(new Timer(x => ShouldUpdateHealthReport = true, null, 0, UserSettings.HealthReportQueryInterval * 1000));
                _timers.Add(new Timer(x => ShouldUpdateValidations = true, null, 0, UserSettings.ValidationQueryInterval * 1000));
                _timers.Add(new Timer(x => ShouldUpdateManagers = true, null, 0, UserSettings.ManagerQueryInterval * 1000));
            }
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
            _vm.CurrentStatus = _statusMessages[DashboardStatus.Idle];
            _vm.UpdateView();
            _vm.CurrentProgress = 0;
        }
    }
}