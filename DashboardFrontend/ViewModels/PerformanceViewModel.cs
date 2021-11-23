using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries.Segments;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;


namespace DashboardFrontend.ViewModels
{
    /// <summary>
    /// The ViewModel class for Performance monitoring.
    /// </summary>
    public class PerformanceViewModel
    {
        public List<ObservableCollection<ObservablePoint>> PerformanceData { get; set; }
        public ObservableCollection<ObservablePoint> RAMValues { get; private set; } = new();
        public ObservableCollection<ObservablePoint> CPUValues { get; private set; } = new();
        public List<ISeries> Series { get; private set; }
        public LiveChartViewModel LineChart { get; private set; }

        public List<Axis> XAxes { get; private set; }
        public List<Axis> YAxes { get; private set; }
        
        public PerformanceViewModel()
        {
            LineChart = new();

            PerformanceData = new()
            {
                RAMValues,
                CPUValues,
            };

            Series = new()
            {
                new LineSeries<ObservablePoint>
                {
                    Name = "RAM",
                    Stroke = new SolidColorPaint(new SKColor(92, 84, 219), 3),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(92, 84, 219)),
                    GeometryStroke = new SolidColorPaint(new SKColor(92, 84, 219)),
                    GeometrySize = 3,
                    TooltipLabelFormatter = e => Series?.ElementAt(0).Name + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 e.PrimaryValue.ToString("P"),
                },
                new LineSeries<ObservablePoint> 
                {
                    Name = "CPU",
                    Stroke = new SolidColorPaint(new SKColor(245, 88, 47), 3),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(245, 88, 47)),
                    GeometryStroke = new SolidColorPaint(new SKColor(245, 88, 47)),
                    GeometrySize = 3,
                    TooltipLabelFormatter = e => Series?.ElementAt(1).Name + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 e.PrimaryValue.ToString("P"),
                }
            };

            XAxes = new()
            {
                new Axis
                {
                    Name = "Time",
                    Labeler = value => DateTime.FromOADate(value).ToString("HH:mm:ss"),
                    MinLimit = DateTime.Now.ToOADate(),
                    MaxLimit = DateTime.Now.ToOADate(),
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
    }
}
