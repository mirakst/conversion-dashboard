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
        private PeriodicTimer? _dataGenerationTimer;
        private HealthReportMonitoring? _monitoring;

        /* List' should be removed once RAM and CPU and Network data is available */
        private readonly List<DateTime> _ramDateTime = new();
        private readonly List<long> _ramReadings = new();

        private readonly List<DateTime> _cpuDateTime = new();
        private readonly List<long> _cpuReadings = new();

        private readonly List<DateTime> _networkSendDateTime = new();
        private readonly List<long> _networkSendReadings = new();
        private readonly List<DateTime> _networkReceivedDateTime = new();
        private readonly List<long> _networkReceivedReadings = new();
        /* To here */

        public void PerformanceMonitoringStart(Chart _chart, Grid _grid, TextBox _userChartTimeInterval)
        {
            _dataGenerationTimer  = new PeriodicTimer(TimeSpan.FromSeconds(1));
            _monitoring = new HealthReportMonitoring();

            _grid.Children.Clear();
            _monitoring.Add(_ramDateTime, _ramReadings, _grid, "ramUsage", "RAM Usage", Color.FromRgb(133, 222, 118), 2);
            _monitoring.Add(_cpuDateTime, _cpuReadings, _grid, "cpuLoad", "CPU Load", Color.FromRgb(245, 88, 47), 2);
            _monitoring.GenerateData(_dataGenerationTimer, _chart, _userChartTimeInterval);
        }

        public void NetworkMonitoringStart(Chart _chart, Grid _grid, TextBox _userChartTimeInterval)
        {
            _dataGenerationTimer  = new PeriodicTimer(TimeSpan.FromSeconds(1));
            _monitoring = new HealthReportMonitoring();

            _monitoring.Add(_networkSendDateTime, _networkSendReadings, _grid, "networkSend", "Send", Color.FromRgb(0, 0, 255), 2);
            _monitoring.Add(_networkReceivedDateTime, _networkReceivedReadings, _grid, "networkReceived", "Received", Color.FromRgb(255, 0, 0), 2);
            _monitoring.GenerateData(_dataGenerationTimer, _chart, _userChartTimeInterval);
        }

        public void Dispose()
        {
            _dataGenerationTimer?.Dispose();
        }
    }
}
