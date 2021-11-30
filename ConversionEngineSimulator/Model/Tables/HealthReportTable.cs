using System.Collections.Generic;

namespace ConversionEngineSimulator
{
    public class HealthReportTable : IDatabaseTable
    {
        public HealthReportTable()
        {
            ColumnNames = "ROW_NO, MONITOR_NO, EXECUTION_ID, REPORT_TYPE, REPORT_KEY, " +
                          "REPORT_STRING_VALUE, REPORT_NUMERIC_VALUE, REPORT_VALUE_TYPE, " +
                          "REPORT_VALUE_HUMAN, LOG_TIME";
            OutputColumnNames = "@ROW_NO, @MONITOR_NO, @EXECUTION_ID, @REPORT_TYPE, @REPORT_KEY, " +
                                "@REPORT_STRING_VALUE, @REPORT_NUMERIC_VALUE, @REPORT_VALUE_TYPE, " +
                                "@REPORT_VALUE_HUMAN, @LOG_TIME";
            TableName = "dbo.HEALTH_REPORT";
            Entries = DBUtilities.QueryTable<HealthReport>(this);
            Entries.Sort();
        }
        public string ColumnNames { get; }

        public string OutputColumnNames { get; }

        public string TableName { get; }
        public List<HealthReport> Entries { get; set; }
    }
}