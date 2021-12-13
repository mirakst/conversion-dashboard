using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using DashboardFrontend.ViewModels;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Model;

namespace DashboardFrontend.Charts
{
    /// <summary>
    /// A class for the creating and controlling <see cref="ISeries"/>
    /// </summary>
    public class PerformanceChartWrapper : ChartWrapper
    {
        public PerformanceChartWrapper(ChartTemplate chart, bool shouldAutoFocus) 
            : base(chart, shouldAutoFocus)
        {
        }

        /// <summary>
        /// Timestamps of last RAM and CPU data plot.
        /// </summary>
        private DateTime LastRamPlot { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
        private DateTime LastCpuPlot { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;

        /// <summary>
        /// Adds points to the series from the health report RAM and CPU objects.
        /// </summary>
        public void UpdateData(Ram? ram, Cpu? cpu)
        {
            if (ram is null || cpu is null) return;
            foreach (var item in ram.Readings.Where(e => e.Date > LastRamPlot))
            {
                Chart.Values[0].Add(CreatePoint(item));       
            }

            foreach (var item in cpu.Readings.Where(e => e.Date > LastCpuPlot))
            {
                Chart.Values[1].Add(CreatePoint(item));
            }
            if (cpu.Readings.Count > 0 && ram.Readings.Count > 0) 
            {
                LastPrimaryReading = cpu.Readings.Last().Load * 100;
                LastSecondaryReading = ram.Readings.Last().Load * 100;
                UpdatePlots(ram.Readings.Last().Date, cpu.Readings.Last().Date);
            }
        }

        /// <summary>
        /// Updates the timestamps of the last plots.
        /// </summary>
        /// <param name="ramDate">Date of the last RAM plot.</param>
        /// <param name="cpuDate">Date of the last CPU plot.</param>
        private void UpdatePlots(DateTime ramDate, DateTime cpuDate)
        {
            LastRamPlot = ramDate;
            LastCpuPlot = cpuDate;
        }

        /// <summary>
        /// Creates a point from a performance metric.
        /// </summary>
        private ObservablePoint CreatePoint(PerformanceMetric pointData)
        {
            return new ObservablePoint(pointData.Date.ToOADate(), pointData.Load);
        }
    }
}