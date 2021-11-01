namespace Model
{
    public class RamUsage
    {
        public RamUsage(int executionId, int available, DateTime date)
        {
            ExecutionId = executionId;
            Available = available;
            Date = date;
        }

        public int ExecutionId { get; }
        public int Available { get; }
        public DateTime Date { get; }
    }
}
