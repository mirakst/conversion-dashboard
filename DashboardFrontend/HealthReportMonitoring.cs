using InteractiveDataDisplay.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;

namespace DashboardFrontend
{
    public class HealthReportMonitoring
    {
        //UserViewInput should be bound to a user input
        public int UserViewInput { get; set; } = 2;
        private List<DataClass> _dataCollections = new();

        /// <summary>
        /// A class constructed of [DateTime] list, [long] List and a [LineGraph].
        /// </summary>
        private class DataClass
        {
            public List<DateTime> Time = new();
            public List<long> Readings = new();
            public LineGraph Line;

            public DataClass(List<DateTime> _time, List<long> _readings, LineGraph line)
            {
                Time = _time;
                Readings = _readings;
                Line = line;
            }
        }

        /// <summary>
        /// A function to create a new DataClass and add it ti the dataCollections list.
        /// </summary>
        /// <param name="_timeList"></param>
        /// <param name="_readingsList"></param>
        /// <param name="myGrid"></param>
        /// <param name="_name"></param>
        /// <param name="_description"></param>
        /// <param name="_color"></param>
        /// <param name="_strokeThickness"></param>
        public void Add(List<DateTime> _timeList, List<long> _readingsList, Grid myGrid, string _name, string _description, Color _color, int _strokeThickness)
        {
            LineGraph line = new()
            {
                Name = _name,
                Description = _description,
                Stroke = new SolidColorBrush(_color),
                StrokeThickness = _strokeThickness
            };

            myGrid.Children.Add(line);
            _dataCollections.Add(new DataClass(_timeList, _readingsList, line));
        }

        public void Clear()
        {
            _dataCollections.Clear();
        }

        /// <summary>
        /// A function for generationg data for all DataClass'.
        /// </summary>
        /// <param name="_timer">A [PeriodicTimer] that calls the function.</param>
        /// <param name="_chart">The [Chart] each [DataCollection] should be plotted on.</param>
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

                _chart.PlotHeight = 105;

                if (!_chart.IsMouseOver)
                {
                    _chart.PlotOriginX = DateTime.Now.AddMinutes(-(UserViewInput * 0.9)).Ticks;
                    _chart.PlotWidth = MaxView;
                }
            }
        }

        /// <summary>
        /// Updates a graph, using a data collection.
        /// </summary>
        /// <param name="_dataCollection"></param>
        private static void UpdateChart(DataClass _dataCollection)
        {
            var _valueY = _dataCollection.Time.Select(e => e.Ticks).ToList();

            _dataCollection.Line.Plot(_valueY, _dataCollection.Readings);
        }
    }
}
