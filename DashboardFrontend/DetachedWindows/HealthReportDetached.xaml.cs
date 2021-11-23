using DashboardFrontend.ViewModels;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace DashboardFrontend.DetachedWindows
{
    public partial class HealthReportDetached
    {
        public NetworkViewModel NetworkViewModel { get; set; } = new();
        public PerformanceViewModel PerformanceViewModel { get; private set; } = new();
        public LiveChartViewModel PerformanceChart { get; private set; }
        public LiveChartViewModel NetworkChart { get; private set; }

        public PeriodicTimer PerformanceTimer;
        public PeriodicTimer NetworkTimer;

        public HealthReportDetached()
        {
            PerformanceChart = new(PerformanceViewModel.Series, PerformanceViewModel.PerformanceData, PerformanceViewModel.XAxis, PerformanceViewModel.YAxis);
            NetworkChart = new(NetworkViewModel.Series, NetworkViewModel.NetworkData, NetworkViewModel.XAxis, NetworkViewModel.YAxis);

            InitializeComponent();

            PerformanceTimer = new(TimeSpan.FromSeconds(2));
            PerformanceChart.StartGraph(PerformanceTimer);

            NetworkTimer = new(TimeSpan.FromSeconds(2));
            NetworkChart.StartGraph(NetworkTimer);

            DataContext = this;
        }

        private void OnClosing(object? sender, CancelEventArgs e)
        {
            PerformanceTimer.Dispose();
            NetworkTimer.Dispose();
        }

        private void ComboBoxPerformanceMaxView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _=int.TryParse(((FrameworkElement)ComboBoxPerformanceMaxView.SelectedItem).Tag as string, out int comboBoxItemValue);
            PerformanceChart.ChangeMaxView(comboBoxItemValue);
        }

        private void CartesianChart_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PerformanceChart.AutoFocusOn();
            NetworkChart.AutoFocusOn();
        }

        private void CartesianChart_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PerformanceChart.AutoFocusOff();
            NetworkChart.AutoFocusOff();
        }

        private void ComboBoxNetworkMaxView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _=int.TryParse(((FrameworkElement)ComboBoxNetworkMaxView.SelectedItem).Tag as string, out int comboBoxItemValue);
            NetworkChart.ChangeMaxView(comboBoxItemValue);
        }
    }
}
