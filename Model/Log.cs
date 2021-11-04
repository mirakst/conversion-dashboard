using System;
using System.Collections.Generic;

namespace Model
{
    public class Log
    {
        public Log()
        {

        }

        [Flags]
        public enum LogFilters : byte
        {
            INFO = 0,
            WARNING = 1,
            ERROR = 2,
            VALIDATION = 4
        }

        public LogFilters Filters { get; set; }
        public List<LogMessage> Messages { get; set; } = new(); //From [dbo].[LOGGING], where [EXECUTION_ID] = someExecution.Id .
    }
}
