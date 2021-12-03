namespace Model
{
    public class Execution
    {
        #region Constructors
        public Execution(int id, DateTime startTime)
        {
            Id = id;
            StartTime = startTime;
            Status = ExecutionStatus.Started;
        }
        #endregion

        #region Enums
        public enum ExecutionStatus
        {
            Started, Finished
        }
        #endregion

        public Manager CurrentManager { get; set; }
        public List<Manager> Managers { get; set; } = new();  //From [dbo].[MANAGERS], where [EXECUTIONS_ID] = Id.
        public DateTime LastUpdatedManagers { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;

        #region Properties
        public int Id { get; } //From [EXECUTION_ID] in [dbo].[EXECUTIONS].
        public DateTime? StartTime { get; } //From [CREATED] in [dbo].[EXECUTIONS].
        public DateTime? EndTime { get; set; } //DateTime.Now when an execution is registered as done (from log?).
        public TimeSpan? Runtime { get; set; } //EndTime.Subtract(StartTime)
        public int RowsReadTotal { get; set; } //OnExecutionFinished, for each manager, RowsReadTotal += RowsRead.
        public ExecutionStatus Status { get; set; } //Status of the manager.
        public ValidationReport ValidationReport { get; set; } = new();
        public Log Log { get; set; } = new();
        #endregion

        public override string ToString()
        {
            return $"Execution {Id}: Status={Status} Start={StartTime} End={EndTime}";
        }

        public override bool Equals(object obj)
        {
            return (obj as Execution)?.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
