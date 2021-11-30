using DashboardFrontend.Charts;

namespace DashboardFrontend.ViewModels
{
    public class HealthReportViewModel : BaseViewModel
    {
        public HealthReportViewModel()
        {
        }
        public DataChart SystemLoadChart { get; set; } = new DataChart(new PerformanceChart());
        public DataChart NetworkChart { get; set; } = new DataChart(new NetworkChart());
        public DataChart NetworkDeltaChart { get; set; } = new DataChart(new NetworkDeltaChart());
        public DataChart NetworkSpeedChart { get; set; } = new DataChart(new NetworkSpeedChart());
    }
}
