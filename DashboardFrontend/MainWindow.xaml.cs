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
            //DialogWindow dialogWindow = new();
            //dialogWindow.Owner = Application.Current.MainWindow;
            //dialogWindow.ShowDialog();
        }

        //Expand window events
        public void buttonSettingsClick(object sender, RoutedEventArgs e)
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
            //buttonExpandManager.IsEnabled = false;
            //expandManager.Owner = Application.Current.MainWindow;
            //expandManager.Show();
        }

        public void ExpandLogButtonClick(object sender, RoutedEventArgs e)
        {
            //LogWindow expandLog = new();
            //expandLog.Closing += OnLogWindowClosing;
            //buttonLogExpand.IsEnabled = false;
            //expandLog.Owner = Application.Current.MainWindow;
            //expandLog.Show();
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
