using Model;
using System;
using System.Collections.Generic;
using Xunit;
using DashboardFrontend.Charts;
using LiveChartsCore.Defaults;
using System.Collections.ObjectModel;

namespace DashboardFrontend.Tests
{
    public class TestsDataCharts
    {  
        [Fact]
        public void TestUpdateData()
        {
            //Arrange
            DataChart chart = new DataChart(new PerformanceChart(), false);
            chart.AutoFocusOff();

            var expected = new List<ObservableCollection<ObservablePoint>>()
            {
                new ObservableCollection<ObservablePoint> { new ObservablePoint( DateTime.Parse("01/01/2020 12:00:00").ToOADate(), 0.4) },
                new ObservableCollection<ObservablePoint> { new ObservablePoint( DateTime.Parse("01/01/2020 12:00:00").ToOADate(), 0.6) }

            };
            var actual = chart.ChartData.Values;

            //Act
            chart.UpdateData(new Ram(69420000000)
            {
                Readings = new List<RamLoad>()
            {
                new RamLoad(1,0.4, 13884000000, DateTime.Parse("01/01/2020 12:00:00"))
            }
            }, new Cpu("TestCpu", 8, 3)
            {
                Readings = new List<CpuLoad>()
            {
                new CpuLoad(1, 0.6, DateTime.Parse("01/01/2020 12:00:00"))
            }
            });

            //Assert
            Assert.True(expected[0][0].X == actual[0][0].X && expected[0][0].Y == actual[0][0].Y && expected[1][0].X == actual[1][0].X && expected[1][0].Y == actual[1][0].Y);

        }

    } 
}