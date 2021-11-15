using InteractiveDataDisplay.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace DashboardFrontend
{
    public class HealthReportMonitoring
    {
        public List<Tuple<DateTime, long>> CpuLoad = new();
        public List<Tuple<DateTime, long>> RamUsage = new();
        private List<LineGraph> lineGraphList = new();

        public int MaxView { get; set; } = 100;

        public void AddLineGraph(Grid myGrid, string _name, string _description, Color _color, int _strokeThickness)
        {
            LineGraph line = new()
            {
                Name = _name,
                Description = _description,
                Stroke = new SolidColorBrush(_color),
                StrokeThickness = _strokeThickness
            };

            myGrid.Children.Add(line);
            lineGraphList.Add(line);
            Trace.WriteLine(lineGraphList.Count);
        }

        public async void UpdatePerformanceChart(PeriodicTimer timer)
        {
            List<double> xAxisFormatting = new();

            while (await timer.WaitForNextTickAsync())
            {
                if (CpuLoad.Count > 0 && RamUsage.Count > 0 && lineGraphList.Count > 0)
                {
                    var _valueXCpu = CpuLoad.Select(e => e.Item1.).ToList();
                    var _valueYCpu = CpuLoad.Select(e => e.Item2).ToList();

                    var _valueXRam = CpuLoad.Select(e => e.Item1).ToList();
                    var _valueYRam = CpuLoad.Select(e => e.Item2).ToList();

                    //TrimDataList();

                    //foreach (DateTime time in _valueXCpu)
                    //{
                    //    xAxisFormatting.Add(time.ToOADate());
                    //    Trace.WriteLine((DateTime.Now.Hour * 100) + DateTime.Now.Minute);
                    //}

                    lineGraphList.ElementAt(0).Plot(_valueXCpu, _valueYCpu);
                    //lineGraphList.ElementAt(1).Plot(xAxisFormatting, _valueYRam);
                }
            }
        }

        public async void GenerateData(PeriodicTimer _timer, Chart _chart)
        {
            Random random = new();
            while (await _timer.WaitForNextTickAsync())
            {
                _chart.PlotHeight = 100;

                CpuLoad.Add(Tuple.Create(DateTime.Now, (long)random.Next(0, 100)));
                RamUsage.Add(Tuple.Create(DateTime.Now, (long)random.Next(0, 100)));
            }
        }

        private void TrimDataList()
        {
            while (CpuLoad.Count >= MaxView)
            {
                CpuLoad.RemoveAt(0);
            }
            while (RamUsage.Count >= MaxView)
            {
                RamUsage.RemoveAt(0);
            }
        }
    }
}
