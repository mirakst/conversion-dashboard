using System;
using System.Windows.Controls;
using DashboardFrontend.ViewModels;
using Microsoft.EntityFrameworkCore;
using DU = DashboardBackend.DataUtilities;
using Model;

namespace DashboardFrontend
{
    public class Controller
    {
        private MainWindowViewModel _vm;
        private Log log;
        private ValidationReport validationReport;
        private HealthReport healthReport;

        public Controller(MainWindowViewModel viewModel)
        {
            _vm = viewModel;
            log = new();
            validationReport = new();
            healthReport = DU.BuildHealthReport();
        }

        public void Initialize(DataGrid dataGridValidations)
        {
            _vm.LogViewModel = new();
            _vm.ValidationReportViewModel = new(dataGridValidations);
            _vm.LiveChartViewModel = new(new PerformanceChart());
        }

        public void UpdateLog(DateTime timestamp)
        {
            log.Messages.AddRange(DU.GetLogMessages(timestamp));
            _vm.LogViewModel.UpdateData(log);
        }

        public void UpdateValidationReport(DateTime timestamp)
        {
            validationReport.ValidationTests.AddRange(DU.GetAfstemninger(timestamp));
            _vm.ValidationReportViewModel.UpdateData(validationReport);
        }

        public void UpdateHealthReport(DateTime timestamp)
        {
            DU.AddHealthReportReadings(healthReport, timestamp);
            _vm.LiveChartViewModel.UpdateData(healthReport.Ram, healthReport.Cpu);
        }
    }
}
