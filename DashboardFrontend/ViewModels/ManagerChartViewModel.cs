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
    public class ManagerChartViewModel
    {
        #region Performance objects
        public ManagerPerformanceViewModel CPUPerformance { get; private set; } = new("load");
        public ManagerPerformanceViewModel RAMPerformance { get; private set; } = new("load");
        public ManagerPerformanceViewModel ReadPerformance { get; private set; } = new("Rows");
        public ManagerPerformanceViewModel WrittenPerformance { get; private set; } = new("Rows");
        public List<ManagerPerformanceViewModel> DataCollections { get; private set; } = new();
        #endregion

        #region Chart objects
        public LiveChartViewModel CPUChart { get; private set; }
        public LiveChartViewModel RAMChart { get; private set; }
        public LiveChartViewModel ReadChart { get; private set; }
        public LiveChartViewModel WrittenChart { get; private set; }
        public List<LiveChartViewModel> Charts { get; private set; } = new();
        #endregion

        public ManagerChartViewModel()
        {
            #region Charts
            CPUChart = new(CPUPerformance.Series, CPUPerformance.ManagerData, CPUPerformance.XAxis, CPUPerformance.YAxis);
            RAMChart = new(RAMPerformance.Series, RAMPerformance.ManagerData, RAMPerformance.XAxis, RAMPerformance.YAxis);
            ReadChart = new(ReadPerformance.Series, ReadPerformance.ManagerData, ReadPerformance.XAxis, ReadPerformance.YAxis);
            WrittenChart = new(ReadPerformance.Series, ReadPerformance.ManagerData, ReadPerformance.XAxis, ReadPerformance.YAxis);
            #endregion

            #region Lists
            Charts = new List<LiveChartViewModel> { CPUChart, RAMChart, ReadChart, WrittenChart };
            DataCollections = new List<ManagerPerformanceViewModel> { CPUPerformance, RAMPerformance, ReadPerformance, WrittenPerformance };
            #endregion
        }

        /// <summary>
        /// Adds a line to all manager charts based on the data stored in the manager.
        /// </summary>
        /// The manager to be added to the chart <param name="manager"></param>
        public void AddChartLinesHelper(ManagerWrapper manager)
        {
            
            foreach (LiveChartViewModel chart in Charts)
            {
                var managerValues = new ObservableCollection<ObservablePoint>(); // DELETE LATER: Exchange with the data stored in the manager.

                chart.AddData(new LineSeries<ObservablePoint>
                {
                    Name = $"{manager.Manager.Name.Split(".").Last()}",
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColor.Parse(manager.LineColor.Color.ToString()), 3),
                    GeometryFill = new SolidColorPaint(SKColor.Parse(manager.LineColor.Color.ToString())),
                    GeometryStroke = new SolidColorPaint(SKColor.Parse(manager.LineColor.Color.ToString())),
                    GeometrySize = 0.4,
                    TooltipLabelFormatter = e => manager.Manager.Name.Split(".").Last() + "\n" +
                                                 "ID: " + manager.Manager.ContextId + " Execution " + manager.Manager.ExecutionId + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 e.PrimaryValue.ToString("P"),
                }, managerValues);
            }
        }

        /// <summary>
        /// Removes a manager from the charts.
        /// </summary>
        /// The manager to be removed <param name="manager"></param>
        public void RemoveChartLinesHelper(ManagerWrapper manager)
        {
            foreach (LiveChartViewModel chart in Charts)
            {
                var managerValues = new ObservableCollection<ObservablePoint>();
                chart.RemoveData(manager.Manager.Name, managerValues);
            }
        }

        /// <summary>
        /// Clears the charts of all managers.
        /// </summary>
        public void ClearChartLinesHelper()
        {
            foreach (LiveChartViewModel chart in Charts)
            {
                chart.Series.Clear();
            }
        }
    }
}
