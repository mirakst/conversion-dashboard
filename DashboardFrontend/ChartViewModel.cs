using InteractiveDataDisplay.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace DashboardFrontend
{
    public class ChartViewModel
    {
        private PeriodicTimer DataGenerationTimer;
        private HealthReportMonitoring Monitoring;

        /* List' should be removed once RAM and CPU and Network data is available */
        private List<DateTime> RAMDateTime = new();
        private List<long> RamReadings = new();

        private List<DateTime> CPUDateTime = new();
        private List<long> CpuReadings = new();

        private List<DateTime> NetworkSendDateTime = new();
        private List<long> NetworkSendReadings = new();
        private List<DateTime> NetworkRecivedDateTime = new();
        private List<long> NetworkRecivedReadings = new();
        /* To here */

        public void PerformanceMonitoringStart(Chart _chart, Grid _grid, TextBox _userChartTimeInterval)
        {
            DataGenerationTimer  = new(TimeSpan.FromSeconds(1));
            Monitoring = new();

            _grid.Children.Clear();
            Monitoring.Add(RAMDateTime, RamReadings, _grid, "ramUsage", "RAM Usage", Color.FromRgb(133, 222, 118), 2);
            Monitoring.Add(CPUDateTime, CpuReadings, _grid, "cpuLoad", "CPU Load", Color.FromRgb(245, 88, 47), 2);
            Monitoring.GenerateData(DataGenerationTimer, _chart, _userChartTimeInterval);
        }

        public void NetworkMonitoringStart(Chart _chart, Grid _grid, TextBox _userChartTimeInterval)
        {
            DataGenerationTimer  = new(TimeSpan.FromSeconds(1));
            Monitoring = new();

            Monitoring.Add(NetworkSendDateTime, NetworkSendReadings, _grid, "networkSend", "Send", Color.FromRgb(0, 0, 255), 2);
            Monitoring.Add(NetworkRecivedDateTime, NetworkRecivedReadings, _grid, "networkRecived", "Recived", Color.FromRgb(255, 0, 0), 2);
            Monitoring.GenerateData(DataGenerationTimer, _chart, _userChartTimeInterval);
        }

        public void Dispose()
        {
            DataGenerationTimer.Dispose();
        }
    }
}
