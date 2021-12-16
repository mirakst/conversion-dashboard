using System.Collections.Generic;
using System.Linq;

namespace ConversionEngineSimulator
{
    public class ExecutionTable : IDatabaseTable
    {
        public ExecutionTable()
        {
            ColumnNames = "EXECUTION_ID, EXECUTION_UUID, CREATED";
            OutputColumnNames = "@EXECUTION_ID, @EXECUTION_UUID, @CREATED";
            TableName = "dbo.EXECUTIONS";
            Entries = DbUtilities.QueryTable<Execution>(this);
            Entries.Sort();
            DbInfo.ConvStartTime = Entries.First().CREATED; //Set the starting time for streaming.
        }
        public string ColumnNames { get; }

        public string OutputColumnNames { get; }

        public string TableName { get; }
        public List<Execution> Entries { get; set;  }
    }
}