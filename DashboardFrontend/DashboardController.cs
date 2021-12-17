using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DashboardBackend;
using DashboardBackend.Settings;
using DashboardFrontend.DetachedWindows;
using DashboardFrontend.ViewModels;
using Model;

namespace DashboardFrontend
{
    /// <summary>
    /// A controller for the Dashboard system which requests data from the <see cref="IDataHandler"/> and updates views accordingly.
    /// </summary>
    public class DashboardController
    {
        private readonly MainWindowViewModel _viewModel;
        private readonly List<Timer> _timers;
        private readonly List<Reconciliation> _homelessReconciliations;
        private readonly object _updateLogLock;
        private readonly object _updateReconciliationsLock;
        private readonly object _updateManagersLock;
        private readonly object _updateExecutionsLock;
        private readonly object _updateHealthReportLock;
        private enum DashboardStatus
        {
            Idle, UpdatingManagers, UpdatingLog, UpdatingExecutions, UpdatingHealthReport, UpdatingReconciliations,
        }
        private readonly Dictionary<DashboardStatus, string> _statusMessages = new()
        {
            { DashboardStatus.Idle, "" },
            { DashboardStatus.UpdatingLog, "Querying log..." },
            { DashboardStatus.UpdatingManagers, "Querying managers..." },
            { DashboardStatus.UpdatingExecutions, "Querying executions..." },
            { DashboardStatus.UpdatingHealthReport, "Querying Health Report..." },
            { DashboardStatus.UpdatingReconciliations, "Querying reconciliations..." },
        };

        public DashboardController()
        {
            _timers = new();
            _updateLogLock = new();
            _updateManagersLock = new();
            _updateExecutionsLock = new();
            _updateReconciliationsLock = new();
            _updateHealthReportLock = new();
            _homelessReconciliations = new();

            DataHandler = new DataHandler();
            Conversion = new();
        }

        public DashboardController(MainWindowViewModel viewModel) : this()
        {
            _viewModel = viewModel;

            HealthReportViewModels = new();
            LogViewModels = new();
            ReconciliationReportViewModels = new();
            ManagerViewModels = new();
            UserSettings = new UserSettings();
            LoadUserSettings();
        }

        public DashboardController(MainWindowViewModel viewModel, IDataHandler dataHandler)
            : this(viewModel)
        {
            DataHandler = dataHandler;
        }

        public IDataHandler DataHandler { get; set; }
        public bool ShouldUpdateExecutions { get; private set; }
        public bool ShouldUpdateLog { get; private set; }
        public bool ShouldUpdateReconciliations { get; private set; }
        public bool ShouldUpdateManagers { get; private set; }
        public bool ShouldUpdateHealthReport { get; private set; }
        public Conversion? Conversion { get; set; }
        public List<HealthReportViewModel> HealthReportViewModels { get; private set; }
        public List<LogViewModel> LogViewModels { get; private set; }
        public List<ReconciliationReportViewModel> ReconciliationReportViewModels { get; private set; }
        public List<ManagerViewModel> ManagerViewModels { get; private set; }
        public IUserSettings UserSettings { get; set; }

        #region Viewmodels
        /// <summary>
        /// Initializes the view models in the <see cref="DashboardController"/>.
        /// </summary>
        public void InitializeViewModels(ListView listViewLog)
        {
            _viewModel.LogViewModel = new LogViewModel(listViewLog);
            LogViewModels = new() { _viewModel.LogViewModel };

            _viewModel.ReconciliationReportViewModel = new ReconciliationReportViewModel();
            ReconciliationReportViewModels = new() { _viewModel.ReconciliationReportViewModel };

            _viewModel.HealthReportViewModel = new HealthReportViewModel();
            HealthReportViewModels = new() { _viewModel.HealthReportViewModel };

            _viewModel.ManagerViewModel = new ManagerViewModel();
            ManagerViewModels = new() { _viewModel.ManagerViewModel };
        }

