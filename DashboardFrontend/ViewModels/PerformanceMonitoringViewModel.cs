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

        private readonly Random _random = new Random();
        private readonly ObservableCollection<DateTimePoint> RAMValues = new();
        private readonly ObservableCollection<DateTimePoint> CPUValues = new();

        public PerformanceMonitoringViewModel()
        {
            Performance = new ObservableCollection<ISeries>()
            {
                new LineSeries<DateTimePoint>
                {
                    Name = "RAM",
                    Values = RAMValues,
                    Fill = null,
                    Stroke = new SolidColorPaint(new SKColor(133, 222, 118), 2),
                    GeometryStroke = new SolidColorPaint(new SKColor(133, 222, 118), 2),
                    GeometryFill = new SolidColorPaint(new SKColor(133, 222, 118), 2),
                    GeometrySize = 10,
                    
                },
                new LineSeries<DateTimePoint>
                {
                    Name = "CPU",
                    Values = CPUValues,
                    Fill = null,
                    Stroke = new SolidColorPaint(new SKColor(245, 88, 47), 2),
                    GeometryStroke = new SolidColorPaint(new SKColor(245, 88, 47), 2),
                    GeometryFill = new SolidColorPaint(new SKColor(245, 88, 47), 2),
                    GeometrySize = 10,
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
    }
}
