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
        private SynchronizationContext? _uiContext;
        public MainWindowViewModel(UserSettings userSettings, Log log, ValidationReport validationReport, DataGrid validationsDataGrid)
        {
            _uiContext = SynchronizationContext.Current;

            _log = log;
            _validationReport = validationReport;
            LogViewModel = new(log);
            ValidationReportViewModel = new(validationReport, validationsDataGrid);
            UserSettings = userSettings;
        }

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
        private List<Timer> _timers = new();

        public void OnStartPressed()
        {
            if (UserSettings.ActiveProfile is null)
            {
                MessageBox.Show("Please select a profile.");
            }
            else if(!IsRunning && !UserSettings.ActiveProfile.HasReceivedCredentials)
            {
                ConnectDBDialog dialogPopup = new(UserSettings);
                dialogPopup.ShowDialog();
                if (UserSettings.ActiveProfile.HasReceivedCredentials)
                {
                    DataUtilities.DatabaseHandler = new SqlDatabase(UserSettings.ActiveProfile.ConnectionString);
                    StartQueryTimers();
                    IsRunning = true;
                }
            }
            else if (!IsRunning)
            {
                StartQueryTimers();
                IsRunning = true;
                MessageBox.Show("Started");
            }
            else
            {
                IsRunning = false;
                StopQueryTimers();
                MessageBox.Show("Stopped");
            }
        }

        private void StartQueryTimers()
        {
            if (UserSettings.SynchronizeAllQueries)
            {
                _timers.Add(new((a) => Trace.WriteLine("querying all"), null, 1000, UserSettings.AllQueryInterval * 1000));
            }
            else
            {
                //_timers.Add(new((a) => Trace.WriteLine("querying health report"), null, 1000, UserSettings.HealthReportQueryInterval * 1000));
                _timers.Add(new((a) => QueryLogging(), null, 1000, UserSettings.LoggingQueryInterval * 1000));
                //_timers.Add(new((a) => Trace.WriteLine("querying managers"), null, 1000, UserSettings.ManagerQueryInterval * 1000));
                //_timers.Add(new((a) => Trace.WriteLine("querying validations"), null, 1000, UserSettings.ValidationQueryInterval * 1000));
            }
        }

        private void StopQueryTimers()
        {
            foreach (var timer in _timers)
            {
                timer.Dispose();
            }
            _timers.Clear();
        }

        private DateTime lastModified = DateTime.Now.AddMonths(-24);

        private void QueryLogging()
        {
            Trace.WriteLine("log jeje");
            //_log.Messages.AddRange(DataUtilities.GetLogMessages(lastModified));
            //LogViewModel.UpdateData();
            //lastModified = DateTime.Now;
            var q = DataUtilities.GetAfstemninger(_validationReport.LastModified);
            Trace.WriteLine("found " + q.Count + " validation tests");
            if (q.Count > 0)
            {
                _validationReport.ValidationTests = q;
                //ValidationReportViewModel.UpdateData();
                _uiContext?.Send(x => ValidationReportViewModel.UpdateData(q), null);
            }
        }
    }
}
