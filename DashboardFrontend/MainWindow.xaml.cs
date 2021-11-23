using DashboardFrontend;
using DashboardFrontend.ViewModels;
using DashboardFrontend.DetachedWindows;
using DashboardFrontend.Settings;
using DashboardBackend;
using DashboardBackend.Database;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Threading;
using System.Windows.Media;

namespace DashboardFrontend
{
    public partial class MainWindow : Window
    {
        private ChartViewModel? _chartVm;
        private bool _isStarted;

        public MainWindow()
        {
            InitializeComponent();
            DataUtilities.DatabaseHandler = new SqlDatabase();
            ValidationReport.ValidationTests = DataUtilities.GetAfstemninger();

            TryLoadUserSettings();
            
            ViewModel = new(UserSettings, Log, ValidationReport, DataGridValidations);
            DataContext = ViewModel;
        }

        private UserSettings UserSettings { get; } = new();
        public ValidationReport ValidationReport { get; set; } = new();
        public Log Log { get; set; } = new();
        public MainWindowViewModel ViewModel { get; }
        
        private void TryLoadUserSettings()
        {
            try
            {
                UserSettings.LoadFromFile();
            }
            catch (System.IO.FileNotFoundException ex)
            {
                // Configuration file was not found, possibly first time setup
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
            LogDetached detachLog = new(ViewModel.LogViewModel);
            detachLog.Closing += OnLogWindowClosing;
            ButtonLogDetach.IsEnabled = false;
            detachLog.Show();
        }

        public void DetachValidationReportButtonClick(object sender, RoutedEventArgs e)
        {            
            ValidationReportDetached detachVR = new(ValidationReport);
            detachVR.Closing += OnValidationWindowClosing;
            buttonValidationReportDetach.IsEnabled = false;
            detachVR.Show();
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
            buttonValidationReportDetach.IsEnabled = true;
        }

        private void OnHealthWindowClosing(object? sender, CancelEventArgs e)
        {
            ButtonHealthReportDetach.IsEnabled = true;
            _chartVm?.Dispose();
            _chartVm = new ChartViewModel();
            _chartVm.PerformanceMonitoringStart(IddChartHealthReportGraph, GridHealthReportChartGridChartGrid, TextBoxChartTimeInterval);

            ButtonHealthReportDetach.IsEnabled = true;
        }

        private void validationsDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = DataGridValidations
            };
            DataGridValidations.RaiseEvent(eventArgs);
        }

        private void DetailsDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = DataGridValidations
            };
            DataGridValidations.RaiseEvent(eventArgs);
        }

        private void MenuItem_SrcSql_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            if (menuItem.DataContext is ValidationTest test)
            {
                Clipboard.SetText(test.SrcSql ?? "");
            }
        }

        private void MenuItem_DstSql_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            if (menuItem.DataContext is ValidationTest test)
            {
                Clipboard.SetText(test.DstSql ?? "");
            }
        }
    }
}
