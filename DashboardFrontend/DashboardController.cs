using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
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
        private readonly System.Windows.Threading.Dispatcher _dispatcher;

        public DashboardController(IUserSettings userSettings)
        {
            _dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            _timers = new();

            DataHandler = new DataHandler();
            UserSettings = userSettings; 
            LoadUserSettings();
        }

        public DashboardController(IUserSettings userSettings, IDataHandler dataHandler) : this(userSettings)
        {
            DataHandler = dataHandler;
        }

        public event ConversionCreated OnConversionCreated;

        public IDataHandler DataHandler { get; set; }
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
                        SetupNewConversion();
                        DataHandler.SetupDatabase(UserSettings.ActiveProfile);

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
            //DataHandler.BuildHealthReport(Conversion.HealthReport);

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
                //Timer executionTimer = new(x => { /*TryUpdateExecutions();*/ }, null, 0, 10000);
                //Timer logTimer = new(x => { TryUpdateLog(); }, null, 200, 1000);
                //Timer managerTimer = new(x => { /*TryUpdateManagers();*/ }, null, 200, UserSettings.ManagerQueryInterval * 1000);
                //Timer validationsTimer = new(x => { }, null, 200, UserSettings.ValidationQueryInterval * 1000);
                //Timer healthReportTimer = new(x => { }, null, 200, UserSettings.HealthReportQueryInterval * 1000);
                //_timers.AddRange(new List<Timer> { executionTimer, healthReportTimer, logTimer, validationsTimer, managerTimer });
            }
            TryUpdateExecutions();
            Trace.WriteLine("Got executions");
            TryUpdateLog();
            Trace.WriteLine("Got logs");
            TryUpdateManagers();
            Trace.WriteLine("Got managers");
            TryUpdateValidations();

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
        /// Queries any new Log data through the <see cref="DataHandler"/>, parses it (which involves creating and/or updating executions and managers), and adds it to the Log of the associated execution.
        /// </summary>
        /// <remarks>Any changes that will affect the UI are done one the UI thread (as dictated by WPF).</remarks>
        /// <exception cref="ArgumentNullException"></exception>
        public void TryUpdateLog()
        {
            lock (_lockLog)
            {
                if (Conversion is null)
                {
                    throw new ArgumentNullException(nameof(Conversion), "Conversion must not be null when monitoring");
                }

                var messages = DataHandler.GetLogMessages(Conversion.LastLogQuery);
                var (managers, executions) = DataHandler.GetParsedLogData(messages);

                _dispatcher.Invoke(() =>
                {
                    // Update executions
                    lock (Conversion.Executions)
                    {
                        foreach (var newExecution in executions)
                        {
                            // If the execution already exists, update its status (since it might be finished)
                            if (Conversion.Executions.FirstOrDefault(e => e.Id == newExecution.Id) is Execution e)
                            {
                                e.Status = newExecution.Status;
                            }
                            // Otherwise, add it
                            else
                            {
                                Conversion.AddExecution(newExecution);
                            }
                        }
                    }
                    // Update managers
                    lock (Conversion.AllManagers)
                    {
                        foreach (var newManager in managers)
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
                    // Add managers and log messages to executions
                    lock (Conversion.Executions)
                    {
                        foreach (var execution in Conversion.Executions)
                        {
                            execution.Log.Messages.AddRange(messages.Where(m => m.ExecutionId == execution.Id));
                        }
                    }
                });
                Conversion.LastLogQuery = DateTime.Now;
            }   
        }
        private readonly object _lockLog = new();

        public void TryUpdateExecutions()
        {
            lock (_lockExecutions)
            {
                if (Conversion is null)
                {
                    throw new ArgumentNullException(nameof(Conversion), "Conversion must not be null when monitoring");
                }

                var executions = DataHandler.GetExecutions(Conversion.LastExecutionQuery);

                _dispatcher.Invoke(() =>
                {
                    lock (Conversion.Executions)
                    {
                        foreach (var execution in executions)
                        {
                            if (Conversion.Executions.FirstOrDefault(e => e.Id == execution.Id) is Execution existingExecution)
                            {
                                existingExecution.StartTime = execution.StartTime;
                            }
                            else
                            {
                                Conversion.AddExecution(execution);
                            }
                        }
                    }
                });
                Conversion.LastExecutionQuery = DateTime.Now;
            }
        }
        private readonly object _lockExecutions = new();

        public void TryUpdateManagers()
        {
            lock (_lockManagers)
            {
                if (Conversion is null)
                {
                    throw new ArgumentNullException(nameof(Conversion), "Conversion must not be null when monitoring");
                }

                var managers = DataHandler.GetManagers(Conversion.LastManagerQuery);

                _dispatcher.Invoke(() =>
                {
                    lock (Conversion.AllManagers)
                    {
                        foreach (var manager in managers)
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
                    }
                });
                Conversion.LastManagerQuery = DateTime.Now;
            }
        }
        private readonly object _lockManagers = new();

        public void TryUpdateValidations()
        {
            lock (_lockValidations)
            {
                if (Conversion is null)
                {
                    throw new ArgumentNullException(nameof(Conversion), "Conversion must not be null when monitoring");
                }

                var data = DataHandler.GetValidations(Conversion.LastValidationsQuery);
                data.AddRange(_homelessValidations); // Add any validations that have not yet been assigned to a manager
                _homelessValidations.Clear();

                _dispatcher.Invoke(() =>
                {
                    lock (Conversion.AllManagers)
                    {
                        foreach (var test in data)
                        {
                            // Try to find the associated manager (and ensure that it is in the right execution as well)
                            if (Conversion.AllManagers.Find(m => m.Name.Contains(test.ManagerName) && test.Date < m.EndTime) is Manager mgr)
                            {
                                mgr.AddValidation(test);
                            }
                            // Otherwise, the manager has not yet been created - it is passed to a list and we will try again next time
                            else
                            {
                                _homelessValidations.Add(test);
                            }
                        }
                    }
                });
                Conversion.LastValidationsQuery = DateTime.Now;
            }
        }
        private readonly object _lockValidations = new();
        private readonly List<ValidationTest> _homelessValidations = new();
    }
}
