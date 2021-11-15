using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardBackend.Models
{
    public class LogMessage
    {
        public LogMessage(string content, string type, DateTime date)
        {
            Content = content;
            Type = type;
            Date = date;
        }

        public DateTime Date { get; set; }
        public string Type { get; set; }
        public long Manager { get; set; }
        public string Content { get; set; }
    }

    public class Log
    {
        public List<LogMessage> Messages = new()
        {
            new("Info #1", "Info", DateTime.Now),
            new("Info #2", "Info", DateTime.Now),
            new("Error #1", "Error", DateTime.Now),
            new("Error #2", "Error", DateTime.Now),
            new("Fatal #1", "Fatal", DateTime.Now),
            new("Warning #1", "Warn", DateTime.Now)
        };
    }
}
