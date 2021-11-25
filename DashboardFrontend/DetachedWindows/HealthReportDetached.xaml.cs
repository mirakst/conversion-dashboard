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
        }

        private void ComboBoxPerformanceMaxView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            /*_ =int.TryParse(((FrameworkElement)ComboBoxPerformanceMaxView.SelectedItem).Tag as string, out int comboBoxItemValue);
            Vm.ChangeMaxView(comboBoxItemValue);*/
        }

        private void ComboBoxNetworkMaxView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            /*_ = int.TryParse(((FrameworkElement)ComboBoxNetworkMaxView.SelectedItem).Tag as string, out int comboBoxItemValue);
            _netVm.ChangeMaxView(comboBoxItemValue);*/
        }

        private void CartesianChart_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Vm.SystemLoadChart.AutoFocusOn();
            /*_netVm.AutoFocusOn();*/
        }

        private void CartesianChart_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Vm.SystemLoadChart.AutoFocusOff();
            /*_netVm.AutoFocusOff();*/

        }
    }
}
