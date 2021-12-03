using DashboardFrontend.Charts;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Model;

namespace DashboardFrontend.ViewModels
{
    public class ManagerChartViewModel
    {

        #region Chart objects

        public DataChart CPUChart { get; private set; } = new(new ManagerChart("load"), false);
        public DataChart RAMChart { get; private set; } = new(new ManagerChart("load"), false);
        public List<DataChart> Charts { get; private set; } = new();
        #endregion

        public ManagerChartViewModel()
        {
            Charts.Add(CPUChart);
            Charts.Add(RAMChart);
        }

        /// <summary>
        /// Adds a line to all manager charts based on the data stored in the manager.
        /// </summary>
        /// The manager to be added to the chart <param name="wrapper"></param>
        public void AddChartLinesHelper(ManagerWrapper wrapper)
        {
            if (wrapper.Manager.CpuReadings.FirstOrDefault()?.Date is DateTime firstCpu)
            {
                wrapper.ManagerValues[0] = new(wrapper.Manager.CpuReadings.Select(p => CreatePoint(p, firstCpu)));
            }
            if (wrapper.Manager.RamReadings.FirstOrDefault()?.Date is DateTime firstRam)
            {
                wrapper.ManagerValues[1] = new(wrapper.Manager.RamReadings.Select(p => CreatePoint(p, firstRam)));
            }

            int index = 0;
            foreach (DataChart chart in Charts)
            {
                chart.AddLine(new LineSeries<ObservablePoint>
                {
                    Name = $"{wrapper.Manager.Name.Split(".").Last()}",
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColor.Parse(wrapper.LineColor.Color.ToString()), 3),
                    GeometryFill = new SolidColorPaint(SKColor.Parse(wrapper.LineColor.Color.ToString())),
                    GeometryStroke = new SolidColorPaint(SKColor.Parse(wrapper.LineColor.Color.ToString())),
                    GeometrySize = 0.4,
                    TooltipLabelFormatter = e => $"{wrapper.Manager.Name.Split(".").Last()}\nID {wrapper.Manager.ContextId}\n{DateTime.FromOADate(e.SecondaryValue):HH:mm:ss}\n{e.PrimaryValue:P}"
                }, wrapper.ManagerValues[index++]);
            }
        }

        private ObservablePoint CreatePoint(PerformanceMetric pointData, DateTime first)
        {
            double offsetDate = pointData.Date.ToOADate() - first.ToOADate();
            return new ObservablePoint(offsetDate, pointData.Load);
        }

        /// <summary>
        /// Removes a manager from the charts.
        /// </summary>
        /// The manager to be removed <param name="manager"></param>
        public void RemoveChartLinesHelper(ManagerWrapper manager)
        {
            for (int i = 0; i < Charts.Count; i++)
            {
                Charts[i].RemoveData(manager.Manager.Name, manager.ManagerValues[i]);
            }
        }

        /// <summary>
        /// Clears the charts of all managers.
        /// </summary>
        public void ClearChartLinesHelper()
        {
            foreach (DataChart chart in Charts)
            {
                chart.ChartData.Series.Clear();
            }
        }
    }
}
