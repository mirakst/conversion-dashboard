namespace Model
{
    public class RamLoad : PerformanceMetric
    {
        public RamLoad(int executionId, double load, long available, DateTime date)
        {
            ExecutionId = executionId;
            Load = load;
            Date = date;
            Available = available;
        }

        public long Available { get; }
        
        public override string ToString()
        {
            return $"{Date.ToLongTimeString()}: {Load} bytes";
        }
    }
}
