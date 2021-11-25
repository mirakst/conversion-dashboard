using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using DashboardFrontend.Annotations;
using Model;

namespace DashboardFrontend.ViewModels
{
    /// <summary>
    /// A class for the creating and controlling <see cref="ISeries"/>
    /// </summary>
    public class DataChart : BaseViewModel
    {
        #region public
        public List<ObservableCollection<ObservablePoint>> Values { get; private set; } = new();
        public List<ISeries> Series { get; private set; } = new();
        public List<Axis> XAxis { get; private set; } = new();
        public List<Axis> YAxis { get; private set; } = new();
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
        private DateTime _lastRamPlot { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
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
        private DateTime _lastCpuPlot { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
        #endregion

        #region private
        private PeriodicTimer? _queryTimer;
        private PeriodicTimer? _autoFocusTimer;
        private bool _isGraphRunning = false;
        private bool _isAutoFocusTimer = false;
        private int _maxView = 10;
        #endregion

        /// <summary>
        /// Creates a new <see cref="ISeries"/>, with given data and starts it with auto focus on.
        /// </summary>
        /// <param name="linesList">A list of <see cref="ISeries"/>.</param>
        /// <param name="dataList">A list of <see cref="ObservableCollection{ObservablePoint}"/>.</param>
        /// <param name="xAxisList">A list of <see cref="Axis"/> for the X axis.</param>
        /// <param name="yAxisList">A list of <see cref="Axis"/> for the Y axis.</param>
        public DataChart(BaseChart chart, Cpu cpu, Ram ram)
        {
            AddLine(chart.Series, chart.Data, chart.XAxis, chart.YAxis);
            AutoFocusOn();
        }

        /// <summary>
        /// Adds one or more new elements to the <see cref="ISeries"/> list.
        /// </summary>
        /// <param name="linesList">List of <see cref="ISeries"/>.</param>
        /// <param name="dataList">List of <see cref="ObservableCollection{ObservablePoint}"/></param>
        public void AddLine(List<ISeries> linesList, List<ObservableCollection<ObservablePoint>> dataList, List<Axis> xAxisList, List<Axis> yAxisList)
        {
            foreach (var collection in dataList)
            {
                Values.Add(collection);
            }

            for (int i = 0; i < linesList.Count; i++)
            {
                Series.Add(linesList[i]);
                Series[i].Values = Values[i];
            }

            foreach (var axis in xAxisList)
            {
                XAxis.Add(axis);
            }

            foreach (var axis in yAxisList)
            {
                YAxis.Add(axis);
            }
        }

        /// <summary>
        /// Removes the given element from the list of <see cref="ISeries"/>.
        /// </summary>
        /// <param name="lineToRemove">The <see cref="ISeries"/> number to remove</param>
        public void RemoveLine(int lineToRemove)
        {
            Series.RemoveAt(lineToRemove);
        }

        /// <summary>
        /// Stops the <see cref="List{ISeries}"/> from updating, if it is running.
        /// </summary>
        public void StopGraph()
        {
            if (_isGraphRunning)
            {
                _queryTimer.Dispose();
                _isGraphRunning = false;
            }
        }

        /// <summary>
        /// Adds points to the chart.
        /// </summary>
        public void UpdateData(Ram? ram, Cpu? cpu)
        {
            if (ram is null || cpu is null) return;
            LastRamReading = ram.Readings.Last().Load * 100;
            LastCpuReading = cpu.Readings.Last().Load * 100;
            foreach (var item in ram.Readings.Where(e => e.Date > _lastRamPlot))
            {
                Values[0].Add(CreatePoint(item));       
            }

            foreach (var item in cpu.Readings.Where(e => e.Date > _lastCpuPlot))
            {
                Values[1].Add(CreatePoint(item));
            }
            UpdatePlots(ram.Readings.Last().Date, cpu.Readings.Last().Date);
        }

        private void UpdatePlots(DateTime ramDate, DateTime cpuDate)
        {
            _lastRamPlot = ramDate;
            _lastCpuPlot = cpuDate;
        }

        /// <summary>
        /// Creates a point from a performance metric.
        /// </summary>
        private ObservablePoint CreatePoint(PerformanceMetric pointData)
        {
            return new ObservablePoint(pointData.Date.ToOADate(), pointData.Load);
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
                    if (Values.Count > 0 && Values.First().Count > 0)
                    {
                        XAxis[0].MinLimit = Values.First().Count >= _maxView ? Values.First().Last().X.Value - DateTime.FromBinary(TimeSpan.FromMinutes(_maxView).Ticks).ToOADate() :
                                                                              Values.First().First().X.Value;
                        XAxis[0].MaxLimit = Values.First().Last().X.Value;
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

            foreach (LineSeries<ObservablePoint> line in Series)
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

            foreach (LineSeries<ObservablePoint> line in Series)
            {
                line.GeometrySize = input;
            }
        }
/*
        /// <summary>
        /// Change how often the query should run.
        /// </summary>
        /// <param name="input">The number of minutes between queryes.</param>
        public void ChangeQueryTimer(int input)
        {
            _queryTimerInterval = input;
            StopGraph();
        }*/
        #endregion
    }
}
