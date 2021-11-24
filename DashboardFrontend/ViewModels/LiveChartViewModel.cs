using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DashboardFrontend.ViewModels
{
    /// <summary>
    /// TODO
    /// </summary>
    public class LiveChartViewModel
    {
        public List<ObservableCollection<ObservablePoint>> Values { get; private set; }
        public List<ISeries> Series { get; private set; }
        public List<Axis> XAxis { get; private set; }
        public List<Axis> YAxis { get; private set; }

        private PeriodicTimer? autoFocusTimer;
        private readonly Random random = new();
        private bool isAutoFocusTimer = false;
        private int maxView = 10;

        public LiveChartViewModel()
        {
            XAxis = new();
            YAxis = new();
            Series = new();
            Values = new();

            AutoFocusOn();
        }

        /// <summary>
        /// Add a new chart to the Graph.
        /// </summary>
        /// <param name="Charts">List of charts.</param>
        /// <param name="Data">List of data</param>
        public void NewChart(List<ISeries> Charts, List<ObservableCollection<ObservablePoint>> Data, List<Axis> xAxis, List<Axis> yAxis)
        {
            foreach (var collection in Data)
            {
                Values.Add(collection);
            }

            for (int i = 0; i < Charts.Count; i++)
            {
                Series.Add(Charts[i]);
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

        public void AddData(ISeries Line, ObservableCollection<ObservablePoint> data)
        {
            Values.Add(data);

            Series.Add(Line);
            Series[Series.IndexOf(Line)].Values = Values[Series.IndexOf(Line)];
        }

        public void RemoveData(String managername, ObservableCollection<ObservablePoint> data)
        {
            Values.Remove(data);

            int managerIndex = Series.FindIndex(e => e.Name == managername);
            Series.RemoveAt(managerIndex);
        }

        /// <summary>
        /// Calls <see cref="QuerryList"/> at a set interval.
        /// </summary>
        /// <param name="querryTimer"></param>
        public async void StartGraph(PeriodicTimer querryTimer)
        {
            while (await querryTimer.WaitForNextTickAsync())
            {
                QuerryList();
            }
        }

        /// <summary>
        /// Function der skal querry for data.
        /// </summary>
        private void QuerryList()
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
                        XAxis[0].MinLimit = Values.First().Count >= maxView ? Values.First().ElementAt(Values.First().Count - maxView).X.Value : 
                                                                              Values.First().First().X.Value;
                        XAxis[0].MaxLimit = Values.First().Last().X.Value;
                    }
                }
            }
        }
        #endregion

        #region Settings Functions

        /// <summary>
        /// Change max view.
        /// </summary>
        /// <param name="i"></param>
        public void ChangeMaxView(int i)
        {
            maxView = i;
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
        #endregion
    }
}
