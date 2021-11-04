using System;

namespace Model
{
    public class LogMessage
    {
        public LogMessage(string content, LogMessageType type, int contextId, DateTime date)
        {
            Content = content;
            Type = type;
            Date = date;
            ContextId = contextId;    
        }

        public enum LogMessageType : byte
        {
            INFO, WARNING, ERROR, FATAL, VALIDATION
        }

        public LogMessageType Type { get; } //From [LOG_LEVEL] in [dbo].[LOGGING].
        public string Content { get; } //From [LOG_MESSAGE] in [dbo].[LOGGING].
        public DateTime Date { get; } //From [CREATED] in [dbo].[LOGGING].
        public int ContextId { get; }
        public Manager Manager { get; } //Based on [CONTEXT_ID] in [dbo].[LOGGING], read function necessary, GetManagerById - returns a manager where Id = [CONTEXT_ID].

        public override string ToString()
        {
            return $"{Date} [{Type}]: {Content}";
        }
    }
}
