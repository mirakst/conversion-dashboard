using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using InteractiveDataDisplay.Core;

namespace DashboardFrontend
{
    internal class ManagerMonitoring
    {

        public void AddLineGraph(Grid chart, string _name)
        {
            LineGraph graph = new()
            {
                Name = _name
            };

            chart.Children.Add(graph);

        }
    }
}
