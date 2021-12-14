using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.Defaults;

namespace DashboardFrontend.Charts
{
    /// <summary>
    /// A class for the creating and controlling <see cref="ISeries"/>
    /// </summary>
    public class ManagerChartWrapper : ChartWrapper
    {
        public ManagerChartWrapper(ChartTemplate chart, bool shouldAutoFocus) : base(chart, shouldAutoFocus)
        {
        }

        /// <summary>
        /// Adds a line to the chart and assigns a list of values to this line
        /// </summary>
        /// The line to be added <param name="Line"></param>
        /// The associated data to the line <param name="data"></param>
        public void AddLine(ISeries Line, ObservableCollection<ObservablePoint> data)
        {
            Line.Values = data;
            Chart.Series.Add(Line);
        }

        /// <summary>
        /// Removes a line from the chart but keeps its data in storage
        /// </summary>
        /// The line to be removed <param name="dataName"></param>
        /// The data associated with the line <param name="data"></param>
        public void RemoveData(string dataName, ObservableCollection<ObservablePoint> data)
        {
            string shortName = dataName.Split('.').Last();
            int managerIndex = Chart.Series.FindIndex(e => e.Name == shortName);
            if (managerIndex > -1) Chart.Series.RemoveAt(managerIndex);
        }
    }
}
