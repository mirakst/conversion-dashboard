using DashboardFrontend.ViewModels;
using System.Windows;

namespace DashboardFrontend.DetachedWindows
{
    public partial class HealthReportDetached
    {
        public HealthReportViewModel Vm { get; set; }

        public HealthReportDetached(HealthReportViewModel healthReportViewModel)
        {
            Vm = healthReportViewModel;

            InitializeComponent();

            DataContext = this;
        }

        private void ComboBoxMaxView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _ = int.TryParse(((FrameworkElement)ComboBoxMaxView.SelectedItem).Tag as string, out int comboBoxItemValue);
            Vm.SystemLoadChart.ChangeMaxView(comboBoxItemValue);
            Vm.NetworkChart.ChangeMaxView(comboBoxItemValue);
        }

        private void CartesianChart_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Vm.SystemLoadChart.AutoFocusOn();
            Vm.NetworkChart.AutoFocusOn();
        }

        private void CartesianChart_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Vm.SystemLoadChart.AutoFocusOff();
            Vm.NetworkChart.AutoFocusOff();
        }
    }
}
