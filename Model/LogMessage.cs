namespace Model
{
    [Flags]
    public enum LogMessageType : byte
    {
        None = 0, Info = 1, Warning = 2, Error = 4, Fatal = 8, Validation = 16
    }

    public class LogMessage : ObservableObject
    {
        public LogMessage(string content, LogMessageType type, int contextId, int executionId, DateTime date)
        {
            Content = content;
            Type = type;
            Date = date;
            ContextId = contextId;
            ExecutionId = executionId;
        }

        public LogMessageType Type { get; }
        public string Content { get; }
        public DateTime Date { get; }
        public int ContextId { get; }
        public int ExecutionId { get; }
        private string _managerName;
        public string ManagerName
        {
            get => _managerName; set
            {
                _managerName = value;
                OnPropertyChanged(nameof(ManagerName));
            }
        }

        public override string ToString()
        {
            return $"{Date} [{Type}]: {Content}";
        }
    }
}