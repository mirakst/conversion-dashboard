using System.Collections.Generic;

namespace Filestreaming_Program
{
    public class EnginePropertyTable : IDatabaseTable
    {
        public EnginePropertyTable()
        {
            ColumnNames = "MANAGER, [KEY], VALUE, TIMESTAMP, RUN_NO";
            OutputColumnNames = "@MANAGER, @KEY, @VALUE, @TIMESTAMP, @RUN_NO";
            TableName = "dbo.ENGINE_PROPERTIES";
            Entries = DBUtilities.QueryTable<EngineProperty>(this);
            Entries.Sort();
        }
        public string ColumnNames { get; }

        public string OutputColumnNames { get; }

        public string TableName { get; }
        public List<EngineProperty> Entries { get; set; }
    }
}