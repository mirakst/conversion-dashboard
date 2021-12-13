using DashboardFrontend.ViewModels;
using System.Windows;
using DashboardFrontend.Charts;
using LiveChartsCore.SkiaSharpView.WPF;

namespace DashboardFrontend.DetachedWindows
{
    public partial class HealthReportDetached
    {
        public HealthReportViewModel Vm { get; set; }

        public HealthReportDetached(HealthReportViewModel healthReportViewModel)
        {
            Vm = healthReportViewModel;

            InitializeComponent();

            DataContext = Vm;
        }

        /// <summary>
        /// Updates graph max views to combo box value.
        /// </summary>
        private void ComboBoxMaxView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _ = int.TryParse(((FrameworkElement)ComboBoxMaxView.SelectedItem).Tag as string, out int comboBoxItemValue);
            Vm.SystemLoadChart.ChangeMaxView(comboBoxItemValue);
            Vm.NetworkChart.ChangeMaxView(comboBoxItemValue);
            Vm.NetworkDeltaChart.ChangeMaxView(comboBoxItemValue);
            Vm.NetworkSpeedChart.ChangeMaxView(comboBoxItemValue);
        }

        /// <summary>
        /// Enables autofocus when mouse isn't over chart.
        /// </summary>
        private void CartesianChart_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChartWrapper? chart = (ChartWrapper)(sender as CartesianChart)?.DataContext!;
            chart?.AutoFocusOn();
        }

        /// <summary>
        /// Disables autofocus when mouse is over chart.
        /// </summary>
        private void CartesianChart_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChartWrapper? chart = (ChartWrapper)(sender as CartesianChart)?.DataContext!;
            chart?.AutoFocusOff();
        }
    }
}
