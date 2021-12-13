namespace Model
{
    public class RamLoad : PerformanceMetric
    {
        #region Constructors
        public RamLoad(int executionId, long available, DateTime date)
        {
            ExecutionId = executionId;
            Date = date;
            Available = available;
        }
        #endregion Constructors

        #region Properties

        public long Available;
        #endregion Properties
        public override string ToString()
        {
            return $"{Date.ToLongTimeString()}: {Load} bytes";
        }
    }
}
