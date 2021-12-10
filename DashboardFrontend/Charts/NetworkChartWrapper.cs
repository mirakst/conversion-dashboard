using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using DashboardFrontend.ViewModels;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Model;

namespace DashboardFrontend.Charts
{
    /// <summary>
    /// A class for the creating and controlling <see cref="ISeries"/>
    /// </summary>
    public class NetworkChartWrapper : ChartWrapper
    {
        public NetworkChartWrapper(ChartTemplate chart, bool shouldAutoFocus) : base(chart, shouldAutoFocus)
        {
        }

        private DateTime LastNetPlot { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;

        public void UpdateData(Network? network)
        {
            if (network is null) return;
            foreach (var item in network.Readings.Where(e => e.Date > LastNetPlot))
            {
                switch (Chart.Type)
                {
                    case ChartTemplate.ChartType.Network:
                        LastPrimaryReading = item.BytesSend / Math.Pow(1024, 3);
                        LastSecondaryReading = item.BytesReceived / Math.Pow(1024, 3);
                        UpdateNetworkData(item);
                        break;
                    case ChartTemplate.ChartType.NetworkDelta:
                        LastPrimaryReading = item.BytesSendDelta / Math.Pow(1024, 2);
                        LastSecondaryReading = item.BytesReceivedDelta / Math.Pow(1024, 2);
                        UpdateNetworkDeltaData(item);
                        break;
                    case ChartTemplate.ChartType.NetworkSpeed:
                        LastPrimaryReading = item.BytesSendSpeed / Math.Pow(1024, 2);
                        LastSecondaryReading = item.BytesReceivedSpeed / Math.Pow(1024, 2);
                        UpdateNetworkSpeedData(item);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (network.Readings.Count > 0)
            {
                LastNetPlot = network.Readings.Last().Date;
            }
        }

        public void UpdateNetworkData(NetworkUsage reading)
        {
            double bytesSendFormatted = reading.BytesSend / Math.Pow(1024, 3);
            double bytesReceivedFormatted = reading.BytesReceived / Math.Pow(1024, 3);
            Chart.Values[0].Add(CreatePoint(bytesSendFormatted, reading.Date));
            Chart.Values[1].Add(CreatePoint(bytesReceivedFormatted, reading.Date));
        }

        public void UpdateNetworkDeltaData(NetworkUsage reading)
        {
            double bytesSendFormatted = reading.BytesSendDelta / Math.Pow(1024, 2);
            double bytesReceivedFormatted = reading.BytesReceivedDelta / Math.Pow(1024, 2);
            Chart.Values[0].Add(CreatePoint(bytesSendFormatted, reading.Date));
            Chart.Values[1].Add(CreatePoint(bytesReceivedFormatted, reading.Date));
        }

        public void UpdateNetworkSpeedData(NetworkUsage reading)
        {
            double bytesSendFormatted = reading.BytesSendSpeed / Math.Pow(1024, 2);
            double bytesReceivedFormatted = reading.BytesReceivedSpeed / Math.Pow(1024, 2);
            Chart.Values[0].Add(CreatePoint(bytesSendFormatted, reading.Date));
            Chart.Values[1].Add(CreatePoint(bytesReceivedFormatted, reading.Date));
        }

        /// <summary>
        /// Creates a new <see cref="ObservablePoint"/> from <see cref="NetworkUsage"/> data.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private ObservablePoint CreatePoint(double readingSize, DateTime time)
        {
            return new ObservablePoint(time.ToOADate(), readingSize);
        }
    }
}
