namespace Model
{
    public class Log
    {
        #region Constructors
        public Log()
        {

        }
        #endregion Constructors

        #region Properties
        private readonly List<LogMessage> _messages = new();
        public List<LogMessage> Messages
        {
            get => _messages;
            set
            {
                _messages.AddRange(value);
                UpdateCounters(value);
            }
        } //From [dbo].[LOGGING], where [EXECUTION_ID] = someExecution.Id.
        public int InfoCount = 0;
        public int WarnCount = 0;
        public int ErrorCount = 0;
        public int FatalCount = 0;
        public int ValidationCount = 0;
        public DateTime LastModified { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue; //Date of last modification.
        #endregion Properties

        /// <summary>
        /// Increments the counter property that corresponds to the LogMessageType of the given LogMessage
        /// </summary>
        /// <param name="msg">LogMessage whose type counter should be updated</param>
        private void UpdateCounters(List<LogMessage> messages)
        {
            foreach (var msg in messages)
            {
                switch (msg.Type)
                {
                    case LogMessage.LogMessageType.Info:
                        InfoCount++;
                        break;
                    case LogMessage.LogMessageType.Warning:
                        WarnCount++;
                        break;
                    case LogMessage.LogMessageType.Error:
                        ErrorCount++;
                        break;
                    case LogMessage.LogMessageType.Fatal:
                        FatalCount++;
                        break;
                    case LogMessage.LogMessageType.Validation:
                        ValidationCount++;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
