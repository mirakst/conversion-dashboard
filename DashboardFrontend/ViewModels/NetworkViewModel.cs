using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DashboardFrontend.ViewModels
{
    public class NetworkViewModel
    {
        public List<ObservableCollection<ObservablePoint>> NetworkData { get; set; }
        public ObservableCollection<ObservablePoint> SendValues { get; private set; } = new();
        public ObservableCollection<ObservablePoint> RecivedValues { get; private set; } = new();
        public List<ISeries> Series { get; private set; }

        public List<Axis> XAxis { get; private set; }
        public List<Axis> YAxis { get; private set; }

        public NetworkViewModel()
        {

            NetworkData = new()
            {
                SendValues,
                RecivedValues,
            };

            Series = new()
            {
                new LineSeries<ObservablePoint>
                {
                    Name = "Send",
                    Stroke = new SolidColorPaint(new SKColor(92, 84, 219), 3),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(92, 84, 219)),
                    GeometryStroke = new SolidColorPaint(new SKColor(92, 84, 219)),
                    GeometrySize = 3,
                    TooltipLabelFormatter = e => Series?.ElementAt(0).Name + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 (e.PrimaryValue * 1000).ToString() + "MB",
                },
                new LineSeries<ObservablePoint>
                {
                    Name = "Recive",
                    Stroke = new SolidColorPaint(new SKColor(245, 88, 47), 3),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(245, 88, 47)),
                    GeometryStroke = new SolidColorPaint(new SKColor(245, 88, 47)),
                    GeometrySize = 3,
                    TooltipLabelFormatter = e => Series?.ElementAt(1).Name + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 e.PrimaryValue.ToString() + "MB",
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
                    Name = "Mega Bytes",
                    Labeler  = (value) => (value * 1000).ToString("N0") + "MB",
                    LabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                }
            };
        }
    }
}
