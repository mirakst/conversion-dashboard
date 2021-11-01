using System.Collections.Generic;

namespace Filestreaming_Program
{

    public class ManagerTable : IDatabaseTable
    {
        public ManagerTable()
        {
            ColumnNames = "MANAGER_NAME, ROW_ID, EXECUTIONS_ID";
            OutputColumnNames = "@MANAGER_NAME, @ROW_ID, @EXECUTIONS_ID";
            TableName = "dbo.MANAGERS";
            Entries = DBUtilities.QueryTable<Manager>(this);
        }
        public string ColumnNames { get; }

        public string OutputColumnNames { get; }

        public string TableName { get; }
        public List<Manager> Entries { get; set; }
    }
}