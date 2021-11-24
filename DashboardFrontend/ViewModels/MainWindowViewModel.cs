using DashboardFrontend.DetachedWindows;
using DashboardFrontend.Settings;
using Microsoft.Data.SqlClient;
using Model;
using DashboardBackend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DashboardBackend.Database;

namespace DashboardFrontend.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel(UserSettings userSettings, Log log, ValidationReport validationReport, DataGrid validationsDataGrid)
        {
            _uiContext = SynchronizationContext.Current;

            _log = log;
            _validationReport = validationReport;
            LogViewModel = new(log);
            ValidationReportViewModel = new(validationReport, validationsDataGrid);
            UserSettings = userSettings;
        }

        private List<Timer> _timers = new();
        private SynchronizationContext? _uiContext;
        private ValidationReport _validationReport = new();
        private Log _log = new();
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
        public bool HasReceivedCredentials { get; set; }
        public LogViewModel LogViewModel { get; set; }
        public ValidationReportViewModel ValidationReportViewModel { get; set; }
        public UserSettings UserSettings { get; }

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
            else if (!IsRunning)
            {
                if (!UserSettings.ActiveProfile.HasReceivedCredentials)
                {
                    GetCredentials();
                }
                if (UserSettings.ActiveProfile.HasReceivedCredentials)
                {
                    DataUtilities.DatabaseHandler = new SqlDatabase(UserSettings.ActiveProfile.ConnectionString);
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
                _timers.Add(new(x => 
                {
                    //Task.Run(() => QueryHealthReport());
                    //Task.Run(() => QueryManagers());
                    Task.Run(() => QueryLogs());
                    //Task.Run(() => QueryValidations());

                }, null, 500, UserSettings.AllQueryInterval * 1000));
            }
            else
            {
                _timers.Add(new(x => QueryHealthReport(), null, 500, UserSettings.HealthReportQueryInterval * 1000));
                _timers.Add(new(x => QueryLogs(), null, 500, UserSettings.LoggingQueryInterval * 1000));
                _timers.Add(new(x => QueryManagers(), null, 500, UserSettings.ManagerQueryInterval * 1000));
                _timers.Add(new(x => QueryValidations(), null, 500, UserSettings.ValidationQueryInterval * 1000));
            }
            IsRunning = true;
        }

        private void StopMonitoring()
        {
            foreach (Timer timer in _timers)
            {
                timer.Dispose();
            }
            _timers.Clear();
            IsRunning = false;
        }

        private DateTime _logLastModified = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
        private void QueryLogs()
        {
            var result = DataUtilities.GetLogMessages(_logLastModified);
#if DEBUG
            Trace.WriteLine($"found {result.Count} log messages");
#endif
            if (result.Count > 0)
            {
                _log.Messages.AddRange(result);
                _uiContext?.Send(x => LogViewModel.UpdateData(), null);
            }
            _logLastModified = DateTime.Now;
            Trace.WriteLine("done");
        }

        private void QueryValidations()
        {
            var result = DataUtilities.GetAfstemninger(_validationReport.LastModified);
#if DEBUG
            Trace.WriteLine("Found " + result.Count + " validation tests");
#endif
            if (result.Count > 0)
            {
                _validationReport.ValidationTests = result;
                _uiContext?.Send(x => ValidationReportViewModel.UpdateData(), null);
            }
        }

        private void QueryHealthReport()
        {
            //throw new NotImplementedException();
        }

        private void QueryManagers()
        {
            //throw new NotImplementedException();
        }
    }
}
