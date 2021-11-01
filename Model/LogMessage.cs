namespace Model
{
    public class LogMessage
    {
        public LogMessage(string content, LogMessageType type, Manager manager, DateTime date)
        {
            Content = content;
            Type = type;
            Date = date;
            Manager = manager;    
        }

        public enum LogMessageType
        {
            INFO, WARNING, ERROR, VALIDATION
        }

        public LogMessageType Type { get; }
        public string Content { get; }
        public DateTime Date { get; }
        public Manager Manager { get; }
    }
}
