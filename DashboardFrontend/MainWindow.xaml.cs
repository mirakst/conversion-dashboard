using DashboardFrontend.DetachedWindows;
using DashboardFrontend.Settings;
using System;
using System.ComponentModel;
using System.Windows;

namespace DashboardInterface
{
    public partial class MainWindow : Window
    {
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

        public void ButtonStartStopClick(object sender, RoutedEventArgs e)
        {
            //DialogWindow dialogWindow = new();
            //dialogWindow.Owner = Application.Current.MainWindow;
            //dialogWindow.ShowDialog();
        }

        //Expand window events
        public void buttonSettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new(UserSettings);
            //settingsWindow.Closing += OnSettingsWindowClosing;
            //settingsWindow.IsEnabled = false;
            //settingsWindow.Owner = Application.Current.MainWindow;
            settingsWindow.ShowDialog();
        }

        public void ExpandManagerButtonClick(object sender, RoutedEventArgs e)
        {
            //ManagerWindow expandManager = new();
            //expandManager.Closing += OnManagerWindowClosing;
            //buttonExpandManager.IsEnabled = false;
            //expandManager.Owner = Application.Current.MainWindow;
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
            expandVR.Owner = Application.Current.MainWindow;
            expandVR.Show();
        }

        public void ExpandHealthReportButtonClick(object sender, RoutedEventArgs e)
        {
            HealthReportDetached expandHR = new();
            buttonHealthReportExpand.IsEnabled = false;
            expandHR.Closing += OnHealthWindowClosing;
            expandHR.Owner = Application.Current.MainWindow;
            expandHR.Show();
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
            buttonHealthReportExpand.IsEnabled = true;
        }
    }
}
