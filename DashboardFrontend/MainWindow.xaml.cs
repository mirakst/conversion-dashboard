using DashboardFrontend;
using DashboardFrontend.DetachedWindows;
using DashboardFrontend.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Threading;
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
            try
            {
                UserSettings.LoadFromFile();
            }
            catch (System.IO.FileNotFoundException ex)
            {
                DisplayGeneralError("Could not find configuration file", ex);
            }
            catch (System.Text.Json.JsonException ex)
            {
                DisplayGeneralError("Failed to parse contents of UserSettings.json", ex);
            }
            catch (System.IO.IOException ex)
            {
                DisplayGeneralError("An unexpected problem occured while loading user settings", ex);
            }
        }

        private UserSettings UserSettings { get; } = new();

        private void DisplayGeneralError(string message, Exception ex)
        {
            MessageBox.Show($"{message}\n\nDetails\n{ex.Message}");
        }

        private void DraggableGrid(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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
                Monitoring.Add(RamDateTime, RamReadings, gridHealthReportChartGridChartGrid, "ramUsage", "RAM Usage", Color.FromRgb(133, 222, 118), 2);
                Monitoring.Add(CpuDataTime, CpuReadings, gridHealthReportChartGridChartGrid, "cpuLoad", "CPU Load", Color.FromRgb(245, 88, 47), 2);
                Monitoring.GenerateData(DataGenerationTimer, iddChartHealthReportGraph, textBoxChartTimeInterval);
                
                IsStarted = true;
            }
            else
            {
                DataGenerationTimer.Dispose();
                IsStarted = false;
            }
        }

        //Detach window events
        public void ButtonSettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new(UserSettings);
            //settingsWindow.Closing += OnSettingsWindowClosing;
            //settingsWindow.IsEnabled = false;
            //settingsWindow.Owner = Application.Current.MainWindow;
            settingsWindow.ShowDialog();
        }

        public void DetachManagerButtonClick(object sender, RoutedEventArgs e)
        {
            //ManagerWindow detachManager = new();
            //detachManager.Closing += OnManagerWindowClosing;
            //buttonDetachManager.IsEnabled = false;
            //detachManager.Show();
        }

        public void DetachLogButtonClick(object sender, RoutedEventArgs e)
        {
            LogDetached detachLog = new();
            detachLog.Closing += OnLogWindowClosing;
            buttonLogDetach.IsEnabled = false;
            detachLog.Show();
        }

        public void DetachValidationReportButtonClick(object sender, RoutedEventArgs e)
        {
            ValidationReportDetached detachVR = new();
            detachVR.Closing += OnValidationWindowClosing;
            buttonValidationReportDetach.IsEnabled = false;
            detachVR.Show();
        }

        public void DetachHealthReportButtonClick(object sender, RoutedEventArgs e)
        {
            HealthReportDetached expandHR = new();

            if (IsStarted)
            {
                DataGenerationTimer.Dispose();
            }

            buttonHealthReportDetach.IsEnabled = false;
            expandHR.Closing += OnHealthWindowClosing;
            
            expandHR.Show();

            DataGenerationTimer  = new(TimeSpan.FromSeconds(1));
            Monitoring = new();

            //gridHealthReportChartGridChartGrid.Children.Clear();
            Monitoring.Add(RamDateTime, RamReadings, expandHR.gridHealthReportChartGrid, "ramUsage", "RAM Usage", Color.FromRgb(133, 222, 118), 2);
            Monitoring.Add(CpuDataTime, CpuReadings, expandHR.gridHealthReportChartGrid, "cpuLoad", "CPU Load", Color.FromRgb(245, 88, 47), 2);
            Monitoring.GenerateData(DataGenerationTimer, expandHR.iddChartHealthReport, expandHR.textBoxChartTimeInterval);
        }

        //OnWindowClosing events
        private void OnSettingsWindowClosing(object? sender, CancelEventArgs e)
        {
            buttonSettings.IsEnabled = true;
        }

        private void OnManagerWindowClosing(object sender, CancelEventArgs e)
        {
            buttonDetachManager.IsEnabled = true;
        }

        private void OnLogWindowClosing(object sender, CancelEventArgs e)
        {
            buttonLogDetach.IsEnabled = true;
        }

        private void OnValidationWindowClosing(object sender, CancelEventArgs e)
        {
            buttonValidationReportDetach.IsEnabled = true;
        }

        private void OnHealthWindowClosing(object sender, CancelEventArgs e)
        {
            DataGenerationTimer.Dispose();

            DataGenerationTimer  = new(TimeSpan.FromSeconds(1));
            Monitoring = new();

            //gridHealthReportChartGridChartGrid.Children.Clear();
            Monitoring.Add(RamDateTime, RamReadings, gridHealthReportChartGridChartGrid, "ramUsage", "RAM Usage", Color.FromRgb(133, 222, 118), 2);
            Monitoring.Add(CpuDataTime, CpuReadings, gridHealthReportChartGridChartGrid, "cpuLoad", "CPU Load", Color.FromRgb(245, 88, 47), 2);
            Monitoring.GenerateData(DataGenerationTimer, iddChartHealthReportGraph, textBoxChartTimeInterval);

            buttonHealthReportDetach.IsEnabled = true;
        }
    }
}
