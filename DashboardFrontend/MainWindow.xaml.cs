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

        public void ButtonStartStopClick(object sender, RoutedEventArgs e)
        {
            ConnectDBDialog dialogPopup = new();
            dialogPopup.Owner = Application.Current.MainWindow;
            dialogPopup.ShowDialog();
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
            buttonHealthReportExpand.IsEnabled = false;
            expandHR.Closing += OnHealthWindowClosing;
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
