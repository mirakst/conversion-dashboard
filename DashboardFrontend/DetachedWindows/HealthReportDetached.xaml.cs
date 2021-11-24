using DashboardFrontend.ViewModels;
using System.Windows;

namespace DashboardFrontend.DetachedWindows
{
    public partial class HealthReportDetached
    {
        public NetworkChart NetworkChart { get; set; } = new();
        public PerformanceChart PerformanceChart { get; private set; } = new();
        public LiveChartViewModel PerformanceViewModel { get; private set; }
        public LiveChartViewModel NetworkViewModel { get; private set; }

        public HealthReportDetached()
        {
            PerformanceViewModel = new(PerformanceChart);
            NetworkViewModel = new(NetworkChart);

            InitializeComponent();

            DataContext = this;
        }

        private void ComboBoxPerformanceMaxView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _=int.TryParse(((FrameworkElement)ComboBoxPerformanceMaxView.SelectedItem).Tag as string, out int comboBoxItemValue);
            PerformanceViewModel.ChangeMaxView(comboBoxItemValue);
        }

        private void CartesianChart_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PerformanceViewModel.AutoFocusOn();
            NetworkViewModel.AutoFocusOn();
        }

        private void CartesianChart_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PerformanceViewModel.AutoFocusOff();
            NetworkViewModel.AutoFocusOff();
        }

        private void ComboBoxNetworkMaxView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _=int.TryParse(((FrameworkElement)ComboBoxNetworkMaxView.SelectedItem).Tag as string, out int comboBoxItemValue);
            NetworkViewModel.ChangeMaxView(comboBoxItemValue);
        }
    }
}
