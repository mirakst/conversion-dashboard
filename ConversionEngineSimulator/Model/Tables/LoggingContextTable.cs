using System.Collections.Generic;

namespace ConversionEngineSimulator
{
    public class LoggingContextTable : IDatabaseTable
    {
        public LoggingContextTable()
        {
            ColumnNames = "CONTEXT_ID, EXECUTION_ID, CONTEXT";
            OutputColumnNames = "@CONTEXT_ID, @EXECUTION_ID, @CONTEXT";
            TableName = "dbo.LOGGING_CONTEXT";
            Entries = DbUtilities.QueryTable<LoggingContext>(this);
        }
        public string ColumnNames { get; }

        public string OutputColumnNames { get; }

        public string TableName { get; }
        public List<LoggingContext> Entries { get; set; }
    }
}