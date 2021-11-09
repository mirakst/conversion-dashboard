using DashboardFrontend.DetachedWindows;
using System.ComponentModel;
using System.Windows;

namespace DashboardInterface
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void StartStopButtonClick(object sender, RoutedEventArgs e)
        {
            //DialogWindow dialogWindow = new();
            //dialogWindow.Owner = Application.Current.MainWindow;
            //dialogWindow.ShowDialog();
        }

        //Expand window events
        public void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            //SettingsWindow settingsWindow = new();
            //settingsWindow.Closing += OnSettingsWindowClosing;
            //settingsWindow.IsEnabled = false;
            //settingsWindow.Owner = Application.Current.MainWindow;
            //settingsWindow.ShowDialog();
        }

        public void ExpandManagerButtonClick(object sender, RoutedEventArgs e)
        {
            //ManagerWindow expandManager = new();
            //expandManager.Closing += OnManagerWindowClosing;
            //managerExpandButton.IsEnabled = false;
            //expandManager.Owner = Application.Current.MainWindow;
            //expandManager.Show();
        }

        public void ExpandLogButtonClick(object sender, RoutedEventArgs e)
        {
            LogDetached expandLog = new();
            expandLog.Closing += OnLogWindowClosing;
            logExpandButton.IsEnabled = false;
            expandLog.Show();
        }

        public void ExpandValidationReportButtonClick(object sender, RoutedEventArgs e)
        {
            //ValidationReportWindow expandVR = new();
            //expandVR.Closing += OnValidationWindowClosing;
            //validationReportExpandButton.IsEnabled = false;
            //expandVR.Owner = Application.Current.MainWindow;
            //expandVR.Show();
        }

        public void ExpandHealthReportButtonClick(object sender, RoutedEventArgs e)
        {
            HealthReportDetached expandHR = new();
            healthReportExpandButton.IsEnabled = false;
            expandHR.Closing += OnHealthWindowClosing;
            expandHR.Owner = Application.Current.MainWindow;
            expandHR.Show();
        }

        //OnWindowClosing events
        private void OnSettingsWindowClosing(object? sender, CancelEventArgs e)
        {
            settingsButton.IsEnabled = true;
        }

        private void OnManagerWindowClosing(object sender, CancelEventArgs e)
        {
            managerExpandButton.IsEnabled = true;
        }

        private void OnLogWindowClosing(object sender, CancelEventArgs e)
        {
            logExpandButton.IsEnabled = true;
        }

        private void OnValidationWindowClosing(object sender, CancelEventArgs e)
        {
            validationReportExpandButton.IsEnabled = true;
        }

        private void OnHealthWindowClosing(object sender, CancelEventArgs e)
        {
            healthReportExpandButton.IsEnabled = true;
        }

        private void LogEntryList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
