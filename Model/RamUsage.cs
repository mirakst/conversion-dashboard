﻿namespace Model
{
    public class RamUsage
    {
        #region Constructors
        public RamUsage(int executionId, long available, DateTime date)
        {
            ExecutionId = executionId;
            Available = available;
            Date = date;
        }
        #endregion Constructors

        #region Properties
        public int ExecutionId { get; } //From [EXECUTION_ID] in [dbo].[HEALTH_REPORT].
        public long Available { get; } //bytes //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'AVAILABLE'.
        public DateTime Date { get; } //From [LOG_TIME] in [dbo].[HEALTH_REPORT].
        //The properties above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'MEMORY'.
        #endregion Properties

        public override string ToString()
        {
            return $"{Date.ToLongTimeString()}: {Available} bytes";
        }
    }
}