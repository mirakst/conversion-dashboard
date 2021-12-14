using DashboardFrontend.Charts;

namespace DashboardFrontend.ViewModels
{
    public class HealthReportViewModel : BaseViewModel
    {
        public HealthReportViewModel()
        {
        }
        public System.DateTime LastUpdated { get; set; }
        public PerformanceChartWrapper SystemLoadChart { get; set; } = new(new PerformanceChartTemplate(), true);
        public NetworkChartWrapper NetworkChart { get; set; } = new(new NetworkChartTemplate(), true);
        public NetworkChartWrapper NetworkDeltaChart { get; set; } = new(new NetworkDeltaChartTemplate(), true);
        public NetworkChartWrapper NetworkSpeedChart { get; set; } = new(new NetworkSpeedChartTemplate(), true);
    }
}
