using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace DashboardFrontend.ViewModels;

public abstract class BaseChart
{
    public List<ObservableCollection<ObservablePoint>> Values { get; set; }
    public List<ISeries> Series { get; set; }
    public List<Axis> XAxis { get; set; }
    public List<Axis> YAxis { get; set; }
}