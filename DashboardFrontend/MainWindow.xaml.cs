using DashboardFrontend.DetachedWindows;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace DashboardInterface
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

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
            //detachManager.Owner = Application.Current.MainWindow;
            //detachManager.Show();
        }

        public void DetachLogButtonClick(object sender, RoutedEventArgs e)
        {
            //LogWindow detachLog = new();
            //detachLog.Closing += OnLogWindowClosing;
            //buttonLogDetach.IsEnabled = false;
            //detachLog.Owner = Application.Current.MainWindow;
            //detachLog.Show();
        }

        public void DetachValidationReportButtonClick(object sender, RoutedEventArgs e)
        {
            ValidationReportDetached detachVR = new();
            detachVR.Closing += OnValidationWindowClosing;
            buttonValidationReportDetach.IsEnabled = false;
            detachVR.Owner = Application.Current.MainWindow;
            detachVR.Show();
        }

        public void DetachHealthReportButtonClick(object sender, RoutedEventArgs e)
        {
            HealthReportDetached detachHR = new();
            buttonHealthReportDetach.IsEnabled = false;
            detachHR.Closing += OnHealthWindowClosing;
            detachHR.Owner = Application.Current.MainWindow;
            detachHR.Show();
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
            buttonHealthReportDetach.IsEnabled = true;
        }
    }
}
