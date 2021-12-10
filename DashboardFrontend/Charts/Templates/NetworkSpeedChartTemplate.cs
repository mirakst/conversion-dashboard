using System;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DashboardFrontend.Charts
{
    public class NetworkSpeedChartTemplate : ChartTemplate
    {
        public ObservableCollection<ObservablePoint> SendSpeedValues { get; private set; } = new();
        public ObservableCollection<ObservablePoint> ReceiveSpeedValues { get; private set; } = new();

        public NetworkSpeedChartTemplate()
        {
            Type = ChartType.NetworkSpeed;

            Values = new()
            {
                SendSpeedValues,
                ReceiveSpeedValues,
            };

            Series = new()
            {
                new LineSeries<ObservablePoint>
                {
                    Name = "Send speed",
                    Stroke = new SolidColorPaint(new SKColor(245, 88, 47), 3),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(245, 88, 47)),
                    GeometryStroke = new SolidColorPaint(new SKColor(245, 88, 47)),
                    GeometrySize = 3,
                    TooltipLabelFormatter = e => Series?.ElementAt(0).Name + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 Math.Round(e.PrimaryValue, 2) + "Mbps",
                    Values=SendSpeedValues,
                },
                new LineSeries<ObservablePoint>
                {
                    Name = "Receive speed",
                    Stroke = new SolidColorPaint(new SKColor(92, 84, 219), 3),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(92, 84, 219)),
                    GeometryStroke = new SolidColorPaint(new SKColor(92, 84, 219)),
                    GeometrySize = 3,
                    TooltipLabelFormatter = e => Series?.ElementAt(1).Name + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 Math.Round(e.PrimaryValue, 2) + "Mbps",
                    Values = ReceiveSpeedValues,
                }
            };

            XAxis = new()
            {
                new Axis
                {
                    Name = "Time",
                    Labeler = value => DateTime.FromOADate(value).ToString("HH:mm"),
                    MinLimit = DateTime.Now.ToOADate(),
                    MaxLimit = DateTime.Now.ToOADate(),
                    LabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                }
            };
            
            YAxis= new()
            {
                new Axis
                {
                    Name = "Speed",
                    Labeler = (value) => value.ToString("N0") + "Mbps",
                    LabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                    Padding = new Padding(0),
                    NamePadding = new Padding(0),
                }
            };
        }
    }
}
