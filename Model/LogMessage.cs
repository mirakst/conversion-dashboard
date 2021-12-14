namespace Model
{
    public class LogMessage
    {
        #region Constructors
        public LogMessage(string content, LogMessageType type, int contextId, int executionId, DateTime date)
        {
            Content = content;
            Type = type;
            Date = date;
            ContextId = contextId;
            ExecutionId = executionId;
        }
        #endregion Constructors

        #region Enums
        [Flags]
        public enum LogMessageType : byte
        {
            None = 0, Info = 1, Warning = 2, Error = 4, Fatal = 8, Validation = 16
        }
        #endregion Enums

        #region Properties
        public LogMessageType Type { get; } //From [LOG_LEVEL] in [dbo].[LOGGING].
        public string Content { get; } //From [LOG_MESSAGE] in [dbo].[LOGGING].
        public DateTime Date { get; } //From [CREATED] in [dbo].[LOGGING].
        public int ContextId { get; } //From [CONTEXT_ID] in [dbo].[LOGGING].
        public int ExecutionId { get; }

        public string ManagerName { get; set; }

        #endregion Properties

        public override string ToString()
        {
            return $"{Date.ToString(new CultureInfo("da-DK"))} [{Type}]: {Content}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Date, Content.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is not LogMessage other)
            {
                return false;
            }

            return GetHashCode() == other.GetHashCode();
        }
    }
}