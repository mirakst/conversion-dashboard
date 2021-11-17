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
        private ChartViewModel? _chartVm;
        private bool _isStarted;

        public MainWindow()
        {
            InitializeComponent();
            IddChartHealthReportGraph.PlotOriginX = DateTime.Now.Ticks;
            IddChartHealthReportGraph.PlotWidth = TimeSpan.FromMinutes(6).Ticks;
            IddChartHealthReportGraph.PlotHeight = 105;
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
            if (!_isStarted)
            {
                _chartVm = new();
                _chartVm.PerformanceMonitoringStart(IddChartHealthReportGraph, GridHealthReportChartGridChartGrid, TextBoxChartTimeInterval);
                _isStarted = true;
            }
            else
            {
                _chartVm?.Dispose();
                _isStarted = false;
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
            ButtonLogDetach.IsEnabled = false;
            detachLog.Show();
        }

        public void DetachValidationReportButtonClick(object sender, RoutedEventArgs e)
        {
            ValidationReportDetached detachVr = new();
            detachVr.Closing += OnValidationWindowClosing;
            ButtonValidationReportDetach.IsEnabled = false;
            detachVr.Show();
        }

        public void DetachHealthReportButtonClick(object sender, RoutedEventArgs e)
        {
            HealthReportDetached expandHr = new();
            _chartVm = new ChartViewModel();

            if (_isStarted)
            {
                _chartVm.Dispose();
            }

            ButtonHealthReportDetach.IsEnabled = false;
            expandHr.Closing += OnHealthWindowClosing;
            
            expandHr.Show();
            _chartVm.PerformanceMonitoringStart(expandHr.IddChartHealthReport, expandHr.GridHealthReportChartGrid, expandHr.TextBoxChartTimeInterval);
            _chartVm.NetworkMonitoringStart(expandHr.IddChartNetwork, expandHr.GridNetworkChartGrid, expandHr.TextBoxChartTimeInterval);
        }

        //OnWindowClosing events
        private void OnSettingsWindowClosing(object? sender, CancelEventArgs e)
        {
            ButtonSettings.IsEnabled = true;
        }

        private void OnManagerWindowClosing(object sender, CancelEventArgs e)
        {
            ButtonDetachManager.IsEnabled = true;
        }

        private void OnLogWindowClosing(object? sender, CancelEventArgs e)
        {
            ButtonLogDetach.IsEnabled = true;
        }

        private void OnValidationWindowClosing(object? sender, CancelEventArgs e)
        {
            ButtonValidationReportDetach.IsEnabled = true;
        }

        private void OnHealthWindowClosing(object? sender, CancelEventArgs e)
        {
            _chartVm?.Dispose();
            _chartVm = new ChartViewModel();
            _chartVm.PerformanceMonitoringStart(IddChartHealthReportGraph, GridHealthReportChartGridChartGrid, TextBoxChartTimeInterval);

            ButtonHealthReportDetach.IsEnabled = true;
        }
    }
}
