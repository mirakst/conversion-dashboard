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

        public long MaxView { get; set; } = TimeSpan.TicksPerMinute * 1;

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

        public async void UpdatePerformanceChart(PeriodicTimer timer, Chart myChart)
        {
            while (await timer.WaitForNextTickAsync())
            {
                if (CpuLoad.Count > 0 && RamUsage.Count > 0 && lineGraphList.Count > 0)
                {
                    var _valueXCpu = CpuLoad.Select(e => e.Item1.Ticks).ToList();
                    var _valueYCpu = CpuLoad.Select(e => e.Item2).ToList();

                    var _valueXRam = RamUsage.Select(e => e.Item1.Ticks).ToList();
                    var _valueYRam = RamUsage.Select(e => e.Item2).ToList();    

                    TrimDataList();
                    myChart.PlotOriginX = _valueXCpu.ElementAt(0);
                    myChart.PlotWidth = MaxView;

                    Trace.WriteLine(_valueXCpu.Count);
                    lineGraphList.ElementAt(0).Plot( _valueXCpu, _valueYCpu);
                    lineGraphList.ElementAt(1).Plot(_valueXRam, _valueYRam);
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
            while ((CpuLoad.Last().Item1.Ticks - CpuLoad.ElementAt(0).Item1.Ticks) > MaxView)
            {
                CpuLoad.RemoveAt(0);
            }
            while ((RamUsage.Last().Item1.Ticks - RamUsage.ElementAt(0).Item1.Ticks) > MaxView)
            {
                RamUsage.RemoveAt(0);
            }
        }
    }
}
