﻿namespace Model
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
            INFO = 0,
            WARNING = 1,
            ERROR = 2,
            VALIDATION = 4
        }
        #endregion Enums

        #region Properties
        public LogFilters Filters { get; set; }
        public List<LogMessage> Messages { get; set; } = new(); //From [dbo].[LOGGING], where [EXECUTION_ID] = someExecution.Id.
        #endregion Properties
    }
}
