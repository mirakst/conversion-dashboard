using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Model;
using DashboardFrontend.ViewModels;

namespace DashboardFrontend.DetachedWindows
{
    public partial class ValidationReportDetached
    {
        public ValidationReportDetached(ValidationReportViewModel validationReportViewModel)
        {
            InitializeComponent();
            validationReportViewModel.DataGrid = DataGridValidations;
            DataContext = validationReportViewModel;
        }
        
        public ValidationReport ValidationReport { get; set; }
        public ValidationReportViewModel ViewModel { get; set; }

        private void validationsDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArgs.RoutedEvent = MouseWheelEvent;
            eventArgs.Source = DataGridValidations;
            DataGridValidations.RaiseEvent(eventArgs);
        }

        private void DetailsDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArgs.RoutedEvent = MouseWheelEvent;
            eventArgs.Source = DataGridValidations;
            DataGridValidations.RaiseEvent(eventArgs);
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


