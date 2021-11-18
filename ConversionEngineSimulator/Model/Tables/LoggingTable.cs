using System.Collections.Generic;

namespace ConversionEngineSimulator
{
    public class LoggingTable : IDatabaseTable
    {
        public LoggingTable()
        {
            ColumnNames = "CREATED, LOG_MESSAGE, LOG_LEVEL, EXECUTION_ID, CONTEXT_ID";
            OutputColumnNames = "@CREATED, @LOG_MESSAGE, @LOG_LEVEL, @EXECUTION_ID, @CONTEXT_ID";
            TableName = "dbo.LOGGING";
            Entries = DBUtilities.QueryTable<LogMsg>(this);
            Entries.Sort();
        }
        public string ColumnNames { get; }

        public string OutputColumnNames { get; }

        public string TableName { get; }
        public List<LogMsg> Entries { get; set; }
    }
}