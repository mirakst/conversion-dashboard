namespace Model
{
    public class Log
    {
        #region Constructors
        public Log()
        {

        }
        #endregion Constructors

        #region Enums
        [Flags]
        public enum LogFilters : byte
        {
            Info = 0,
            Warning = 1,
            Error = 2,
            Validation = 4
        }
        #endregion Enums

        #region Properties
        private readonly List<LogMessage> _messages = new();
        public List<LogMessage> Messages
        {
            get => _messages;
            set
            {
                _messages.AddRange(value);
                LastModified = DateTime.Now;
            } } //From [dbo].[LOGGING], where [EXECUTION_ID] = someExecution.Id.
        public DateTime LastModified { get; private set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue; //Date of last modification.
        #endregion Properties
    }
}
