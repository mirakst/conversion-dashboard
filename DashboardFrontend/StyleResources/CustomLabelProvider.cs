using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DashboardFrontend.StyleResources
{
    //public class CustomLabelProvider : ILabelProvider
    //{
    //    private static readonly DateTime Origin = new(2000, 1, 1);

    //    public FrameworkElement[] GetLabels(double[] ticks)
    //    {
    //        if (ticks == null)
    //            throw new ArgumentNullException(nameof(ticks));

    //        List<TextBlock> labels = new();
    //        foreach (double tick in ticks)
    //        {
    //            TextBlock text = new();
    //            var time = Origin + TimeSpan.FromTicks((long)tick);
    //            text.Text = time.ToLongTimeString();
    //            labels.Add(text);

    //        }
    //        return labels.ToArray();
    //    }
    //}

    //public class CustomAxis : Axis
    //{
    //    public CustomAxis() : base(new CustomLabelProvider(), new TicksProvider())
    //    {
    //    }
    //}
}
