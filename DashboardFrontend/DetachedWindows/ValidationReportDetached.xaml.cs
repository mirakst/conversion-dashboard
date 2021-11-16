using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Model;
using DashboardBackend;
using DashboardFrontend.ViewModels;
using System.Diagnostics;

namespace DashboardFrontend.DetachedWindows
{
    public partial class ValidationReportDetached : Window
    {
        public ValidationReportDetached(ValidationReport validationReport)
        {
            InitializeComponent();
            ValidationReport = validationReport;
            ViewModel = new(validationReport);
            DataContext = ViewModel;
        }
        
        public ValidationReport ValidationReport { get; set; }
        public ValidationReportViewModel ViewModel { get; set; }

        private void ExpandRow_Click(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
        }
    }
}


