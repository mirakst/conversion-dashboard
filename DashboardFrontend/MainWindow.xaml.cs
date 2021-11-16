using DashboardFrontend;
using DashboardFrontend.DetachedWindows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace DashboardInterface
{
    public partial class MainWindow : Window
    {
        private PeriodicTimer DataGenerationTimer;
        private HealthReportMonitoring Monitoring;

        /* List' should be removed once RAM and CPU data is available */
        private List<DateTime> RamDateTime = new();
        private List<long> RamReadings = new();

        private List<DateTime> CpuDataTime = new();
        private List<long> CpuReadings = new();
        /* To here */

        private bool IsStarted = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void ButtonStartStopClick(object sender, RoutedEventArgs e)
        {
            //ConnectDBDialog dialogPopup = new();
            //dialogPopup.Owner = Application.Current.MainWindow;
            //dialogPopup.ShowDialog();

            /* Should be moved to OnConnected */
            if (!IsStarted)
            {
                DataGenerationTimer  = new(TimeSpan.FromSeconds(1));
                Monitoring = new();

                gridHealthReportChartGridChartGrid.Children.Clear();
                Monitoring.Add(RamDateTime, RamReadings, gridHealthReportChartGridChartGrid, "ramUsage", "RAM Usage", Color.FromRgb(0, 255, 0), 2);
                Monitoring.Add(CpuDataTime, CpuReadings, gridHealthReportChartGridChartGrid, "cpuLoad", "CPU Load", Color.FromRgb(0, 0, 255), 2);
                Monitoring.GenerateData(DataGenerationTimer, iddChartHealthReportGraph);
                
                IsStarted = true;
            }
            else
            {
                DataGenerationTimer.Dispose();
                IsStarted = false;
            }
        }

        //Expand window events
        public void buttonSettingsClick(object sender, RoutedEventArgs e)
        {
            //SettingsWindow settingsWindow = new();
            //settingsWindow.Closing += OnSettingsWindowClosing;
            //settingsWindow.ShowDialog();
        }

        public void ExpandManagerButtonClick(object sender, RoutedEventArgs e)
        {
            //ManagerWindow expandManager = new();
            //expandManager.Closing += OnManagerWindowClosing;
            //buttonExpandManager.IsEnabled = false;
            //expandManager.Show();
        }

        public void ExpandLogButtonClick(object sender, RoutedEventArgs e)
        {
            LogDetached expandLog = new();
            expandLog.Closing += OnLogWindowClosing;
            buttonLogExpand.IsEnabled = false;
            expandLog.Show();
        }

        public void ExpandValidationReportButtonClick(object sender, RoutedEventArgs e)
        {
            ValidationReportDetached expandVR = new();
            expandVR.Closing += OnValidationWindowClosing;
            buttonValidationReportExpand.IsEnabled = false;
            expandVR.Show();
        }

        public void ExpandHealthReportButtonClick(object sender, RoutedEventArgs e)
        {
            HealthReportDetached expandHR = new();

            if (IsStarted)
            {
                DataGenerationTimer.Dispose();
            }

            buttonHealthReportExpand.IsEnabled = false;
            expandHR.Closing += OnHealthWindowClosing;
            
            expandHR.Show();

            DataGenerationTimer  = new(TimeSpan.FromSeconds(1));
            Monitoring = new();

            gridHealthReportChartGridChartGrid.Children.Clear();
            Monitoring.Add(RamDateTime, RamReadings, expandHR.gridHealthReportChartGrid, "ramUsage", "RAM Usage", Color.FromRgb(0, 255, 0), 2);
            Monitoring.Add(CpuDataTime, CpuReadings, expandHR.gridHealthReportChartGrid, "cpuLoad", "CPU Load", Color.FromRgb(0, 0, 255), 2);
            Monitoring.GenerateData(DataGenerationTimer, expandHR.iddChartHealthReport);
        }

        //OnWindowClosing events
        private void OnSettingsWindowClosing(object? sender, CancelEventArgs e)
        {
            buttonSettings.IsEnabled = true;
        }

        private void OnManagerWindowClosing(object sender, CancelEventArgs e)
        {
            buttonExpandManager.IsEnabled = true;
        }

        private void OnLogWindowClosing(object sender, CancelEventArgs e)
        {
            buttonLogExpand.IsEnabled = true;
        }

        private void OnValidationWindowClosing(object sender, CancelEventArgs e)
        {
            buttonValidationReportExpand.IsEnabled = true;
        }

        private void OnHealthWindowClosing(object sender, CancelEventArgs e)
        {
            DataGenerationTimer.Dispose();

            DataGenerationTimer  = new(TimeSpan.FromSeconds(1));
            Monitoring = new();

            gridHealthReportChartGridChartGrid.Children.Clear();
            Monitoring.Add(RamDateTime, RamReadings, gridHealthReportChartGridChartGrid, "ramUsage", "RAM Usage", Color.FromRgb(0, 255, 0), 2);
            Monitoring.Add(CpuDataTime, CpuReadings, gridHealthReportChartGridChartGrid, "cpuLoad", "CPU Load", Color.FromRgb(0, 0, 255), 2);
            Monitoring.GenerateData(DataGenerationTimer, iddChartHealthReportGraph);

            buttonHealthReportExpand.IsEnabled = true;
        }
    }
}
