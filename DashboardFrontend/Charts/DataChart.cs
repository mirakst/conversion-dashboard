using System;
using System.Collections.ObjectModel;
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
        public DataChart(BaseChart chart, bool shouldAutoFocus)
        {
            ChartData = chart;
            if (shouldAutoFocus)
            {
                AutoFocusOn();
            }
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
        /// Adds a line to the chart and assigns a list of values to this line
        /// </summary>
        /// The line to be added <param name="Line"></param>
        /// The associated data to the line <param name="data"></param>
        public void AddLine(ISeries Line, ObservableCollection<ObservablePoint> data)
        {
            ChartData.Values.Add(data);
            ChartData.Series.Add(Line);
            ChartData.Series[ChartData.Series.IndexOf(Line)].Values = ChartData.Values[ChartData.Series.IndexOf(Line)];
        }

        /// <summary>
        /// Removes a line from the chart but keeps its data in storage
        /// </summary>
        /// The line to be removed <param name="dataName"></param>
        /// The data associated with the line <param name="data"></param>
        public void RemoveData(string dataName, ObservableCollection<ObservablePoint> data)
        {
            ChartData.Values.Remove(data);
            string shortName = dataName.Split('.').Last();
            int managerIndex = ChartData.Series.FindIndex(e => e.Name == shortName);
            ChartData.Series.RemoveAt(managerIndex);
        }

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
            if (cpu.Readings.Count > 0 && ram.Readings.Count > 0) 
            {
                LastRamReading = ram.Readings.Last().Load * 100;
                LastCpuReading = cpu.Readings.Last().Load * 100;
                UpdatePlots(ram.Readings.Last().Date, cpu.Readings.Last().Date);
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
            if (network.Readings.Count > 0)
            {
                LastNetPlot = network.Readings.Last().Date;
            }
        }

        public void UpdateNetworkData(NetworkUsage reading)
        {
            double bytesSendFormatted = reading.BytesSend / Math.Pow(1024, 3);
            double bytesReceivedFormatted = reading.BytesReceived / Math.Pow(1024, 3);
            ChartData.Values[0].Add(CreatePoint(bytesSendFormatted, reading.Date));
            ChartData.Values[1].Add(CreatePoint(bytesReceivedFormatted, reading.Date));
        }

        public void UpdateNetworkDeltaData(NetworkUsage reading)
        {
            double bytesSendFormatted = reading.BytesSendDelta / Math.Pow(1024, 2);
            double bytesReceivedFormatted = reading.BytesReceivedDelta / Math.Pow(1024, 2);
            ChartData.Values[0].Add(CreatePoint(bytesSendFormatted, reading.Date));
            ChartData.Values[1].Add(CreatePoint(bytesReceivedFormatted, reading.Date));
        }

        public void UpdateNetworkSpeedData(NetworkUsage reading)
        {
            double bytesSendFormatted = reading.BytesSendSpeed / Math.Pow(1024, 2);
            double bytesReceivedFormatted = reading.BytesReceivedSpeed / Math.Pow(1024, 2);
            ChartData.Values[0].Add(CreatePoint(bytesSendFormatted, reading.Date));
            ChartData.Values[1].Add(CreatePoint(bytesReceivedFormatted, reading.Date));
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
        private ObservablePoint CreatePoint(double readingSize, DateTime time)
        {
            return new ObservablePoint(time.ToOADate(), readingSize);
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
                    if (ChartData.Values.Count > 0 && ChartData.Values.First()?.Count > 0)
                    {
                        ChartData.XAxis[0].MinLimit = ChartData.Values.First().Count >= _maxView
                            ? ChartData.Values.First().Last().X.Value - DateTime.FromBinary(TimeSpan.FromMinutes(_maxView).Ticks).ToOADate() 
                            : ChartData.Values.First().First().X.Value;
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
