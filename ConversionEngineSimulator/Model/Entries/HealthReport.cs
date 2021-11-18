using System;

namespace ConversionEngineSimulator
{
    public class HealthReport : ITimestampedDatabaseEntry, IComparable<HealthReport>
    {
        public int ROW_NO { get; set; }
        public int MONITOR_NO { get; set; }
        public int EXECUTION_ID { get; set; }
        public string REPORT_TYPE { get; set; }
        public string REPORT_KEY { get; set; }
        public string REPORT_STRING_VALUE { get; set; }
        public double REPORT_NUMERIC_VALUE { get; set; }
        public string REPORT_VALUE_TYPE { get; set; }
        public string REPORT_VALUE_HUMAN { get; set; }
        public DateTime LOG_TIME { get; set; }
        public DateTime CREATED { get => LOG_TIME; set => LOG_TIME = value; }

        public int CompareTo(HealthReport other)
        {
            return CREATED.CompareTo(other.CREATED);
        }

        public override string ToString()
        {
            return $"{"[Health report]",-15} {CREATED}: {REPORT_TYPE} {REPORT_KEY} {REPORT_VALUE_HUMAN}";
        }
    }
}
