namespace Model
{
    public class CpuLoad : PerformanceMetric
    {
        #region Constructors
        public CpuLoad(int executionId, double load, DateTime date)
        {
            ExecutionId = executionId;
            Load = load;
            Date = date;
        }
        #endregion Constructors

        public override string ToString()
        {
            return $"{Date.ToLongTimeString()}: {Load}%";
        }
    }
}
