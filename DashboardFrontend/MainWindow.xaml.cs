using DashboardFrontend;
using DashboardFrontend.DetachedWindows;
using DashboardFrontend.ViewModels;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace DashboardInterface
{
    public partial class MainWindow : Window
    {
        public PerformanceViewModel PerformanceViewModel { get; private set; } = new();
        public LiveChartViewModel LiveChartViewModel { get; private set; }

        private bool _isStarted;

        public MainWindow()
        {
            LiveChartViewModel = new(PerformanceViewModel.Series, PerformanceViewModel.PerformanceData, PerformanceViewModel.XAxis, PerformanceViewModel.YAxis);

            InitializeComponent();

            DataContext = this;
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
                _isStarted = true;
            }
            else
            {
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

            if (_isStarted)
            {
            }

            ButtonHealthReportDetach.IsEnabled = false;
            expandHr.Closing += OnHealthWindowClosing;
            
            expandHr.Show();
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
            ButtonHealthReportDetach.IsEnabled = true;
        }

        private void CartesianChart_MouseLeave(object sender, MouseEventArgs e)
        {
            LiveChartViewModel.AutoFocusOn();

        }

        private void CartesianChart_MouseEnter(object sender, MouseEventArgs e)
        {
            LiveChartViewModel.AutoFocusOff();
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _=int.TryParse(((FrameworkElement)comboBoxMaxView.SelectedItem).Tag as string, out int comboBoxItemValue);
            LiveChartViewModel.ChangeMaxView(comboBoxItemValue);
        }
    }
}
