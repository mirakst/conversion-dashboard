using System;
using System.Linq;
using System.Threading;
using DashboardFrontend.ViewModels;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Model;

namespace DashboardFrontend.Charts
{
    /// <summary>
    /// A class for the creating and controlling <see cref="ISeries"/>
    /// </summary>
    public class DataChart : BaseViewModel
    {

        public DataChart()
        {
            AutoFocusOn();
        }

        public DataChart(BaseChart chart) : this()
        {
            ChartData = chart;
            
        }

        #region public

        public BaseChart ChartData { get; set; }

        private double? _lastRamReading = 0;
        public double? LastRamReading
        {
            get => _lastRamReading;
            set
            {
                _lastRamReading = value;
                OnPropertyChanged(nameof(LastRamReading));
            }
        }
        private DateTime LastRamPlot { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
        private double? _lastCpuReading = 0;
        public double? LastCpuReading
        {
            get => _lastCpuReading;
            set
            {
                _lastCpuReading = value;
                OnPropertyChanged(nameof(LastCpuReading));
            }
        }
        private DateTime LastCpuPlot { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
        private DateTime LastNetPlot { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
        #endregion

        #region private
        private PeriodicTimer? _autoFocusTimer;
        private bool _isAutoFocusTimer = false;
        private int _maxView = 10;
        #endregion

        /// <summary>
        /// Adds points to the chart.
        /// </summary>
        public void UpdateData(Ram? ram, Cpu? cpu)
        {
            
            
            if (ram is null || cpu is null) return;
            foreach (var item in ram.Readings.Where(e => e.Date > LastRamPlot))
            {
                ChartData.Values[0].Add(CreatePoint(item));       
            }

            foreach (var item in cpu.Readings.Where(e => e.Date > LastCpuPlot))
            {
                ChartData.Values[1].Add(CreatePoint(item));
            }
            try
            {
                LastRamReading = ram.Readings.Last().Load * 100;
                LastCpuReading = cpu.Readings.Last().Load * 100;
                UpdatePlots(ram.Readings.Last().Date, cpu.Readings.Last().Date);
            }
            catch (InvalidOperationException ex)
            {
                LastRamReading = 0;
                LastCpuReading = 0;
            }
        }

        public void UpdateData(Network? network)
        {
            if (network is null) return;
            foreach (var item in network.Readings.Where(e => e.Date > LastNetPlot))
            {
                switch (ChartData.Type)
                {
                    case BaseChart.ChartType.Network:
                        UpdateNetworkData(item);
                        break;
                    case BaseChart.ChartType.NetworkDelta:
                        UpdateNetworkDeltaData(item);
                        break;
                    case BaseChart.ChartType.NetworkSpeed:
                        UpdateNetworkSpeedData(item);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void UpdateNetworkData(NetworkUsage reading)
        {
            ChartData.Values[0].Add(CreatePoint(reading.BytesSend, reading.Date));
            ChartData.Values[1].Add(CreatePoint(reading.BytesReceived, reading.Date));
        }

        public void UpdateNetworkDeltaData(NetworkUsage reading)
        {
            ChartData.Values[0].Add(CreatePoint(reading.BytesSendDelta, reading.Date));
            ChartData.Values[1].Add(CreatePoint(reading.BytesReceivedDelta, reading.Date));
        }

        public void UpdateNetworkSpeedData(NetworkUsage reading)
        {
            ChartData.Values[0].Add(CreatePoint(reading.BytesSendSpeed, reading.Date));
            ChartData.Values[1].Add(CreatePoint(reading.BytesReceivedSpeed, reading.Date));
        }

        private void UpdatePlots(DateTime ramDate, DateTime cpuDate)
        {
            LastRamPlot = ramDate;
            LastCpuPlot = cpuDate;
        }

        /// <summary>
        /// Creates a point from a performance metric.
        /// </summary>
        private ObservablePoint CreatePoint(PerformanceMetric pointData)
        {
            return new ObservablePoint(pointData.Date.ToOADate(), pointData.Load);
        }

        /// <summary>
        /// Creates a new <see cref="ObservablePoint"/> from <see cref="NetworkUsage"/> data.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private ObservablePoint CreatePoint(long bytes, DateTime time)
        {
            return new ObservablePoint(time.ToOADate(), Convert.ToDouble(bytes));
        }

        #region AutoFocus Functions
        /// <summary>
        /// Disables AutoFocusOn().
        /// </summary>
        public void AutoFocusOff()
        {
            _autoFocusTimer?.Dispose();
            _isAutoFocusTimer = false;
        }

        /// <summary>
        /// Aync function that focus the chart on the most resent entries, within a given time span.
        /// </summary>
        public async void AutoFocusOn()
        {
            if (!_isAutoFocusTimer)
            {
                _autoFocusTimer = new(TimeSpan.FromMilliseconds(100));
                _isAutoFocusTimer = true;

                while (await _autoFocusTimer.WaitForNextTickAsync())
                {
                    if (ChartData.Values.Count > 0 && ChartData.Values.First().Count > 0)
                    {
                        ChartData.XAxis[0].MinLimit = ChartData.Values.First().Count >= _maxView ? ChartData.Values.First().Last().X.Value - DateTime.FromBinary(TimeSpan.FromMinutes(_maxView).Ticks).ToOADate() :
                            ChartData.Values.First().First().X.Value;
                        ChartData.XAxis[0].MaxLimit = ChartData.Values.First().Last().X.Value;
                    }
                }
            }
        }
        #endregion

        #region Settings Functions
        /// <summary>
        /// Change max viewable time span on the graph while auto focusing.
        /// </summary>
        /// <param name="input"></param>
        public void ChangeMaxView(int input)
        {
            _maxView = input;
        }

        /// <summary>
        /// Sets the smoothness of curves on all <see cref="ISeries"/>.
        /// </summary>
        /// <param name = "input">A number between 0 and 1. Standart is 1.</param>
        public void ChangeLineSmoothness(double input)
        {
            if (input < 0 || input > 1) { return; }

            foreach (LineSeries<ObservablePoint> line in ChartData.Series)
            {
                line.LineSmoothness = input;
            }
        }

        /// <summary>
        /// Sets the size of line points on all <see cref="ISeries"/>.
        /// </summary>
        /// <param name="input">A number between 0 and 60. less is smaller. Standart is 10.</param>
        public void ChangePointSize(int input)
        {
            if (input > 60) { return; }
            else if (input < 0) { return; }

            foreach (LineSeries<ObservablePoint> line in ChartData.Series)
            {
                line.GeometrySize = input;
            }
        }
        #endregion
    }
}
