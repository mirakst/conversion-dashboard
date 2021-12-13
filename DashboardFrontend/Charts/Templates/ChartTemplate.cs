using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace DashboardFrontend.Charts;

public abstract class ChartTemplate
{
    public List<ObservableCollection<ObservablePoint>> Values { get; set; }
    public List<ISeries> Series { get; set; }
    public List<Axis> XAxis { get; set; }
    public List<Axis> YAxis { get; set; }
    public ChartType Type { get; set; }

    public enum ChartType
    {
        Network, NetworkDelta, NetworkSpeed
    }
}