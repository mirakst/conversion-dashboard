using DashboardFrontend.ViewModels;
using System.Windows;

namespace DashboardFrontend.DetachedWindows
{
    public partial class HealthReportDetached
    {
        public NetworkViewModel NetworkViewModel { get; set; } = new();
        public PerformanceViewModel PerformanceViewModel { get; private set; } = new();
        public LiveChartViewModel PerformanceChart { get; private set; }
        public LiveChartViewModel NetworkChart { get; private set; }
        public LiveChartViewModel NetworkDeltaChart { get; private set; }
        public LiveChartViewModel NetworkSpeedChart { get; private set; }

        public HealthReportDetached()
        {
            PerformanceChart = new(PerformanceViewModel.Series, PerformanceViewModel.PerformanceData, PerformanceViewModel.XAxis, PerformanceViewModel.YAxis);
            NetworkChart = new(NetworkViewModel.NetworkSeries, NetworkViewModel.NetworkData, NetworkViewModel.XAxis, NetworkViewModel.YAxis);
            NetworkDeltaChart = new(NetworkViewModel.NetworkDeltaSeries, NetworkViewModel.NetworkDelta, NetworkViewModel.XAxis, NetworkViewModel.YAxisDelta);
            NetworkSpeedChart = new(NetworkViewModel.NetworkSpeedSeries, NetworkViewModel.NetworkSpeed, NetworkViewModel.XAxis, NetworkViewModel.YAxisSpeed);

            InitializeComponent();

            DataContext = this;
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
