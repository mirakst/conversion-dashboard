using DashboardFrontend.Charts;
using LiveChartsCore.Defaults;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace DashboardFrontend.Tests
{
    public class TestDataChart
    {
        [Fact]
        public void UpdateData_Test()
        {
            var dataChart = new DataChart(new PerformanceChart());
            dataChart.AutoFocusOff();

            var expected = new List<ObservableCollection<ObservablePoint>>()
            {
                new ObservableCollection<ObservablePoint>()
                {
                    new ObservablePoint() { X = DateTime.Parse("01-01-2020 12:00:00").ToOADate(), Y = 0.5},
                    new ObservablePoint() { X = DateTime.Parse("01-01-2020 13:00:00").ToOADate(), Y = 0.6}
                },
                new ObservableCollection<ObservablePoint>()
                {
                    new ObservablePoint() { X = DateTime.Parse("01-01-2020 12:00:00").ToOADate(), Y = 0.2},
                    new ObservablePoint() { X = DateTime.Parse("01-01-2020 13:00:00").ToOADate(), Y = 0.3}
                }
            };

            var ram = new Ram(100)
            {
                Readings = new List<RamLoad>
                {
                    new RamLoad(1, 0.5, 50, DateTime.Parse("01-01-2020 12:00:00")),
                    new RamLoad(1, 0.6, 40, DateTime.Parse("01-01-2020 13:00:00")),
                }
            };

            var cpu = new Cpu("CPU", 4, 1234567890)
            {
                Readings = new List<CpuLoad>
                {
                    new CpuLoad(1, 0.2, DateTime.Parse("01-01-2020 12:00:00")),
                    new CpuLoad(1, 0.3, DateTime.Parse("01-01-2020 13:00:00"))
                }
            };

            dataChart.UpdateData(ram, cpu);

            var actual = dataChart.ChartData.Values;
            bool ZeroZero = expected[0][0].X == actual[0][0].X && expected[0][0].Y == actual[0][0].Y;
            bool ZeroOne = expected[0][1].X == actual[0][1].X && expected[0][1].Y == actual[0][1].Y;
            bool OneZero = expected[1][0].X == actual[1][0].X && expected[1][0].Y == actual[1][0].Y;
            bool OneOne = expected[1][1].X == actual[1][1].X && expected[1][1].Y == actual[1][1].Y;
            bool total = ZeroZero && ZeroOne && OneZero && OneOne;

            Assert.True(total);
        }
    }
}