using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DashboardBackend.Settings;
using DashboardBackend;
using Microsoft.VisualBasic;

namespace DashboardFrontend.Controllers
{
    public interface IDashboardController
    {
        IDataHandler DataHandler { get; set; }
        IUserSettings UserSettings { get; set; }
        Conversion Conversion { get; set; }
        bool IsRunning { get; }

        void SetupNewConversion();
        void ChangeMonitoringState();
        void StartMonitoring();
        void StopMonitoring();
        void TryUpdateLog();
        void TryUpdateExecutions();
        void TryUpdateManagers();
    }
}