        /// <summary>
        /// Creates a log view model, and updates data for it, if data exists.
        /// </summary>
        /// <returns>The log view model.</returns>
        public LogViewModel CreateLogViewModel()
        {
            LogViewModel result = new();
            if (Conversion != null && Conversion.Executions.Any())
            {
                result.UpdateData(Conversion.Executions);
            }
            LogViewModels.Add(result);
            return result;
        }

        /// <summary>
        /// Creates a reconciliation report view model, and updates data for it, if data exists.
        /// </summary>
        /// <returns>The reconciliation report view model.</returns>
        public ReconciliationReportViewModel CreateReconciliationReportViewModel()
        {
            ReconciliationReportViewModel result = new();
            if (Conversion != null && Conversion.Executions.Any())
            {
                result.UpdateData(Conversion.Executions);
            }
            ReconciliationReportViewModels.Add(result);
            return result;
        }

        /// <summary>
        /// Creates a health report view model, and updates data for it, if data exists.
        /// </summary>
        /// <returns>The health report view model.</returns>
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

        /// <summary>
        /// Creates a manager view model, and updates data for it, if data exists.
        /// </summary>
        /// <returns>The manager view model.</returns>
        public ManagerViewModel CreateManagerViewModel()
        {
            ManagerViewModel result = new();
            if (Conversion != null && Conversion.Executions.Any())
            {
                result.UpdateData(Conversion.Executions);
            }
            ManagerViewModels.Add(result);
            return result;
        }
        #endregion

