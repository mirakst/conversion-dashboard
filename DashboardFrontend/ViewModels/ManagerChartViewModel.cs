using DashboardFrontend.Charts;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Model;

namespace DashboardFrontend.ViewModels
{
    public class ManagerChartViewModel
    {

        #region Chart objects

        public DataChart CPUChart { get; private set; } = new(new ManagerChart("load"));
        public DataChart RAMChart { get; private set; } = new(new ManagerChart("load"));
        public DataChart ReadChart { get; private set; } = new(new ManagerChart("Rows"));
        public DataChart WrittenChart { get; private set; } = new(new ManagerChart("Rows"));
        public List<DataChart> Charts { get; private set; } = new();
        #endregion

        private HealthReport _healthReport { get; set; }

        public ManagerChartViewModel(HealthReport healthReport)
        {
            _healthReport = healthReport;
            Charts.Add(CPUChart);
        }

        /// <summary>
        /// Adds a line to all manager charts based on the data stored in the manager.
        /// </summary>
        /// The manager to be added to the chart <param name="manager"></param>
        public void AddChartLinesHelper(ManagerWrapper manager)
        {
            List<ObservableCollection<ObservablePoint>> managerValues = new(4);
            managerValues.Add(new ObservableCollection<ObservablePoint>(_healthReport.Cpu.Readings
                .Where(e => e.Date >= manager.Manager.StartTime)
                .Where(e => e.Date < manager.Manager.EndTime)
                .Select(CreatePoint)));

            int index = 0;
            foreach (DataChart chart in Charts)
            {
                chart.AddLine(new LineSeries<ObservablePoint>
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
                }, managerValues[index++]);
            }
        }

        private ObservablePoint CreatePoint(PerformanceMetric pointData)
        {
            return new ObservablePoint(pointData.Date.ToOADate(), pointData.Load);
        }

        /// <summary>
        /// Removes a manager from the charts.
        /// </summary>
        /// The manager to be removed <param name="manager"></param>
        public void RemoveChartLinesHelper(ManagerWrapper manager)
        {
            foreach (DataChart chart in Charts)
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
            foreach (DataChart chart in Charts)
            {
                chart.ChartData.Series.Clear();
            }
        }
    }
}
