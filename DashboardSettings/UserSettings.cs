using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardSettings
{
    public static class UserSettings
    {
        public static List<Profile> Profiles { get; set; } = new() 
        {
            new Profile("Nikolaj desktop", "Conversion #1", "NIKOLAJ-DESKTOP", "ANS_CUSTOM_2", 30),
            new Profile("Nikolaj laptop", "Conversion #2", @"NIKOLAJ-DAM-LEN\SQLEXPRESS", "KONV_DEST_1_CUSTOM", 15)
        };

        public static Profile? ActiveProfile { get; set; } = Profiles.FirstOrDefault();
        public static int LoggingQueryInterval { get; set; } = 15; // seconds
        public static int HealthReportQueryInterval { get; set; } = 30;
        public static int ValidationQueryInterval { get; set; } = 120;
        public static int ManagerQueryInterval { get; set; } = 60;
        public static int AllQueryInterval { get; set; } = 30;
        public static bool SynchronizeAllQueries { get; set; } = false;
        public static bool HasActiveProfile => ActiveProfile is not null;
    }
}
