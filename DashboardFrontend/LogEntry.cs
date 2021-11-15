using System;

namespace DashboardFrontend
{
    public class LogEntry
    {
        public LogEntry()
        {
            Date = DateTime.Now;
            Type = "fuck";
            Manager = 1;
            Content = "content...";
        }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public long Manager { get; set; }
        public string Content { get; set; }
    }
}
