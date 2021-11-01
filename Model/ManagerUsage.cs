namespace Model
{
    public class ManagerUsage
    {
        public ManagerUsage(int execId, int rowsRead, int rowsWritten, DateTime date)
        {
            ExecutionId = execId;
            RowsRead = rowsRead;
            RowsWritten = rowsWritten;
            Date = date;
        }

        public int ExecutionId { get; }
        public int RowsRead { get; }
        public int RowsWritten { get; }
        public DateTime Date { get; }
    }
}
