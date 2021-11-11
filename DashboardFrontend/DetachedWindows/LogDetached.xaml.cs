using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace DashboardFrontend.DetachedWindows
{
    
    public class LogEntry
    {
        public string Date { get; set; }
        public string Type { get; set; }
        public long Manager { get; set; }
        public string Content { get; set; }
    }

    public class LogFilter
    {
        public static bool ShowInfo { get; set; } = true;
        public static bool ShowWarn { get; set; } = true;
        public static bool ShowError { get; set; } = true;
        public static bool ShowFatal { get; set; } = true;
        public static bool ShowValidation { get; set; } = true;

    }

    public partial class LogDetached : Window
    {
        List<LogEntry> logList = new List<LogEntry>();
        List<LogEntry> filteredList = new List<LogEntry>();

        public LogDetached()
        {
            InitializeComponent();
            AddItems();
            PrintList(logList);

        }
        public void AddItems()
        {
            logList.Add(new LogEntry() { Content = "Det her er en besked", Type = "Info", Manager = 2, Date = "10:11:2021" });
            logList.Add(new LogEntry() { Content = "Det her er en advarsel", Type = "Warn", Manager = 2, Date = "10:11:2021" });
            logList.Add(new LogEntry() { Content = "Det her er en fejl", Type = "Error", Manager = 2, Date = "10:11:2021" });
            logList.Add(new LogEntry() { Content = "Det her er en TOTAL DESTRUKTION", Type = "Fatal", Manager = 2, Date = "10:11:2021" });
            logList.Add(new LogEntry() { Content = "Det her er en validation", Type = "Validation", Manager = 3, Date = "10/11:2021" });
            logList.Add(new LogEntry() { Content = "Det her er en endnu besked", Type = "Info", Manager = 2, Date = "10:11:2021" });
            logList.Add(new LogEntry() { Content = "Det her er en  asneiubesked", Type = "Info", Manager = 2, Date = "10:11:2021" });
            logList.Add(new LogEntry() { Content = "Det her er en edadvarsel", Type = "Warn", Manager = 2, Date = "10:11:2021" });
            logList.Add(new LogEntry() { Content = "Det her er en efefeffejl", Type = "Error", Manager = 2, Date = "10:11:2021" });
        }

        public void PrintList(List<LogEntry> inputList)
        {
            dataGridLogEntry.Items.Clear();
            foreach (LogEntry logEntry in inputList)
            {

                dataGridLogEntry.Items.Add(logEntry);

            }
            
        }
        
        
        public void OnToggleButtonCheck(object sender, RoutedEventArgs e)
        {
            var button = (FrameworkElement)sender;
            string logType = (string)button.Tag;
            
            switch (logType)
            {
                case "Info":
                    LogFilter.ShowInfo = !LogFilter.ShowInfo;
                    break;
                case "Warn":
                    LogFilter.ShowWarn = !LogFilter.ShowWarn;
                    break;
                case "Error":
                    LogFilter.ShowError = !LogFilter.ShowError;
                    break;
                case "Fatal":
                    LogFilter.ShowFatal = !LogFilter.ShowFatal;
                    break;
                case "Validation":
                    LogFilter.ShowValidation = !LogFilter.ShowValidation;
                    break;
            }
            
        }

        /*
        public void toggleButtonChecked(object sender, RoutedEventArgs e)
        {
            var fuckinglort = (FrameworkElement)sender;
            string logType = (string)fuckinglort.Tag;
            filteredList.AddRange(logList.Where(e => e.Type == logType).ToList());
            PrintList(filteredList);
        }
        public void OnToggleButtonUnCheck(object sender, RoutedEventArgs e)
        {
            var fuckinglort = (FrameworkElement)sender;
            string logType = (string)fuckinglort.Tag;
            filteredList = filteredList.Where(e => e.Type != logType).ToList();
            PrintList(filteredList);
        }

        */
        
        
        
        
    }
}
