using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DashboardBackend;
using DashboardFrontend.Settings;
using Model;

namespace DashboardFrontend
{
    public delegate void ConversionCreated(Conversion conversion);

    public interface IDashboardController
    {
        event ConversionCreated OnConversionCreated;

        IDatabaseHandler DatabaseHandler { get; }
        IUserSettings UserSettings { get; }
        Conversion? Conversion { get; set; }
        bool HasConversion { get; }
        bool IsRunning { get; }

        void SetupNewConversion();
        void OnChangeMonitoringStateRequested();
    }
}
