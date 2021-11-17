using DashboardFrontend;
using DashboardFrontend.DetachedWindows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace DashboardInterface
{
    public partial class MainWindow : Window
    {
        private ChartViewModel ChartVM;
        private bool IsStarted = false;

        public MainWindow()
        {
            InitializeComponent();
            iddChartHealthReportGraph.PlotOriginX = DateTime.Now.Ticks;
            iddChartHealthReportGraph.PlotWidth = TimeSpan.FromMinutes(6).Ticks;
            iddChartHealthReportGraph.PlotHeight = 105;
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
                ChartVM = new();
                ChartVM.PerformanceMonitoringStart(iddChartHealthReportGraph, gridHealthReportChartGridChartGrid, textBoxChartTimeInterval);
                IsStarted = true;
            }
            else
            {
                ChartVM.Dispose();
                IsStarted = false;
            }
        }

        //Detach window events
        public void ButtonSettingsClick(object sender, RoutedEventArgs e)
        {
            //SettingsWindow settingsWindow = new();
            //settingsWindow.Closing += OnSettingsWindowClosing;
            //settingsWindow.ShowDialog();
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
            ChartVM = new();

            if (IsStarted)
            {
                ChartVM.Dispose();
            }

            buttonHealthReportDetach.IsEnabled = false;
            expandHR.Closing += OnHealthWindowClosing;
            
            expandHR.Show();
            ChartVM.PerformanceMonitoringStart(expandHR.iddChartHealthReport, expandHR.gridHealthReportChartGrid, expandHR.textBoxChartTimeInterval);
            ChartVM.NetworkMonitoringStart(expandHR.iddChartNetwork, expandHR.gridNetworkChartGrid, expandHR.textBoxChartTimeInterval);
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
            ChartVM.Dispose();

            ChartVM = new();
            ChartVM.PerformanceMonitoringStart(iddChartHealthReportGraph, gridHealthReportChartGridChartGrid, textBoxChartTimeInterval);

            buttonHealthReportDetach.IsEnabled = true;
        }
    }
}
