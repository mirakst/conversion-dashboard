using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using LiveChartsCore.Defaults;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Model;

namespace DashboardFrontend.ViewModels
{
    public class ManagerPerformanceViewModel
    {
        public List<ObservableCollection<ObservablePoint>> ManagerData { get; set; }
        public List<ISeries> Series { get; private set; }
        public LiveChartViewModel LineChart { get; private set; }

        public List<Axis> XAxis { get; private set; }
        public List<Axis> YAxis { get; private set; }

        public ManagerPerformanceViewModel(string YAxisLabel)
        {
            LineChart = new();

            ManagerData = new()
            {

            };

            Series = new()
            {

            };

            XAxis = new()
            {
                new Axis
                {
                    Name = "Time",
                    Labeler = value => DateTime.FromOADate(value).ToString("HH:mm:ss"),
                    MinLimit = DateTime.Now.ToOADate(),
                    MaxLimit = DateTime.Now.ToOADate(),
                }
            };

            YAxis = new()
            {
                new Axis
                {
                    Name = $"{YAxisLabel}",
                    Labeler = (value) => value.ToString("P"),
                    MaxLimit = 1,
                    MinLimit = 0,
                }
            };
        }
    }
}
