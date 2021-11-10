using System.Windows;
using System.Collections.Generic;

namespace DashboardFrontend.DetachedWindows
{
    public class LogEntry
    {
        public string Date { get; set; }
        public string Type { get; set; }
        public long Manager { get; set; }
        public string Content { get; set; }
    }

    public partial class LogDetached : Window
    {
        public LogDetached()
        {
            InitializeComponent();
            
            
            dataGridLogEntry.Items.Add(new LogEntry() { Content = "Det her er en besked", Type = "Info", Manager = 2, Date = "10:11:2021"});
            dataGridLogEntry.Items.Add(new LogEntry() { Content = "Det her er en advarsel", Type = "Warn", Manager = 2, Date = "10:11:2021" });
            dataGridLogEntry.Items.Add(new LogEntry() { Content = "Det her er en fejl", Type = "Error", Manager = 2, Date = "10:11:2021" });
            dataGridLogEntry.Items.Add(new LogEntry() { Content = "Det her er en TOTAL DESTRUKTION ", Type = "Fatal", Manager = 2, Date = "10:11:2021" });
            dataGridLogEntry.Items.Add(new LogEntry() { Content = "Det her er en validation ", Type = "Validation", Manager = 3, Date = "10/11:2021" });

            
        }
    }
}
