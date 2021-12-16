using System.Collections.Generic;

namespace ConversionEngineSimulator
{
    public class EnginePropertyTable : IDatabaseTable
    {
        public EnginePropertyTable()
        {
            ColumnNames = "MANAGER, [KEY], VALUE, TIMESTAMP, RUN_NO";
            OutputColumnNames = "@MANAGER, @KEY, @VALUE, @TIMESTAMP, @RUN_NO";
            TableName = "dbo.ENGINE_PROPERTIES";
            Entries = DbUtilities.QueryTable<EngineProperty>(this);
            Entries.Sort();
        }
        public string ColumnNames { get; }

        public string OutputColumnNames { get; }

        public string TableName { get; }
        public List<EngineProperty> Entries { get; set; }
    }
}