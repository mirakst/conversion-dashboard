namespace Model
{
    public class CpuLoad
    {
        public CpuLoad(int executionId, double load, DateTime date)
        {
            ExecutionId = executionId;
            Load = load;
            Date = date;
        }

        public int ExecutionId { get; }
        public double Load { get; }
        public DateTime Date { get; }
    }
}
