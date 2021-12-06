using DashboardFrontend.Charts;

namespace DashboardFrontend.ViewModels
{
    public class HealthReportViewModel : BaseViewModel
    {
        public HealthReportViewModel()
        {
        }
        public System.DateTime LastUpdated { get; set; }
        public DataChart SystemLoadChart { get; set; } = new DataChart(new PerformanceChart(), true);
        public DataChart NetworkChart { get; set; } = new DataChart(new NetworkChart(), true);
        public DataChart NetworkDeltaChart { get; set; } = new DataChart(new NetworkDeltaChart(), true);
        public DataChart NetworkSpeedChart { get; set; } = new DataChart(new NetworkSpeedChart(), true);
    }
}
