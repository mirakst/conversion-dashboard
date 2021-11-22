using DashboardBackend;
using DashboardBackend.Database;
using DashboardFrontend.DetachedWindows;
using DashboardFrontend.ViewModels;
using Model;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DashboardFrontend
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataUtilities.DatabaseHandler = new SqlDatabase();
            ValidationReport = new();
            ValidationReport.ValidationTests = DataUtilities.GetAfstemninger();
            gridValidationReport.DataContext = new ValidationReportViewModel(ValidationReport, validationsDataGrid);
        }

        public ValidationReport ValidationReport { get; set; }

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
            LogDetached detachLog = new();
            //detachLog.Closing += OnLogWindowClosing;
            buttonLogDetach.IsEnabled = false;
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
            HealthReportDetached detachHR = new();
            buttonHealthReportDetach.IsEnabled = false;
            detachHR.Closing += OnHealthWindowClosing;
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

        private void validationsDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = validationsDataGrid
            };
            validationsDataGrid.RaiseEvent(eventArgs);
        }

        private void DetailsDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = validationsDataGrid
            };
            validationsDataGrid.RaiseEvent(eventArgs);
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
