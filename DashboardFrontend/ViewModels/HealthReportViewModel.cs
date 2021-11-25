using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DashboardFrontend.ViewModels
{
    public class HealthReportViewModel : BaseViewModel
    {
        public HealthReportViewModel(HealthReport hr)
        {
            SystemLoadChart = new DataChart(new PerformanceChart(), hr.Cpu, hr.Ram);
        }
        public DataChart SystemLoadChart { get; set; }
        public DataChart NetworkUsageChart { get; set; } = new DataChart(new NetworkChart(), null, null);
        public DataChart ManagerChart { get; set; }
    }
}
