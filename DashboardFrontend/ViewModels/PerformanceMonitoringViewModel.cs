using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;


namespace DashboardFrontend.ViewModels
{
    public class PerformanceMonitoringViewModel
    {
        public IEnumerable<ISeries> Performance { get; set; }

        public List<Axis> XAxes { get; set; }
        public List<Axis> YAxes { get; set; }
        public int MaxView { get; set; } = 10;

        private double LineSmoothness = 1;
        private int LinePointSize = 10;
        private readonly Random _random = new Random();
        private readonly ObservableCollection<DateTimePoint> RAMValues = new();
        private readonly ObservableCollection<DateTimePoint> CPUValues = new();
        private PeriodicTimer _autoFocusTimer;
        private bool _isAutoFocusTimer = false;

        public PerformanceMonitoringViewModel()
        {
            Performance = new ObservableCollection<ISeries>()
            {
                new LineSeries<DateTimePoint>
                {
                    Name = "RAM",
                    Values = RAMValues,
                    LineSmoothness = LineSmoothness,
                    Fill = null,
                    Stroke = new SolidColorPaint(new SKColor(133, 222, 118)),
                    GeometryStroke = new SolidColorPaint(new SKColor(133, 222, 118)),
                    GeometryFill = new SolidColorPaint(new SKColor(133, 222, 118)),
                    GeometrySize = LinePointSize,
                    
                },
                new LineSeries<DateTimePoint>
                {
                    Name = "CPU",
                    Values = CPUValues,
                    LineSmoothness =LineSmoothness,
                    Fill = null,
                    Stroke = new SolidColorPaint(new SKColor(245, 88, 47), 2),
                    GeometryStroke = new SolidColorPaint(new SKColor(245, 88, 47), 2),
                    GeometryFill = new SolidColorPaint(new SKColor(245, 88, 47), 2),
                    GeometrySize = LinePointSize,
                }
            };

            XAxes = new()
            {
                new Axis
                {
                    Name = "Time",
                    Labeler = value => new DateTime((long)value).ToString("HH:mm:ss"),
                }
            };

            YAxes = new()
            {
                new Axis
                {
                    Name = "Load",
                    Labeler  = (value) => value.ToString("P"),
                    MaxLimit = 1,
                    MinLimit = 0,                    
                }
            };
        }

        public void AutoFocusOff()
        {
            if (_isAutoFocusTimer) { _autoFocusTimer.Dispose(); }
            _isAutoFocusTimer = false;
        }

        public async void AutoFocusOn()
        {
            if (!_isAutoFocusTimer)
            {
                _autoFocusTimer = new(TimeSpan.FromSeconds(1));
                _isAutoFocusTimer = true;

                while (await _autoFocusTimer.WaitForNextTickAsync())
                {
                    if (RAMValues.Count > 0)
                    {
                        XAxes[0].MinLimit = RAMValues.Count >= MaxView ? RAMValues.ElementAt(RAMValues.Count - MaxView).DateTime.Ticks : RAMValues.First().DateTime.Ticks;
                        XAxes[0].MaxLimit = RAMValues.Last().DateTime.Ticks;
                    }
                }
            }
        }

        //Skal ikke bruges
        public async void StartPerformanceGraph(PeriodicTimer _performanceTimer)
        {
            while (await _performanceTimer.WaitForNextTickAsync())
            {
                if (RAMValues.Count >= 20)
                {
                    //RemoveItem();
                }

                AddItem();
                
            }
        }

        //Skal ikke bruges
        private void AddItem()
        {
            var randomValue = _random.NextDouble();
            var randomValueTwo = _random.NextDouble();

            RAMValues.Add(
                new DateTimePoint(DateTime.Now, randomValue));
            
            CPUValues.Add(
                new DateTimePoint(DateTime.Now, randomValueTwo));
        }

        //Skal ikke bruges
        private void RemoveItem()
        {
            RAMValues.RemoveAt(0);
        }

        //Functions for settings ---------------------

        /// <summary>
        /// Sets the smoothness of curves on all lines.
        /// </summary>
        /// <param name="_input">A number between 0 and 1. Standart is 1.</param>
        public void ChangeLineSmoothness(double _input)
        {
            if (_input > 1) { return; }
            else if (_input < 0) { return;}

            foreach (LineSeries<DateTimePoint> line in Performance)
            {
                line.LineSmoothness = _input;
            }
        }
        
        /// <summary>
        /// Sets the size of line points on all lines.
        /// </summary>
        /// <param name="_input">A number between 0 and 60. less is smaller. Standart is 10.</param>
        public void ChangePointSize(int _input)
        {
            if (_input > 60) { return; }
            else if (_input < 0) { return; }

            foreach (LineSeries<DateTimePoint> line in Performance)
            {
                line.GeometrySize = _input;
            }
        }

        //public void ChangeLineColor(int _line, byte _r, byte _g, byte _b)
        //{
        //    .Stroke = new SolidColorPaint(new SKColor(_r, _g, _b));
        //}
    }
}
