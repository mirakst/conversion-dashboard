using DashboardInterface;
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
        private readonly List<DataClass> _dataCollections = new();

        /// <summary>
        /// A class constructed of [DateTime] list, [long] List and a [LineGraph].
        /// </summary>
        private class DataClass
        {
            public readonly List<DateTime> Time;
            public readonly List<long> Readings;
            public readonly LineGraph Line;

            public DataClass(List<DateTime> _time, List<long> _readings, LineGraph line)
            {
                Time = _time;
                Readings = _readings;
                Line = line;
            }
        }

        /// <summary>
        /// A function to create a new DataClass and add it to the dataCollections list.
        /// </summary>
        /// <param name="_timeList">A list of [DateTime] objects.</param>
        /// <param name="_readingsList">A list of [long] objects.</param>
        /// <param name="myGrid">The child [Grid] of [Chart].</param>
        /// <param name="_name">The name for the line.</param>
        /// <param name="_description">A description for the graph. The description is shown in the legend.</param>
        /// <param name="_color">A [Color] for the [LineGraph].</param>
        /// <param name="_strokeThickness">The thickness of the [LineGraph].</param>
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
        /// A function for generating data for all DataClass'.
        /// </summary>
        /// <param name="_timer">A [PeriodicTimer] that calls the function.</param>
        /// <param name="_chart">The [Chart] each [DataCollection] should be plotted on.</param>
        /// <param name="_userView">The input duration to show on the chart.</param>
        public async void GenerateData(PeriodicTimer? _timer, Chart _chart, TextBox _userView)
        {
            Random random = new();

            while (await _timer!.WaitForNextTickAsync())
            {
                if (!int.TryParse(_userView.Text.Split(' ')[0], out int userViewInt)) { userViewInt = 6; }

                long maxView = TimeSpan.TicksPerMinute * userViewInt;

                foreach (DataClass dataCollection in _dataCollections)
                {
                    /* Random data should be replaced with actual data */
                    dataCollection.Time.Add(DateTime.Now);
                    dataCollection.Readings.Add((long)random.Next(0, 100));

                    UpdateChart(dataCollection);

                    _chart.PlotHeight = 105;

                    if (_chart.IsMouseOver) continue;
                    _chart.PlotWidth = maxView;
                    _chart.PlotOriginX = AutoFitChart(userViewInt, maxView, dataCollection);
                }


            }
        }

        /// <summary>
        /// Sets the view span of the graph.
        /// </summary>
        /// <param name="_userViewInt"></param>
        /// <param name="_maxView"></param>
        /// <param name="_dataCollection"></param>
        /// <returns></returns>
        private static long AutoFitChart(int _userViewInt, long _maxView, DataClass _dataCollection)
        {
            return _dataCollection.Time.Last().Ticks - _dataCollection.Time.First().Ticks >= _maxView * 0.9 ? 
                DateTime.Now.AddMinutes(-(_userViewInt * 0.9)).Ticks : 
                _dataCollection.Time.First().Ticks;
        }

        /// <summary>
        /// Updates a graph, using a data collection.
        /// </summary>
        /// <param name="_dataCollection"></param>
        private static void UpdateChart(DataClass _dataCollection)
        {
            var valueY = _dataCollection.Time.Select(e => e.Ticks).ToList();

            _dataCollection.Line.Plot(valueY, _dataCollection.Readings);
        }
    }
}
