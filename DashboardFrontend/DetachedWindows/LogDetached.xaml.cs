using System;
using System.Windows;
using System.Collections.Generic;

namespace DashboardFrontend.DetachedWindows
{
    
    public class LogEntry
    {
        public DateTime Date { get; set; }
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

        public LogDetached()
        {
            InitializeComponent();
            AddItems();
            PrintList(logList);

        }
        public void AddItems()
        {
            logList.Add(new LogEntry() { Content = "Det her er en besked", Type = "Info", Manager = 2, Date =  DateTime.Now});
            logList.Add(new LogEntry() { Content = "Det her er en advarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en fejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en TOTAL DESTRUKTION", Type = "Fatal", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en validation", Type = "Validation", Manager = 3, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en endnu besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en  asneiubesked", Type = "Info", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en edadvarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en efefeffejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en advarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en fejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en TOTAL DESTRUKTION", Type = "Fatal", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en validation", Type = "Validation", Manager = 3, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en endnu besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en  asneiubesked", Type = "Info", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en edadvarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en efefeffejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en advarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en fejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en TOTAL DESTRUKTION", Type = "Fatal", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en validation", Type = "Validation", Manager = 3, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en endnu besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en  asneiubesked", Type = "Info", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en edadvarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            logList.Add(new LogEntry() { Content = "Det her er en efefeffejl", Type = "Error", Manager = 2, Date = DateTime.Now });
        }

        public void PrintList(List<LogEntry> inputList)
        {
            dataGridLogEntry.Items.Clear();
            foreach (LogEntry logEntry in inputList)
            {
                switch (logEntry.Type)
                {
                    case "Info":
                        if (LogFilter.ShowInfo)
                            dataGridLogEntry.Items.Add(logEntry);
                        break;
                    case "Warn":
                        if (LogFilter.ShowWarn)
                            dataGridLogEntry.Items.Add(logEntry);
                        break;
                    case "Error":
                        if (LogFilter.ShowError)
                            dataGridLogEntry.Items.Add(logEntry);
                        break;
                    case "Fatal":
                        if (LogFilter.ShowFatal)
                            dataGridLogEntry.Items.Add(logEntry);
                        break;
                    case "Validation":
                        if (LogFilter.ShowValidation)
                            dataGridLogEntry.Items.Add(logEntry);
                        break;
                }
            }
        }
        
        public void OnToggleButtonClick(object sender, RoutedEventArgs e)
        {
            var logToggleButton = (FrameworkElement)sender;
            string logType = (string)logToggleButton.Tag;
            
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

            PrintList(logList); 
        }
    }
}
