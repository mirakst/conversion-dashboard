using System;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DashboardFrontend.Charts
{
    /// <summary>
    /// Empty chart class to add manager data to.
    /// </summary>
    public class ManagerChart : BaseChart
    {
        public ManagerChart(string YAxisLabel)
        {
            Values = new();
            Series = new();
            XAxis = new()
            {
                new Axis
                {
                    Name = "Time",
                    Labeler = value => DateTime.FromOADate(value).ToString("mm:ss"),
                    MinLimit = 0,
                    LabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                    AnimationsSpeed = TimeSpan.FromMilliseconds(500),
                }
            };

            YAxis = new()
            {
                new Axis
                {
                    Name = $"{YAxisLabel}",
                    Labeler = (value) => value.ToString("P"),
                    MaxLimit = 1,
                    MinLimit = 0,
                    MinStep = 0.25,
                    ForceStepToMin = true,
                    LabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                    AnimationsSpeed = TimeSpan.FromMilliseconds(500),
                }
            };
        }
    }
}