        /// <summary>
        /// Updates the estimated count of managers for each execution.
        /// </summary>
        public void UpdateEstimatedManagerCounts()
        {
            if (Conversion != null)
            {
                foreach (Execution exec in Conversion.Executions.Where(e => e.EstimatedManagerCount == 0))
                {
                    int estimatedMgrCount = DataHandler.GetEstimatedManagerCount(exec.Id);
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
        /// Updates the Log in all executions and also creates/updates managers and executions.
        /// </summary>
        public void UpdateLog()
        {
            lock (_updateLogLock)
            {
                if (Conversion is null)
                {
                    return;
                }
                SetStatusMessage(DashboardStatus.UpdatingLog);

                (List<LogMessage> messages, List<Manager> managers, List<Execution> executions) = DataHandler.GetParsedLogData(Conversion.LastLogQuery);
                Conversion.LastLogQuery = DateTime.Now;

                if (messages.Any())
                {
                    // Update executions
                    lock (Conversion.Executions)
                    {
                        foreach (Execution? newExecution in executions)
                        {
                            // If the execution already exists, update its status (since it might be finished)
                            if (Conversion.Executions.FirstOrDefault(e => e.Id == newExecution.Id) is Execution e)
                            {
                                e.Status = newExecution.Status;
                            }
                            // Otherwise, add it
                            else
                            {
                                if (_viewModel is not null)
                                {
                                    newExecution.OnExecutionProgressUpdated += _viewModel.UpdateExecutionProgress;
                                }
                                Conversion.AddExecution(newExecution);
                                Conversion.LastReconciliationsUpdated = DateTime.Now;
                            }
                        }
                    }
                    // Update managers
                    lock (Conversion.AllManagers)
                    {
                        foreach (Manager? newManager in managers)
                        {
                            // If the manager already exists, update its context and execution ID's
                            if (Conversion.AllManagers.Find(m => m.Name == newManager.Name && m.ContextId == 0) is Manager existingManager)
                            {
                                existingManager.ContextId = newManager.ContextId;
                                existingManager.ExecutionId = newManager.ExecutionId;
                            }
                            // Otherwise, add it
                            else
                            {
                                Conversion.AllManagers.Add(newManager);
                                if (Conversion.Executions.FirstOrDefault(e => e.Id == newManager.ExecutionId) is Execution execution)
                                {
                                    execution.AddManager(newManager);
                                }
                                else
                                {
                                    Trace.WriteLine($"Did not find execution for manager: {newManager.ShortName}, ContextID={newManager.ContextId}, ExecutionID={newManager.ExecutionId}");
                                }
                            }
                        }
                    }
                    // Also add new log messages to their respective execution's log
                    lock (Conversion.Executions)
                    {
                        foreach (Execution? execution in Conversion.Executions)
                        {
                            var execMessages = messages.Where(m => m.ExecutionId == execution.Id);
                            foreach (var msg in execMessages)
                            {
                                if (execution.Managers.Find(m => m.ContextId == msg.ContextId) is Manager mgr)
                                {
                                    msg.ManagerName = mgr.ShortName;
                                }
                            }
                            execution.Log.Messages.AddRange(messages.Where(m => m.ExecutionId == execution.Id));
                        }
                    }
                    Conversion.LastLogUpdated = DateTime.Now;
                }
                ClearStatusMessage(DashboardStatus.UpdatingLog);
            }
        }

        /// <summary>
        /// Gets and updates reconciliations for all managers.
        /// </summary>
        /// <remarks>If the manager associated with a reconciliation does not yet exist, it will be passed to a list of <see cref="_homelessReconciliations"/>, and they will be added when the manager exists.</remarks>
        public void UpdateReconciliations()
        {
            lock (_updateReconciliationsLock)
            {
                if (Conversion is null)
                {
                    return;
                }
                SetStatusMessage(DashboardStatus.UpdatingReconciliations);

                List<Reconciliation> newData = DataHandler.GetParsedReconciliations(Conversion.LastReconciliationsQuery);
                Conversion.LastReconciliationsQuery = DateTime.Now;
                newData.AddRange(_homelessReconciliations);
                _homelessReconciliations.Clear();

                if (newData.Any())
                {
                    newData.ForEach(v =>
                    {
                        // Try to find the associated manager (and ensure that it is in the right execution as well)
                        if (Conversion.AllManagers.Find(m => m.Name.Contains(v.ManagerName) && v.Date < m.EndTime) is Manager mgr)
                        {
                            mgr.AddReconciliation(v);
                        }
                        // Otherwise, the manager has not yet been created - it is passed to a list and we will try again next time
                        else
                        {
                            _homelessReconciliations.Add(v);
                        }
                    });
                    Conversion.LastReconciliationsUpdated = DateTime.Now;
                }
                ClearStatusMessage(DashboardStatus.UpdatingReconciliations);
            }
        }

        /// <summary>
        /// Updates the Health Report for the current Conversion.
        /// </summary>
        /// <remarks>Locks are used to prevent InvalidOperationExceptions when multiple threads work on the same collections.</remarks>
        public void UpdateHealthReport()
        {
            lock (_updateHealthReportLock)
            {
                if (Conversion is null)
                {
                    return;
                }
                SetStatusMessage(DashboardStatus.UpdatingHealthReport);

                Conversion.HealthReport = DataHandler.GetParsedHealthReport(Conversion.HealthReport.LastModified, Conversion.HealthReport);
                Conversion.HealthReport.LastModified = DateTime.Now;

                ClearStatusMessage(DashboardStatus.UpdatingHealthReport);
            }
        }

        /// <summary>
        /// Updates the list of managers in the current Conversion and adds them to their associated executions.
        /// </summary>
        /// <remarks>Locks are used to prevent InvalidOperationExceptions when multiple threads work on the same collections.</remarks>
        public void UpdateManagers()
        {
            lock (_updateManagersLock)
            {
                if (Conversion?.ActiveExecution is null)
                {
                    return;
                }
                SetStatusMessage(DashboardStatus.UpdatingManagers);

                List<Manager> managers = DataHandler.GetParsedManagers(Conversion.LastManagerQuery);
                Conversion.LastManagerQuery = DateTime.Now;

                lock (Conversion.AllManagers)
                {
                    if (managers.Any())
                    {
                        // Update managers
                        foreach (Manager? manager in managers)
                        {
                            // Check if the manager already exists but does not have parsed data yet (it may have been created from the log)
                            if (Conversion.AllManagers.Find(m => m.Name == manager.Name && m.IsMissingValues) is Manager existingManager)
                            {
                                // Ensure that we do not overwrite any existing values with null
                                if (!existingManager.StartTime.HasValue && manager.StartTime.HasValue)
                                {
                                    existingManager.StartTime = manager.StartTime.Value;
                                }
                                if (!existingManager.RowsRead.HasValue && manager.RowsRead.HasValue)
                                {
                                    existingManager.RowsRead = manager.RowsRead;
                                }
                                if (!existingManager.RowsWritten.HasValue && manager.RowsWritten.HasValue)
                                {
                                    existingManager.RowsWritten = manager.RowsWritten;
                                }
                                if (!existingManager.EndTime.HasValue && manager.EndTime.HasValue)
                                {
                                    existingManager.EndTime = manager.EndTime;
                                }
                            }
                            // If the manager does not exist, it is added to both the conversion and an appropriate execution.
                            else
                            {
                                Conversion.AllManagers.Add(manager);
                                lock (Conversion.Executions)
                                {
                                    if (Conversion.Executions.FirstOrDefault(e => (e.StartTime <= manager.StartTime) && (!e.EndTime.HasValue || e.EndTime >= manager.EndTime)) is Execution execution)
                                    {
                                        execution.Managers.Add(manager);
                                    }
                                    else
                                    {
                                        Trace.WriteLine("Did not find execution for manager " + manager.Name);
                                    }
                                }
                            }
                        }
                        Conversion.LastManagerUpdated = DateTime.Now;
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
                }
                ClearStatusMessage(DashboardStatus.UpdatingManagers);
            }
        }

        /// <summary>
        /// Updates the list of executions in the current conversion.
        /// </summary>
        /// <remarks>If a new execution was already created by the log, the existing one will simply be updated.
        /// Locks are used to prevent InvalidOperationExceptions when multiple threads work on the same collections.</remarks>
        public void UpdateExecutions()
        {
            lock (_updateExecutionsLock)
            {
                if (Conversion is null)
                {
                    return;
                }
                SetStatusMessage(DashboardStatus.UpdatingExecutions);

                List<Execution> executions = DataHandler.GetParsedExecutions(Conversion.LastExecutionQuery);
                Conversion.LastExecutionQuery = DateTime.Now;

                if (executions.Any())
                {
                    lock (Conversion.Executions)
                    {
                        foreach (Execution? execution in executions)
                        {
                            // If the execution already exists (it was created by the log), update its start time
                            if (Conversion.Executions.FirstOrDefault(e => e.Id == execution.Id) is Execution existingExecution)
                            {
                                existingExecution.StartTime = execution.StartTime;
                            }
                            // Otherwise, add it!
                            else
                            {
                                Conversion.AddExecution(execution);
                                if (_viewModel is not null)
                                {
                                    execution.OnExecutionProgressUpdated += _viewModel.UpdateExecutionProgress;
                                }
                                Conversion.LastReconciliationsUpdated = DateTime.Now;
                            }
                        }
                    }
                }
                UpdateEstimatedManagerCounts();
                ClearStatusMessage(DashboardStatus.UpdatingExecutions);
            }
        }

        /// <summary>
        /// Clears the current status message, if it matches the input message, after a delay.
        /// </summary>
        /// <param name="status">The status message to clear.</param>
        /// <param name="delay">The delay to wait.</param>
        private async void ClearStatusMessage(DashboardStatus status, int delay = 1000)
        {
            await Task.Delay(delay);
            if (_viewModel is not null && _viewModel.CurrentStatus == _statusMessages[status])
            {
                _viewModel.CurrentStatus = _statusMessages[DashboardStatus.Idle];
            }
        }

        /// <summary>
        /// Sets the status message in the control bar.
        /// </summary>
        /// <param name="status">The status message to set.</param>
        private void SetStatusMessage(DashboardStatus status)
        {
            if (_viewModel is not null)
            {
                _viewModel.CurrentStatus = _statusMessages[status];
            }
        }

        /// <summary>
        /// Ensures that there is an active profile with database credentials, and then starts the monitoring process.
        /// If the process is already running, it is ended.
        /// </summary>
        public void ChangeMonitoringState()
        {
            if (UserSettings.ActiveProfile is null)
            {
                MessageBox.Show("Please select a profile.");
            }
            else if (!_viewModel.IsRunning)
            {
                if (!UserSettings.ActiveProfile.HasReceivedCredentials)
                {
                    GetCredentials();
                }
                if (UserSettings.ActiveProfile.HasReceivedCredentials)
                {
                    _viewModel.LoadingVisibility = Visibility.Visible;

                    DataHandler.SetupDatabase(UserSettings.ActiveProfile);
                    if (!UserSettings.ActiveProfile.HasEventListeners())
                    {
                        UserSettings.ActiveProfile.ProfileChanged += Reset;
                    }
                    if (!UserSettings.HasEventListeners())
                    {
                        UserSettings.SettingsChanged += ResetTimers;
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
            bool IsLogReady = false,
                 IsManagersReady = false,
                 IsReconciliationsReady = false,
                 IsHealthReportReady = false;

            _viewModel.IsRunning = true;
            UpdateExecutions();
            SetupTimers();

            while (_viewModel.IsRunning)
            {
                if (ShouldUpdateLog)
                {
                    UpdateLog();
                    ShouldUpdateLog = false;
                    IsLogReady = true;
                }
                if (ShouldUpdateManagers)
                {
                    UpdateManagers();
                    ShouldUpdateManagers = false;
                    IsManagersReady = true;
                }
                if (ShouldUpdateReconciliations)
                {
                    UpdateReconciliations();
                    ShouldUpdateReconciliations = false;
                    IsReconciliationsReady = true;
                }
                if (ShouldUpdateHealthReport)
                {
                    UpdateHealthReport();
                    ShouldUpdateHealthReport = false;
                    IsHealthReportReady = true;
                }
                if (ShouldUpdateExecutions)
                {
                    UpdateExecutions();
                    ShouldUpdateExecutions = false;
                }
                if (IsLogReady && IsManagersReady && IsReconciliationsReady && IsHealthReportReady)
                {
                    _viewModel.LoadingVisibility = Visibility.Collapsed;
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
            while (_viewModel.IsRunning)
            {
                if (Conversion?.ActiveExecution is null)
                {
                    continue;
                }
                foreach (ReconciliationReportViewModel vm in ReconciliationReportViewModels.ToList())
                {
                    if (vm.LastUpdated < Conversion.LastReconciliationsUpdated)
                    {
                        vm.LastUpdated = DateTime.Now;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            vm.UpdateData(Conversion.Executions);
                        });
                    }
                }
                foreach (LogViewModel vm in LogViewModels.ToList())
                {
                    if (vm.LastUpdated <= Conversion.LastLogUpdated)
                    {
                        vm.LastUpdated = DateTime.Now;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            vm.UpdateData(Conversion.Executions);
                        });
                    }
                }
                foreach (ManagerViewModel vm in ManagerViewModels.ToList())
                {
                    if (vm.LastUpdated <= Conversion?.LastManagerUpdated)
                    {
                        vm.LastUpdated = DateTime.Now;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            vm.UpdateData(Conversion.Executions);
                        });
                    }
                }
                foreach (HealthReportViewModel vm in HealthReportViewModels.ToList())
                {
                    if (vm.LastUpdated <= Conversion.HealthReport.LastModified)
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
                    ShouldUpdateReconciliations = true;
                    ShouldUpdateManagers = true;

                }, null, 0, UserSettings.AllQueryInterval * 1000));
            }
            else
            {
                _timers.Add(new Timer(x => ShouldUpdateLog = true, null, 0, UserSettings.LoggingQueryInterval * 1000));
                _timers.Add(new Timer(x => ShouldUpdateExecutions = true, null, 0, 10000));
                _timers.Add(new Timer(x => ShouldUpdateHealthReport = true, null, 0, UserSettings.HealthReportQueryInterval * 1000));
                _timers.Add(new Timer(x => ShouldUpdateReconciliations = true, null, 0, UserSettings.ReconciliationQueryInterval * 1000));
                _timers.Add(new Timer(x => ShouldUpdateManagers = true, null, 0, UserSettings.ManagerQueryInterval * 1000));
            }
        }

        /// <summary>
        /// Disposes all timers in <see cref="_timers"/>, and sets them up with new query intervals.
        /// </summary>
        private void ResetTimers()
        {
            foreach (Timer? timer in _timers)
            {
                timer.Dispose();
            }
            _timers.Clear();
            SetupTimers();
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
            _viewModel.IsRunning = false;
        }

        /// <summary>
        /// Attempts to load user settings, catching potential exceptions if loading fails and displaying an error.
        /// </summary>
        private void LoadUserSettings()
        {
            try
            {
                UserSettings.Load();
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

        /// <summary>
        /// Displays an error to the user.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="ex">The exception that occurred.</param>
        private static void DisplayGeneralError(string message, Exception ex)
        {
            MessageBox.Show($"{message}\n\nDetails\n{ex.Message}");
        }

        /// <summary>
        /// Resets the controller and all view models.
        /// </summary>
        internal void Reset()
        {
            StopMonitoring();
            Conversion = new();
            InitializeViewModels(LogViewModels[0].LogListView);
            _viewModel.CurrentStatus = _statusMessages[DashboardStatus.Idle];
            _viewModel.UpdateView();
            _viewModel.CurrentProgress = 0;
            KillAllChildren();
        }

        /// <summary>
        /// Closes all child windows.
        /// </summary>
        internal void KillAllChildren()
        {
            if (LogViewModels.Count > 1)
            {
                LogViewModels.RemoveRange(1, LogViewModels.Count - 1);
            }
            if (ManagerViewModels.Count > 1)
            {
                ManagerViewModels.RemoveRange(1, LogViewModels.Count - 1);
            }
            if (ReconciliationReportViewModels.Count > 1)
            {
                ReconciliationReportViewModels.RemoveRange(1, LogViewModels.Count - 1);
            }
            if (HealthReportViewModels.Count > 1)
            {
                HealthReportViewModels.RemoveRange(1, LogViewModels.Count - 1);
            }
            foreach (Window item in Application.Current.Windows)
            {
                if (item != Application.Current.MainWindow)
                    item.Close();
            }
        }

        /// <summary>
        /// Expands the manager view for a specific manager in a new or already existing detached manager view.
        /// </summary>
        /// <param name="wrapper">The selected <see cref="ManagerWrapper"/></param>
        public async void ExpandManagerView(ManagerWrapper wrapper)
        {
            ManagerViewModel vm;
            if (wrapper == null) return;
            if (_viewModel.ManagerViewModel.SelectedExecution == null) return;
            int selectedExecutionId = _viewModel.ManagerViewModel.SelectedExecution.Id;
            if (_viewModel.Controller.ManagerViewModels.Skip(1).Any(vm => vm.SelectedExecution?.Id == selectedExecutionId))
            {
                vm = _viewModel.Controller.ManagerViewModels.Skip(1).First(vm => vm.SelectedExecution?.Id == selectedExecutionId);
            }
            else
            {
                vm = CreateManagerViewModel();
                ManagerListDetached managerWindow = new ManagerListDetached(vm);
                vm.Window = managerWindow;
                vm.DataGridManagers = managerWindow.DatagridManagers;
                vm.Window.Show();
                vm.Window.Closed += delegate
                {
                    // Ensures that the ViewModel is only removed from the controller after its data has been modified, preventing an InvalidOperationException.
                    _ = Task.Run(() =>
                    {
                        while (ShouldUpdateManagers) { }
                        ManagerViewModels.Remove(vm);
                    });
                };
                await Task.Delay(5);
                vm.SelectedExecution = vm.Executions[selectedExecutionId - 1];
            }
            vm.Window.Activate();
            ManagerWrapper? foreignManager = vm.Managers.FirstOrDefault(m => m.Manager.ContextId == wrapper.Manager.ContextId);
            if (foreignManager != null)
            {
                vm.Managers.Remove(foreignManager);
                if (!vm.DetailedManagers.Contains(foreignManager))
                {
                    vm.DetailedManagers.Add(foreignManager);
                    vm.UpdateHiddenManagers();
                    foreignManager.IsDetailedInfoShown = true;
                    vm.ManagerChartViewModel.AddChartLinesHelper(foreignManager);
                }
            }
        }
    }
}