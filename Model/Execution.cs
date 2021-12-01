namespace Model
{
    public class Execution
    {
        #region Constructors
        public Execution(int id, DateTime startTime)
        {
            Id = id;
            StartTime = startTime;
            Status = ExecutionStatus.Finished;
        }

        //Probably won't ever get to use this. [ENGINE_PROPERTIES] does not include [EXECUTION_ID], but is constantly overwritten.
        /*public Execution(int id, DateTime startTime, DateTime endTime, int rowsReadTotal, int rowsWrittenTotal)
            : this(id, startTime)
        {
            Runtime = endTime - startTime;
            Status = ExecutionStatus.FINISHED;
            RowsReadTotal = rowsReadTotal;
            RowsWrittenTotal = rowsWrittenTotal;
        }*/
        #endregion

        #region Enums
        public enum ExecutionStatus
        {
            Started, Finished
        }
        #endregion

        public Manager CurrentManager { get; set; }
        public List<Manager> Managers { get; set; } = new();  //From [dbo].[MANAGERS], where [EXECUTIONS_ID] = Id.
        public Dictionary<int, Manager> ContextIdManagerDict = new();

        #region Properties
        public int Id { get; } //From [EXECUTION_ID] in [dbo].[EXECUTIONS].
        public DateTime StartTime { get; } //From [CREATED] in [dbo].[EXECUTIONS].
        public DateTime EndTime { get; set; } //DateTime.Now when an execution is registered as done (from log?).
        public TimeSpan Runtime => EndTime.Subtract(StartTime).Duration(); //EndTime.Subtract(StartTime)
        public int RowsReadTotal { get; set; } //OnExecutionFinished, for each manager, RowsReadTotal += RowsRead.
        public ExecutionStatus Status { get; set; } //Status of the manager.
        public ValidationReport ValidationReport { get; set; } = new();
        public Log Log { get; set; } = new();
        #endregion

        public void AddManager(Manager manager)
        {
            if (!ContextIdManagerDict.ContainsKey(manager.ContextId))
            {
                ContextIdManagerDict[manager.ContextId] = manager;
                Managers.Add(manager);
            }
        }

        public void SetUpDictionaries()
        {
            foreach (var manager in Managers)
            {
                ContextIdManagerDict.Add(manager.ContextId, manager);
            }
        }

        public override string ToString()
        {
            return $"ID: {Id}\nSTART TIME: {StartTime}\nEND TIME: {EndTime}\n" +
                   $"RUNTIME: {Runtime}\nROWS READ TOTAL: {RowsReadTotal}\n" +
                   $"STATUS: {Status}\nMANAGERS: {Managers.Count}\n";

        }
    }
}
