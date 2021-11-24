namespace Model;

public abstract class PerformanceMetric
{
    public int ExecutionId { get; set;  } //From [EXECUTION_ID] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'LOAD'.
    public double Load { get; set;  } //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'LOAD'.
    public DateTime Date { get; set; } //From [LOG_TIME] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'LOAD'.
    //The properties above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'CPU'.
}