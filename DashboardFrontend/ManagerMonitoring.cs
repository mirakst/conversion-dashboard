using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using InteractiveDataDisplay.Core;
using DashboardFrontend.DetachedWindows;
using Model;
using System.Threading;

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


        public void AddLine(List<DateTime> _timeList, List<long> _readingsList, Grid chart, string _name, string _description)
        {
            LineGraph line = new()
            {
                Name = _name,
                Description = _description
            };

                chart.Children.Add(line);
                _dataCollections.Add(new DataClass(_timeList, _readingsList, line));
        }

        public async void GenerateData(PeriodicTimer _timer, Chart _chart)
        {
            Random random = new();

            while (await _timer.WaitForNextTickAsync())
            {
                long MaxView = TimeSpan.TicksPerMinute * UserViewInput;

                foreach (DataClass dataCollection in _dataCollections)
                {
                    /* Random data should be replaced with actual data */
                    dataCollection.Time.Add(DateTime.Now);
                    dataCollection.Readings.Add((long)random.Next(0, 100));

                    UpdateChart(dataCollection);
                }

                _chart.PlotHeight = 103;

                if (!_chart.IsMouseOver)
                {
                    _chart.PlotOriginX = DateTime.Now.AddMinutes(-(UserViewInput * 0.9)).Ticks;
                    _chart.PlotWidth = MaxView;
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
