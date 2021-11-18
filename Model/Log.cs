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
        public LogFilters Filters { get; set; }
        public List<LogMessage> Messages { get; set; } = new(); //From [dbo].[LOGGING], where [EXECUTION_ID] = someExecution.Id.
        #endregion Properties
    }
}
