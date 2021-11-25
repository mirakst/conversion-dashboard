using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DashboardFrontend.ViewModels
{
    public class NetworkViewModel
    {
        public List<ObservableCollection<ObservablePoint>> NetworkData { get; private set; }
        public List<ObservableCollection<ObservablePoint>> NetworkSpeed { get; private set; }
        public List<ObservableCollection<ObservablePoint>> NetworkDelta { get; private set; }
        
        public List<ISeries> NetworkSeries { get; private set; }
        public List<ISeries> NetworkDeltaSeries { get; private set; }
        public List<ISeries> NetworkSpeedSeries { get; private set; }

        public List<Axis> XAxis { get; private set; }

        public List<Axis> YAxis { get; private set; }
        public List<Axis> YAxisDelta { get; private set; }
        public List<Axis> YAxisSpeed { get; private set; }

        private ObservableCollection<ObservablePoint> sendData = new();
        private ObservableCollection<ObservablePoint> sendSpeedData = new();
        private ObservableCollection<ObservablePoint> sendDeltaData = new();
        private ObservableCollection<ObservablePoint> recivedData = new();
        private ObservableCollection<ObservablePoint> recivedSpeedData = new();
        private ObservableCollection<ObservablePoint> recivedDeltaData = new();

        public NetworkViewModel()
        {

            NetworkData = new()
            {
                sendData,
                recivedData
            };
            NetworkDelta = new()
            {
                sendDeltaData,
                recivedDeltaData
            };

            NetworkSpeed = new()
            {
                sendSpeedData,
                recivedSpeedData
            };

            NetworkSeries = new()
            {
                new LineSeries<ObservablePoint>
                {
                    Name = "Send",
                    Stroke = new SolidColorPaint(new SKColor(92, 84, 219), 3),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(92, 84, 219)),
                    GeometryStroke = new SolidColorPaint(new SKColor(92, 84, 219)),
                    GeometrySize = 3,
                    TooltipLabelFormatter = e => NetworkSeries?.ElementAt(0).Name + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 e.PrimaryValue.ToString() + "GB",
                },
                new LineSeries<ObservablePoint>
                {
                    Name = "Recive",
                    Stroke = new SolidColorPaint(new SKColor(245, 88, 47), 3),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(245, 88, 47)),
                    GeometryStroke = new SolidColorPaint(new SKColor(245, 88, 47)),
                    GeometrySize = 3,
                    TooltipLabelFormatter = e => NetworkSeries?.ElementAt(1).Name + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 e.PrimaryValue.ToString() + "GB",
                }
            };

            NetworkDeltaSeries = new()
            {
                new LineSeries<ObservablePoint>
                {
                    Name = "Send delta",
                    Stroke = new SolidColorPaint(new SKColor(92, 84, 219), 3),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(92, 84, 219)),
                    GeometryStroke = new SolidColorPaint(new SKColor(92, 84, 219)),
                    GeometrySize = 3,
                    TooltipLabelFormatter = e => NetworkDeltaSeries?.ElementAt(0).Name + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 e.PrimaryValue.ToString() + "MB",
                },
                new LineSeries<ObservablePoint>
                {
                    Name = "Recive delta",
                    Stroke = new SolidColorPaint(new SKColor(245, 88, 47), 3),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(245, 88, 47)),
                    GeometryStroke = new SolidColorPaint(new SKColor(245, 88, 47)),
                    GeometrySize = 3,
                    TooltipLabelFormatter = e => NetworkDeltaSeries?.ElementAt(1).Name + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 e.PrimaryValue.ToString() + "MB",
                }
            };

            NetworkSpeedSeries = new()
            {
                new LineSeries<ObservablePoint>
                {
                    Name = "Send speed",
                    Stroke = new SolidColorPaint(new SKColor(245, 88, 47), 3),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(245, 88, 47)),
                    GeometryStroke = new SolidColorPaint(new SKColor(245, 88, 47)),
                    GeometrySize = 3,
                    TooltipLabelFormatter = e => NetworkSpeedSeries?.ElementAt(0).Name + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 e.PrimaryValue.ToString() + "KBps",
                },
                new LineSeries<ObservablePoint>
                {
                    Name = "Recive speed",
                    Stroke = new SolidColorPaint(new SKColor(92, 84, 219), 3),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(92, 84, 219)),
                    GeometryStroke = new SolidColorPaint(new SKColor(92, 84, 219)),
                    GeometrySize = 3,
                    TooltipLabelFormatter = e => NetworkSpeedSeries?.ElementAt(1).Name + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 e.PrimaryValue.ToString() + "KBps",
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
                    Name = "SendReceived",
                    Labeler  = (value) => value.ToString("N0") + "GB",
                    LabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                }
            };

            YAxisDelta = new()
            {
                new Axis
                {
                    Name = "Delta",
                    Labeler  = (value) => value.ToString("N0") + "MB",
                    LabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                }
            };

            YAxisSpeed = new()
            {
                new Axis
                {
                    Name = "Speed",
                    Labeler  = (value) => value.ToString("N2") + "KBps",
                    LabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                }
            };
        }
    }
}
