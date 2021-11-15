using DashboardFrontend.DetachedWindows;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using DashboardFrontend.ViewModels;
using DashboardBackend.Models;
using System;

namespace DashboardFrontend
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new(Log);
            DataContext = ViewModel;
        }

        public Log Log { get; set; } = new();
        public MainWindowViewModel ViewModel { get; }

        private void DraggableGrid(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public void ButtonStartStopClick(object sender, RoutedEventArgs e)
        {
            //DialogWindow dialogWindow = new();
            //dialogWindow.Owner = Application.Current.MainWindow;
            //dialogWindow.ShowDialog();
        }

        //Detach window events
        public void ButtonSettingsClick(object sender, RoutedEventArgs e)
        {
            //SettingsWindow settingsWindow = new();
            //settingsWindow.Closing += OnSettingsWindowClosing;
            //settingsWindow.IsEnabled = false;
            //settingsWindow.Owner = Application.Current.MainWindow;
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
            LogDetached detachLog = new(ViewModel.LogViewModel);
            // detachLog.Closing += OnLogWindowClosing
            // buttonLogDetach.IsEnabled = false;
            detachLog.Show();
        }

        public void DetachValidationReportButtonClick(object sender, RoutedEventArgs e)
        {
            //ValidationReportDetached detachVR = new();
            //detachVR.Closing += OnValidationWindowClosing;
            //buttonValidationReportDetach.IsEnabled = false;
            //detachVR.Show();
        }

        public void DetachHealthReportButtonClick(object sender, RoutedEventArgs e)
        {
            //HealthReportDetached detachHR = new();
            //buttonHealthReportDetach.IsEnabled = false;
            //detachHR.Closing += OnHealthWindowClosing;
            //detachHR.Show();
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
        public void OnToggleButtonClick(object sender, RoutedEventArgs e)
        {
            
        }
        private void OnValidationWindowClosing(object sender, CancelEventArgs e)
        {
            buttonValidationReportDetach.IsEnabled = true;
        }

        private void OnHealthWindowClosing(object sender, CancelEventArgs e)
        {
            buttonHealthReportDetach.IsEnabled = true;
        }
    }
}
