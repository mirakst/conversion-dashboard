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
    public class ManagerChartViewModel : BaseViewModel
    {

        #region Chart objects
        public ManagerChartWrapper CPUChart { get; private set; } = new(new ManagerChartTemplate("load"), false);
        public ManagerChartWrapper RAMChart { get; private set; } = new(new ManagerChartTemplate("load"), false);
        public List<ManagerChartWrapper> Charts { get; private set; } = new();
        public List<double> FurthestPoints { get; private set; } = new();
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
            if (wrapper.Manager.CpuReadings.Count < 2) return;
            DateTime firstCpu = wrapper.Manager.CpuReadings.First().Date;
            wrapper.ManagerValues[0] = new(wrapper.Manager.CpuReadings.Select(p => CreatePoint(p, firstCpu)));
            FurthestPoints.Add(wrapper.Manager.CpuReadings.Last().Date.ToOADate() - firstCpu.ToOADate());
            DateTime firstRam = wrapper.Manager.RamReadings.First().Date;
            wrapper.ManagerValues[1] = new(wrapper.Manager.RamReadings.Select(p => CreatePoint(p, firstRam)));

            int index = 0;
            foreach (ManagerChartWrapper chart in Charts)
            {
                chart.AddLine(new LineSeries<ObservablePoint>
                {
                    Name = $"{wrapper.Manager.ShortName}",
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColor.Parse(wrapper.LineColor.Color.ToString()), 3),
                    GeometryFill = new SolidColorPaint(SKColor.Parse(wrapper.LineColor.Color.ToString())),
                    GeometryStroke = new SolidColorPaint(SKColor.Parse(wrapper.LineColor.Color.ToString())),
                    GeometrySize = 0.4,
                    LineSmoothness = 0.5,
                    AnimationsSpeed = TimeSpan.FromMilliseconds(200),
                }, wrapper.ManagerValues[index++]);
            }
            UpdateView();
        }
        
        /// <summary>
        /// Creates a plot point from performance point data, with an offset date.
        /// </summary>
        /// <param name="pointData">The data to create the point from.</param>
        /// <param name="first">The time of the first performance reading during the manager's runtime.</param>
        /// <returns>A new point with an offset date and the load value.</returns>
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
            if (manager.Manager.CpuReadings.Count < 2) return;
            for (int i = 0; i < Charts.Count; i++)
            {
                Charts[i].RemoveData(manager.Manager.Name, manager.ManagerValues[i]);
            }
            DateTime first = manager.Manager.CpuReadings.First().Date;
            double offsetDate = manager.Manager.CpuReadings.Last().Date.ToOADate() - first.ToOADate();
            FurthestPoints.Remove(offsetDate);
            UpdateView();
        }

        /// <summary>
        /// Clears the charts of all managers.
        /// </summary>
        public void ClearChartLinesHelper()
        {
            foreach (ManagerChartWrapper chart in Charts)
            {
                chart.Chart.Series.Clear();
            }
            FurthestPoints = new();
            UpdateView();
        }

        /// <summary>
        /// Updates the view of the chart.
        /// </summary>
        private void UpdateView()
        {
            double maxLimit = 0;
            if (FurthestPoints.Any())
            {
                maxLimit = FurthestPoints.Max();
            }
            CPUChart.Chart.XAxis[0].MaxLimit = maxLimit;
            RAMChart.Chart.XAxis[0].MaxLimit = maxLimit;
        }
    }
}
