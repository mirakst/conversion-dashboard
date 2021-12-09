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

        public LogMessageType Type
        {
            get => _type; set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }
        public string Content
        {
            get => _content; set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }
        public DateTime Date
        {
            get => _date; set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        public int ContextId
        {
            get => _contextId; set
            {
                _contextId = value;
                OnPropertyChanged(nameof(ContextId));
            }
        }
        public int ExecutionId
        {
            get => _executionId; set
            {
                _executionId = value;
                OnPropertyChanged(nameof(ExecutionId));
            }
        }
        private string _managerName;
        private LogMessageType _type;
        private string _content;
        private DateTime _date;
        private int _contextId;
        private int _executionId;

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