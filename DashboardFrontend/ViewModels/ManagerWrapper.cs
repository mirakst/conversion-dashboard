using System;
using Model;
using System.Windows.Media;
using System.Collections.ObjectModel;
using LiveChartsCore.Defaults;
using System.Collections.Generic;

namespace DashboardFrontend
{
    /// <summary>
    /// Wrapper class for manager to store a color associated with the manager
    /// </summary>
    public class ManagerWrapper
    {
        public Manager Manager {  get; set; }
        public SolidColorBrush LineColor { get; private set; }
        public List<ObservableCollection<ObservablePoint>> ManagerValues { get; set; } = new(4) { new(), new(), new(), new() };

        public ManagerWrapper(Manager manager)
        {
            Manager = manager;
            LineColor = new(RandomColor());
        }

        /// <summary>
        /// Assigns the managerwrapper with a random color using RGB.
        /// </summary>
        /// <returns></returns>
        public Color RandomColor()
        {
            Random rand = new();

            int r = rand.Next(255);
            int g = rand.Next(255);
            int b = rand.Next(255);

            return Color.FromRgb((byte)r, (byte)g, (byte)b);
        }

    }
}
