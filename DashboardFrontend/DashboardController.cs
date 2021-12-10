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
        private readonly SemaphoreSlim _logSemaphore;


        public DashboardController(IUserSettings userSettings)
        {
            _dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            _timers = new();
            _logSemaphore = new(1, 1);

            DataHandler = new DataHandler();
            UserSettings = userSettings; 
            LoadUserSettings();
        }

        public DashboardController(IUserSettings userSettings, IDataHandler dataHandler) : this(userSettings)
        {
            DataHandler = dataHandler;
        }

        public event LogsUpdated OnLogsUpdated;
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

                        //Conversion!.AddExecution(new(1, DateTime.Now));
                        //Conversion.ActiveExecution.AddManager(new()
                        //{
                        //    Name = "dk.aes.ans.konvertering.managers.conversionUser.AnsConversionUserManager",
                        //    ContextId = 0,
                        //    RowsRead = 10,
                        //    RowsWritten = 20,
                        //    StartTime = new DateTime(1999, 02, 23),
                        //    Status = ManagerStatus.Ready
                        //});

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
                //Timer executionTimer = new(x => { }, null, 0, 1000);
                _timers.Add(new(x => { _ = TryUpdateLog(); }, null, 0, 200));
                //Timer managerTimer = new(x => { }, null, 200, UserSettings.ManagerQueryInterval * 1000);
                //Timer validationsTimer = new(x => { }, null, 200, UserSettings.ValidationQueryInterval * 1000);
                //Timer healthReportTimer = new(x => { }, null, 200, UserSettings.HealthReportQueryInterval * 1000);
                //_timers.AddRange(new List<Timer> { executionTimer, healthReportTimer, logTimer, validationsTimer, managerTimer });
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
        /// Queries any new Log data through the <see cref="DataHandler"/>, parses it (which involves creating and/or updating executions and managers), and adds it to the Log of the associated execution.
        /// </summary>
        /// <remarks>Any changes that will affect the UI are done one the UI thread (as dictated by WPF).</remarks>
        /// <exception cref="ArgumentNullException"></exception>
        //public async Task TryUpdateLog()
        //{
        //    // This function uses a 'Semaphore', which works kind of like 'locking' does.
        //    // The particular semaphore is limited to only one thread (during its initialization), which means that only one thread can access it at a given time.
        //    // Basically, we wait for the previous '_logSemaphore-thread' to finish its work before starting this one, and consequently, the updates will not clash.
        //    await _logSemaphore.WaitAsync();
        //    try
        //    {
        //        if (Conversion is null)
        //        {
        //            throw new ArgumentNullException(nameof(Conversion), "Conversion must not be null when monitoring");
        //        }

        //        var messages = await DataHandler.GetLogMessagesAsync(Conversion.LastLogQuery);
        //        var (managers, executions) = await DataHandler.GetParsedLogDataAsync(messages);

        //        await _dispatcher.InvokeAsync(() =>
        //        {
        //            // Update executions
        //            lock (Conversion.Executions)
        //            {
        //                foreach (var newExecution in executions)
        //                {
        //                    // If the execution already exists, update its status (since it might be finished)
        //                    if (Conversion.Executions.FirstOrDefault(e => e.Id == newExecution.Id) is Execution e)
        //                    {
        //                        e.Status = newExecution.Status;
        //                    }
        //                    // Otherwise, add it
        //                    else
        //                    {
        //                        Conversion.AddExecution(newExecution);
        //                    }
        //                }
        //            }

        //            // Update managers
        //            lock (Conversion.AllManagers)
        //            {
        //                foreach (var newManager in managers)
        //                {
        //                    // If the manager already exists, update its context and execution ID's
        //                    if (Conversion.AllManagers.Find(m => m.Name == newManager.Name && m.ContextId == 0) is Manager existingManager)
        //                    {
        //                        existingManager.ContextId = newManager.ContextId;
        //                        existingManager.ExecutionId = newManager.ExecutionId;
        //                    }
        //                    // Otherwise, add it
        //                    else
        //                    {
        //                        Conversion.AllManagers.Add(newManager);
        //                    }
        //                }
        //            }

        //            // Add managers and log messages to executions
        //            lock (Conversion.Executions)
        //            {
        //                foreach (var execution in Conversion.Executions)
        //                {
        //                    //var messages = 
        //                }
        //            }
        //        });
        //        Conversion.LastLogQuery = DateTime.Now;
        //    }
        //    finally
        //    {
        //        // Releasing the semaphore means that the thread has finished its work.
        //        // This is done in a 'try-finally' clause since it is important to ensure that the semaphore is always released, even in case of exceptions, etc.
        //        // If it is not released, it will always be 'occupied', and future updates will endlessly wait for the semaphore to be released before doing anything.
        //        _logSemaphore.Release();
        //    }
        //}

        public void TryUpdateLog()
        {
            if (Conversion is null)
            {
                throw new ArgumentNullException(nameof(Conversion), "Conversion must not be null when monitoring");
            }

            var messages = DataHandler.GetLogMessagesAsync(Conversion.LastLogQuery);
            var (managers, executions) = DataHandler.GetParsedLogDataAsync(messages);

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
                        }
                    }
                }

                // Add managers and log messages to executions
                lock (Conversion.Executions)
                {
                    foreach (var execution in Conversion.Executions)
                    {
                        //var messages = 
                    }
                }
            });
            Conversion.LastLogQuery = DateTime.Now;
        }

        private void TryUpdateExecutions()
        {

        }
    }
}
