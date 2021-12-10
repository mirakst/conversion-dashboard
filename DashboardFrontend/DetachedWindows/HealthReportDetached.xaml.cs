using System;
using System.Diagnostics;
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

        private void ComboBoxMaxView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _ = int.TryParse(((FrameworkElement)ComboBoxMaxView.SelectedItem).Tag as string, out int comboBoxItemValue);
            Vm.SystemLoadChart.ChangeMaxView(comboBoxItemValue);
            Vm.NetworkChart.ChangeMaxView(comboBoxItemValue);
            Vm.NetworkDeltaChart.ChangeMaxView(comboBoxItemValue);
            Vm.NetworkSpeedChart.ChangeMaxView(comboBoxItemValue);
        }

        private void CartesianChart_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChartWrapper? chart = (ChartWrapper)(sender as CartesianChart)?.DataContext!;
            chart?.AutoFocusOn();
        }

        private void CartesianChart_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChartWrapper? chart = (ChartWrapper)(sender as CartesianChart)?.DataContext!;
            chart?.AutoFocusOff();
        }
    }
}
