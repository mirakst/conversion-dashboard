using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace DashboardFrontend.Charts;

/// <summary>
/// A template for the data contained in charts.
/// Chart wrapper is built on this, or an inherited type of this.
/// </summary>
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