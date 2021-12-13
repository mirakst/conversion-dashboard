namespace Model
{
    public class RamLoad : PerformanceMetric
    {
        public RamLoad()
        {

        }

        public RamLoad(int executionId, long available, DateTime date)
        {
            ExecutionId = executionId;
            Date = date;
            Available = available;
        }


        public long Available;
        
        public override string ToString()
        {
            return $"{Date.ToLongTimeString()}: {Load} bytes";
        }
    }
}
