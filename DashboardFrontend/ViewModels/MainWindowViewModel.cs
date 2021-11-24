using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DashboardFrontend.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel(Log log, ValidationReport validationReport, DataGrid validationsDataGrid)
        {
            LogViewModel = new(log);
            ValidationReportViewModel = new(validationReport, validationsDataGrid);
        }

        public LogViewModel LogViewModel { get; set; }
        public ValidationReportViewModel ValidationReportViewModel { get; set; }
    }
}
