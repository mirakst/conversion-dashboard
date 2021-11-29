using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DashboardFrontend.Charts
{
    /// <summary>
    /// The ViewModel class for Performance monitoring.
    /// </summary>
    public class PerformanceChart : BaseChart
    {
        public ObservableCollection<ObservablePoint> RamValues
        { get; private set; } = new();
        public ObservableCollection<ObservablePoint> CpuValues { get; private set; } = new();
        public PerformanceChart()
        {
            Type = ChartType.Performance;

            Values = new()
            {
                RamValues,
                CpuValues,
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
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss", new CultureInfo("da-DK")) + "\n" +
                                                 e.PrimaryValue.ToString("P"),
                    LineSmoothness = 0,
                    Values=RamValues,
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
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss", new CultureInfo("da-DK")) + "\n" +
                                                 e.PrimaryValue.ToString("P"),
                    LineSmoothness = 0,
                    Values = CpuValues,
                }
            };

            XAxis = new()
            {
                new Axis
                {
                    Name = "Time",
                    Labeler = value => DateTime.FromOADate(value).ToString("HH:mm", new CultureInfo("da-DK")),
                    MinLimit = DateTime.Now.ToOADate(),
                    MaxLimit = DateTime.Now.ToOADate(),
                    LabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                }
            };

            YAxis = new()
            {
                new Axis
                {
                    Name = "Load",
                    Labeler  = (value) => value.ToString("P0"),
                    MaxLimit = 1,
                    MinLimit = 0,
                    LabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                    MinStep = 0.25,
                    ForceStepToMin = true,
                }
            };
        }
    }
}
