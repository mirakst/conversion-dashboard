namespace Model
{
    public class CpuLoad : PerformanceMetric
    {
        public CpuLoad(int executionId, double load, DateTime date)
        {
            ExecutionId = executionId;
            Load = load;
            Date = date;
        }

        public override string ToString()
        {
            return $"{Date.ToLongTimeString()}: {Load}%";
        }
    }
}