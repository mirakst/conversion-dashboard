using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DashboardFrontend.Charts
{
    public class NetworkDeltaChart : BaseChart
    {
        public ObservableCollection<ObservablePoint> SendDeltaValues { get; private set; } = new();
        public ObservableCollection<ObservablePoint> ReceiveDeltaValues { get; private set; } = new();

        public NetworkDeltaChart()
        {
            Type = ChartType.Network;

            Values = new()
            {
                SendDeltaValues,
                ReceiveDeltaValues,
            };

            Series = new()
            {
                new LineSeries<ObservablePoint>
                {
                    Name = "Send delta",
                    Stroke = new SolidColorPaint(new SKColor(92, 84, 219), 3),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(92, 84, 219)),
                    GeometryStroke = new SolidColorPaint(new SKColor(92, 84, 219)),
                    GeometrySize = 3,
                    TooltipLabelFormatter = e => Series?.ElementAt(0).Name + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 e.PrimaryValue.ToString() + "MB",
                    Values=SendDeltaValues,
                },
                new LineSeries<ObservablePoint>
                {
                    Name = "Receive delta",
                    Stroke = new SolidColorPaint(new SKColor(245, 88, 47), 3),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(245, 88, 47)),
                    GeometryStroke = new SolidColorPaint(new SKColor(245, 88, 47)),
                    GeometrySize = 3,
                    TooltipLabelFormatter = e => Series?.ElementAt(1).Name + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 e.PrimaryValue.ToString() + "MB",
                    Values=ReceiveDeltaValues,
                }
            };

            

            XAxis = new()
            {
                new Axis
                {
                    Name = "Time",
                    Labeler = value => DateTime.FromOADate(value).ToString("HH:mm:ss"),
                    MinLimit = DateTime.Now.ToOADate(),
                    MaxLimit = DateTime.Now.ToOADate(),
                    LabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                }
            };

            YAxis = new()
            {
                new Axis
                {
                    Name = "Delta",
                    Labeler = (value) => value.ToString("N0") + "MB",
                    LabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                }
            };
        }
    }
}
