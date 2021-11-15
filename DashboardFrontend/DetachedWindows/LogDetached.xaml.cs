using System;
using System.Windows;
using System.Collections.Generic;
using DashboardFrontend;

namespace DashboardFrontend.DetachedWindows
{

    public partial class LogDetached : Window
    {
        
        List<LogEntry> logList = new List<LogEntry>();

        private LogFilter _logFilter;

        public LogDetached(LogFilter logFilter)
        {

            InitializeComponent();
            _logFilter = logFilter;
            DataContext = _logFilter;

            AddItems();
            //PrintList(logList);

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
        

        
        
        public void OnToggleButtonClick(object sender, RoutedEventArgs e)
        {
            //var logToggleButton = (FrameworkElement)sender;
            //string logType = (string)logToggleButton.Tag;
            
            //switch (logType)
            //{
            //    case "Info":
            //        LogFilter.ShowInfo = !LogFilter.ShowInfo;
            //        break;
            //    case "Warn":
            //        LogFilter.ShowWarn = !LogFilter.ShowWarn;
            //        break;
            //    case "Error":
            //        LogFilter.ShowError = !LogFilter.ShowError;
            //        break;
            //    case "Fatal":
            //        LogFilter.ShowFatal = !LogFilter.ShowFatal;
            //        break;
            //    case "Validation":
            //        LogFilter.ShowValidation = !LogFilter.ShowValidation;
            //        break;
            //}

            //PrintList(logList); 
        }
    }
}
