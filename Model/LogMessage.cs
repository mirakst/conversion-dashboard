namespace Model
{
    [Flags]
    public enum LogMessageType : byte
    {
        None = 0, Info = 1, Warning = 2, Error = 4, Fatal = 8, Validation = 16
    }

    public class LogMessage : BaseViewModel
    {
        public LogMessage(string content, LogMessageType type, int contextId, int executionId, DateTime date)
        {
            Content = content;
            Type = type;
            Date = date;
            ContextId = contextId;
            ExecutionId = executionId;
        }

        public LogMessageType Type { get; } //From [LOG_LEVEL] in [dbo].[LOGGING].
        public string Content { get; } //From [LOG_MESSAGE] in [dbo].[LOGGING].
        public DateTime Date { get; } //From [CREATED] in [dbo].[LOGGING].
        public int ContextId { get; } //From [CONTEXT_ID] in [dbo].[LOGGING].
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