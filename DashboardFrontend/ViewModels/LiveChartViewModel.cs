using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace DashboardFrontend.ViewModels
{
    /// <summary>
    /// A class for the creating and controlling charts
    /// </summary>
    public class LiveChartViewModel
    {
        #region public
        public List<ObservableCollection<ObservablePoint>> Values { get; private set; } = new();
        public List<ISeries> Series { get; private set; } = new();
        public List<Axis> XAxis { get; private set; } = new();
        public List<Axis> YAxis { get; private set; } = new();
        #endregion

        #region private
        private PeriodicTimer? queryTimer;
        private int queryTimerInterval = 2;
        private PeriodicTimer? autoFocusTimer;
        private readonly Random random = new();
        private bool isGraphRunning = false;
        private bool isAutoFocusTimer = false;
        private int maxView = 10;
        #endregion

        /// <summary>
        /// Creates a new Chart, with given data and starts it with auto focus on.
        /// </summary>
        /// <param name="charts">A list of <see cref="ISeries"/>.</param>
        /// <param name="data">A list of <see cref="ObservableCollection{ObservablePoint}"/>.</param>
        /// <param name="xAxis">A list of <see cref="Axis"/> for the X axis.</param>
        /// <param name="yAxis">A list of <see cref="Axis"/> for the Y axis.</param>
        public LiveChartViewModel(List<ISeries> charts, List<ObservableCollection<ObservablePoint>> data, List<Axis> xAxis, List<Axis> yAxis)
        {
            AddChart(charts, data, xAxis, yAxis);
            AutoFocusOn();
            StartGraph();
        }

        /// <summary>
        /// Add a new chart to the Graph.
        /// </summary>
        /// <param name="charts">List of charts.</param>
        /// <param name="data">List of data</param>
        public void AddChart(List<ISeries> charts, List<ObservableCollection<ObservablePoint>> data, List<Axis> xAxis, List<Axis> yAxis)
        {
            foreach (var collection in data)
            {
                Values.Add(collection);
            }

            for (int i = 0; i < charts.Count; i++)
            {
                Series.Add(charts[i]);
                Series[i].Values = Values[i];
            }

            foreach (var axis in xAxis)
            {
                XAxis.Add(axis);
            }

            foreach (var axis in yAxis)
            {
                YAxis.Add(axis);
            }
        }

        /// <summary>
        /// Removes the chart from the list of charts.
        /// </summary>
        /// <param name="chartToRemove">The chart number to remove</param>
        public void RemoveChart(int chartToRemove)
        {
            Series.RemoveAt(chartToRemove);
        }

        /// <summary>
        /// Stops the graph, if it is running.
        /// </summary>
        public void StopGraph()
        {
            if (isGraphRunning)
            {
                queryTimer.Dispose();
                isGraphRunning = false;
            }
        }

        /// <summary>
        /// Calls <see cref="QueryList"/> at a set interval.
        /// </summary>
        /// <param name="querryTimer"></param>
        public async void StartGraph()
        {
            if (!isGraphRunning) 
            {
                isGraphRunning = true;
                queryTimer = new(TimeSpan.FromSeconds(queryTimerInterval));

                while (await queryTimer.WaitForNextTickAsync())
                {
                    QueryList();
                }
            }
        }

        /// <summary>
        /// Function qurying data (generates data currently).
        /// </summary>
        private void QueryList()
        {
            foreach (var item in Values)
            {
                item.Add(new ObservablePoint(DateTime.Now.ToOADate(), random.NextDouble()));
            }
        }

        #region AutoFocus Functions
        /// <summary>
        /// Disables AutoFocusOn().
        /// </summary>
        public void AutoFocusOff()
        {
            autoFocusTimer?.Dispose();
            isAutoFocusTimer = false;
        }

        /// <summary>
        /// Aync function that focus the chart on the most resent entries.
        /// </summary>
        public async void AutoFocusOn()
        {
            if (!isAutoFocusTimer)
            {
                autoFocusTimer = new(TimeSpan.FromMilliseconds(100));
                isAutoFocusTimer = true;

                while (await autoFocusTimer.WaitForNextTickAsync())
                {
                    if (Values.Count > 0 && Values.First().Count > 0)
                    {
                        XAxis[0].MinLimit = Values.First().Count >= maxView ? Values.First().Last().X.Value - DateTime.FromBinary(TimeSpan.FromSeconds(maxView).Ticks).ToOADate() :
                                                                              Values.First().First().X.Value; /* skal ændres fra .FromSeconds til .FromMinutes*/
                        XAxis[0].MaxLimit = Values.First().Last().X.Value;
                    }
                }
            }
        }
        #endregion

        #region Settings Functions
        /// <summary>
        /// Change max viewable timespan on the graph while auto focusing.
        /// </summary>
        /// <param name="input"></param>
        public void ChangeMaxView(int input)
        {
            maxView = input;
        }

        /// <summary>
        /// Sets the smoothness of curves on all lines.
        /// </summary>
        /// <param name = "input">A number between 0 and 1. Standart is 1.</param>
        public void ChangeLineSmoothness(double input)
        {
            if (input > 1) { return; }
            else if (input < 0) { return; }

            foreach (LineSeries<ObservablePoint> line in Series)
            {
                line.LineSmoothness = input;
            }
        }

        /// <summary>
        /// Sets the size of line points on all lines.
        /// </summary>
        /// <param name="input">A number between 0 and 60. less is smaller. Standart is 10.</param>
        public void ChangePointSize(int input)
        {
            if (input > 60) { return; }
            else if (input < 0) { return; }

            foreach (LineSeries<ObservablePoint> line in Series)
            {
                line.GeometrySize = input;
            }
        }

        /// <summary>
        /// Change how often the query should run.
        /// </summary>
        /// <param name="input">The number of minutes between queryes.</param>
        public void ChangeQueryTimer(int input)
        {
            queryTimerInterval = input;
            StopGraph();
            StartGraph();
        }
        #endregion
    }
}
