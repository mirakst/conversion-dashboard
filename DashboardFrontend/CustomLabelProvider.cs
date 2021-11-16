using InteractiveDataDisplay.Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DashboardFrontend
{
    public class CustomLabelProvider : ILabelProvider
    {
        public static DateTime Origin = new(2000, 1, 1);

        public FrameworkElement[] GetLabels(double[] ticks)
        {
            if (ticks == null)
                throw new ArgumentNullException("ticks");


            List<TextBlock> Labels = new();
            foreach (double tick in ticks)
            {
                TextBlock text = new();
                var time = Origin + TimeSpan.FromTicks((long)tick);
                text.Text = time.ToLongTimeString();
                Labels.Add(text);
            }
            return Labels.ToArray();
        }
    }

    public class CustomAxis : Axis
    {
        public CustomAxis() : base(new CustomLabelProvider(), new TicksProvider())
        {
        }
    }
}
