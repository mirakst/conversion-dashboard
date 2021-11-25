using System;
using Model;
using System.Windows.Media;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting;

namespace DashboardFrontend
{
    class ManagerWrapper
    {
        public Manager manager {  get; set; }
        public SolidColorBrush lineColor { get; private set; }
        public byte R { get; private set; }
        public byte G { get; private set; }
        public byte B { get; private set; }

        public ManagerWrapper(Manager manager)
        {
            this.manager = manager;
            lineColor = new(RandomColor());
        }

        public Color RandomColor()
        {
            Random rand = new();

            int r = rand.Next(255);
            int g = rand.Next(255);
            int b = rand.Next(255);

            R = (byte)r;
            G = (byte)g;
            B = (byte)b;

            return Color.FromRgb(R, G, B);
        }

    }
}
