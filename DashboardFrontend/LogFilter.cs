using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace DashboardFrontend
{
    public class LogFilter : INotifyPropertyChanged
    {
        public LogFilter()
        {
            AddItems();
            
            
        }
        private bool _showInfo = true;
        private bool _showWarn = true;
        private bool _showError = true;
        private bool _showFatal = true;
        private bool _showValidation = true;

        public bool ShowInfo
        {
            get { return _showInfo; }
            set
            { 
                _showInfo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowInfo)));
                UpdateList();
            }
        }
        public bool ShowWarn
        {
            get { return _showWarn; }
            set
            {
                _showWarn = value;
                UpdateList();
            }
        }
        public bool ShowError
        {
            get { return _showError; }
            set
            {
                _showError = value;
                UpdateList();
            }
        }
        public bool ShowFatal
        {
            get { return _showFatal; }
            set
            {
                _showFatal = value;
                UpdateList();
            }
        }
        public bool ShowValidation
        {
            get { return _showValidation; }
            set
            {
                _showValidation = value;
                UpdateList();
            }
        }


        public ObservableCollection<LogEntry> LogListAll = new();
        public ObservableCollection<LogEntry> LogList = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public void AddItems()
        {
            LogList.Add(new LogEntry() { Content = "Det her er en besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en advarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en fejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en TOTAL DESTRUKTION", Type = "Fatal", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en validation", Type = "Validation", Manager = 3, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en endnu besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en  asneiubesked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en edadvarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en efefeffejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en advarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en fejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en TOTAL DESTRUKTION", Type = "Fatal", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en validation", Type = "Validation", Manager = 3, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en endnu besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en  asneiubesked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en edadvarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en efefeffejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en advarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en fejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en TOTAL DESTRUKTION", Type = "Fatal", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en validation", Type = "Validation", Manager = 3, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en endnu besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en  asneiubesked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en edadvarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en efefeffejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en advarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en fejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en TOTAL DESTRUKTION", Type = "Fatal", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en validation", Type = "Validation", Manager = 3, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en endnu besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en  asneiubesked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en edadvarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en efefeffejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en advarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en fejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en TOTAL DESTRUKTION", Type = "Fatal", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en validation", Type = "Validation", Manager = 3, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en endnu besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en  asneiubesked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en edadvarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en efefeffejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en advarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en fejl", Type = "Error", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en TOTAL DESTRUKTION", Type = "Fatal", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en validation", Type = "Validation", Manager = 3, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en endnu besked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en  asneiubesked", Type = "Info", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en edadvarsel", Type = "Warn", Manager = 2, Date = DateTime.Now });
            LogList.Add(new LogEntry() { Content = "Det her er en efefeffejl", Type = "Error", Manager = 2, Date = DateTime.Now });
        }
        public void UpdateList()
        {
            LogList.Clear();
            foreach (LogEntry logEntry in LogList)
            {
                switch (logEntry.Type)
                {
                    case "Info":
                        if (ShowInfo)
                            LogList.Add(logEntry);
                        break;
                    case "Warn":
                        if (ShowWarn)
                            LogList.Add(logEntry);
                        break;
                    case "Error":
                        if (ShowError)
                            LogList.Add(logEntry);
                        break;
                    case "Fatal":
                        if (ShowFatal)
                            LogList.Add(logEntry);
                        break;
                    case "Validation":
                        if (ShowValidation)
                            LogList.Add(logEntry);
                        break;
                }
            }
        }
    }
}
