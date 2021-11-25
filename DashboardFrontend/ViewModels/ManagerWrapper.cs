using System;
using Model;
using System.Windows.Media;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting;

namespace DashboardFrontend
{
    public class ManagerWrapper
    {
        public Manager Manager {  get; set; }
        public SolidColorBrush LineColor { get; private set; }

        public ManagerWrapper(Manager manager)
        {
            Manager = manager;
            LineColor = new(RandomColor());
        }

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
