namespace Model
{
    public class CpuLoad
    {
        #region Constructors
        public CpuLoad(int executionId, long load, DateTime date)
        {
            ExecutionId = executionId;
            Load = load;
            Date = date;
        }
        #endregion Constructors

        #region Properties
        public int ExecutionId { get; } //From [EXECUTION_ID] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'LOAD'.
        public long Load { get; } //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'LOAD'.
        public DateTime Date { get; } //From [LOG_TIME] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'LOAD'.
        //The properties above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'CPU'.
        #endregion Properties

        public override string ToString()
        {
            return $"{Date.ToLongTimeString()}: {Load}%";
        }
    }
}
