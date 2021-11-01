namespace Model
{
    public class Execution
    {
        #region Constructors
        public Execution(int id, DateTime startTime)
        {
            Id = id;
            StartTime = startTime;
        }

        public Execution(int id, DateTime startTime, DateTime endTime, int rowsReadTotal, int rowsWrittenTotal)
            : this(id, startTime)
        {
            Runtime = endTime - startTime;
            Status = ExecutionStatus.FINISHED;
            RowsReadTotal = rowsReadTotal;
            RowsWrittenTotal = rowsWrittenTotal;
        }
        #endregion
        
        #region Enums
        public enum ExecutionStatus
        {
            STARTED, FINISHED
        }
        #endregion

        #region Properties
        public int Id { get; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public TimeSpan Runtime { get; private set; }
        public int RowsReadTotal { get; set; }
        public int RowsWrittenTotal { get; set; }
        public ExecutionStatus Status { get; private set; }
        public List<Manager> Managers { get; } = new();
        #endregion
    }
}
