using InteractiveDataDisplay.Core;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;

namespace DashboardFrontend
{
    public class ManagerMonitoring
    {

        private class DataClass
        {
            public List<DateTime> Time = new();
            public List<long> Readings = new();
            public LineGraph line;

            public DataClass(List<DateTime> _time, List<long> _readings, LineGraph _line)
            {
                Time = _time;
                Readings = _readings;
                line = _line;
            }
        }

        private List<DataClass> _dataCollections = new();
        public int UserViewInput { get; set; } = 2;

        public List<DateTime> _timestampsCpu = new();
        public List<long> _readingsCpu = new();

        public List<DateTime> _timestampsRam = new();
        public List<long> _readingsRam = new();

        public List<DateTime> _timestampsRead = new();
        public List<long> _readingsRead = new();

        public List<DateTime> _timestampsWritten = new();
        public List<long> _readingsWritten = new();

        public void InitializeCharts(ManagerMonitoring managerMonitoring, List<Manager> managers, Grid chartCpu, Grid chartRam, Grid chartRead, Grid chartWrittten)
        {
            for (int i = 0; i < managers.Count; i++)
            {
                managerMonitoring.AddLine(_timestampsCpu, _readingsCpu, chartCpu, managers[i].Name.Split('.').Last(), managers[i].Name.Split('.').Last());
                managerMonitoring.AddLine(_timestampsRam, _readingsRam, chartRam, managers[i].Name.Split('.').Last(), managers[i].Name.Split('.').Last());
                managerMonitoring.AddLine(_timestampsRead, _readingsRead, chartRead, managers[i].Name.Split('.').Last(), managers[i].Name.Split('.').Last());
                managerMonitoring.AddLine(_timestampsWritten, _readingsWritten, chartWrittten, managers[i].Name.Split('.').Last(), managers[i].Name.Split('.').Last());
            }
        }

        public void AddLine(List<DateTime> _timeList, List<long> _readingsList, Grid chart, string _name, string _description)
        {
            Random r = new();

            LineGraph line = new()
            {
                Name = _name,
                Description = _description,
                Stroke = new SolidColorBrush(Color.FromArgb((byte)r.Next(256), (byte)r.Next(256), (byte)r.Next(265), 0))
            };

            chart.Children.Add(line);
            _dataCollections.Add(new DataClass(_timeList, _readingsList, line));
        }

        public async void GenerateData(PeriodicTimer _timer, Chart _chartCpu, Chart _chartRam, Chart _chartRead, Chart _chartWritten)
        {
            List<Chart> charts = new();
            Random random = new();

            while (await _timer.WaitForNextTickAsync())
            {
                long MaxView = TimeSpan.TicksPerMinute * UserViewInput;

                foreach (DataClass dataCollection in _dataCollections)
                {
                    dataCollection.Time.Add(DateTime.Now);
                    dataCollection.Readings.Add((long)random.Next(0, 100));

                    UpdateChart(dataCollection);
                }

                foreach (Chart chart in charts)
                {
                    chart.PlotHeight = 103;
                }

                if (!_chartCpu.IsMouseOver || !_chartRam.IsMouseOver || !_chartRead.IsMouseOver || !_chartWritten.IsMouseOver)
                {
                    _chartCpu.PlotOriginX = DateTime.Now.AddMinutes(-(UserViewInput * 0.9)).Ticks;
                    _chartCpu.PlotWidth = MaxView;
                    _chartRam.PlotOriginX = DateTime.Now.AddMinutes(-(UserViewInput * 0.9)).Ticks;
                    _chartRam.PlotWidth = MaxView;
                    _chartRead.PlotOriginX = DateTime.Now.AddMinutes(-(UserViewInput * 0.9)).Ticks;
                    _chartRead.PlotWidth = MaxView;
                    _chartWritten.PlotOriginX = DateTime.Now.AddMinutes(-(UserViewInput * 0.9)).Ticks;
                    _chartWritten.PlotWidth = MaxView;
                }
            }
        }

        private static void UpdateChart(DataClass _dataCollection)
        {
            var _valueY = _dataCollection.Time.Select(e => e.Ticks).ToList();

            _dataCollection.line.Plot(_valueY, _dataCollection.Readings);
        }
    }
}
