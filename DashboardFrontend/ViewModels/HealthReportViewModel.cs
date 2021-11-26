namespace DashboardFrontend.ViewModels
{
    public class HealthReportViewModel : BaseViewModel
    {
        public HealthReportViewModel()
        {
            SystemLoadChart = new DataChart(new PerformanceChart());
        }
        public DataChart SystemLoadChart { get; set; }
        public DataChart NetworkUsageChart { get; set; } = new DataChart(new NetworkChart());
        public DataChart ManagerChart { get; set; }
    }
}
