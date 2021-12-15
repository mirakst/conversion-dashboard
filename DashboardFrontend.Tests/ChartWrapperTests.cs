using Model;
using System;
using System.Collections.Generic;
using Xunit;
using DashboardFrontend.Charts;
using LiveChartsCore.Defaults;
using System.Collections.ObjectModel;

namespace DashboardFrontend.Tests
{
    public class ChartWrapperTests
    {
        [Fact]
        public void PerformanceChartWrapper_UpdateData_PlotsPerformanceReadingsCorrectly()
        {
            // Arrange
            PerformanceChartWrapper chart = new(new PerformanceChartTemplate(), false);
            Ram ram = new(100)
            {
                Readings =
                {
                    new RamLoad(1, 0.5, 50, DateTime.Parse("01-01-2020 12:00:00"))
                }
            };
            Cpu cpu = new()
            {
                Readings =
                {
                    new CpuLoad(1, 0.5, DateTime.Parse("01-01-2020 12:00:00"))
                }
            };

            // Act
            chart.UpdateData(ram, cpu);

            // Assert
            Assert.Collection(chart.Chart.Values,
                item => Assert.IsType<ObservableCollection<ObservablePoint>>(item),
                item => Assert.IsType<ObservableCollection<ObservablePoint>>(item));
            var ramReading = Assert.Single(chart.Chart.Values[0]);
            Assert.Equal(0.5, ramReading.Y);
            Assert.Equal(DateTime.Parse("01-01-2020 12:00:00"), DateTime.FromOADate(ramReading.X!.Value));
            var cpuReading = Assert.Single(chart.Chart.Values[1]);
            Assert.Equal(0.5, cpuReading.Y);
            Assert.Equal(DateTime.Parse("01-01-2020 12:00:00"), DateTime.FromOADate(cpuReading.X!.Value));
        }

    } 
}