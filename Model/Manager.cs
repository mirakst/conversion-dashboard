namespace Model
{
    public class Manager
    {
        #region Constructors
        public Manager(int id, int execId, string name)
        {
            ContextId = id;
            ExecutionId = execId;
            Name = name;
            Status = ManagerStatus.READY;
        }

        public Manager(int id, int execId, string name, DateTime startTime)
            : this(id, execId, name)
        {
            StartTime = startTime;
            Status = ManagerStatus.RUNNING;
        }

        public Manager(int id, int execId, string name, DateTime startTime, DateTime endTime)
            : this(id, execId, name, startTime)
        {
            EndTime = endTime;
            Status = ManagerStatus.OK;
        }
        #endregion

        #region Enums
        public enum ManagerStatus : byte
        {
            READY, RUNNING, OK
        }
        #endregion Enums

        #region Properties
        public int ContextId { get; } //[CONTEXT_ID] from [dbo].[LOGGING_CONTEXT], [ROW_ID] from [dbo].[MANAGERS]
        public int ExecutionId { get; } //[EXECUTION_ID] from [dbo].[MANAGERS]
        public string Name { get; set; } //[MANAGER_NAME] from [dbo].[MANAGERS]
        public DateTime StartTime { get; private set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'START_TIME'.
        public DateTime EndTime { get; private set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'END_TIME'.
        public TimeSpan Runtime { get => EndTime - StartTime; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'runtimeOverall'.
        public ManagerStatus Status { get; private set; } //[STATUS] from [dbo].[MANAGER_TRACKING], where [MGR] = Name - until a manager start is logged, in which case it is RUNNING until a manager finishing is logged.
        /*        public List<ManagerUsage> Readings { get; set; } = new(); //Readings from [dbo].[MANAGER_TRACKING], where [MGR] = Name.
        */
        public int RowsRead { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='READ [TOTAL]'.
        public int RowsWritten { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='WRITE [TOTAL]'.
        #endregion Properties

        public void SetStartTime(string dateString)
        {
            StartTime = DateTime.Parse(dateString);
        }

        public void SetEndTime(string dateString)
        {
            EndTime = DateTime.Parse(dateString);
        }

        public override string ToString()
        {
            return $"MANAGER ID: {ContextId}\nMANAGER EXECUTION ID: {ExecutionId}\nMANAGER NAME: {Name}\n" +
                   $"START TIME: {StartTime}\nEND TIME: {EndTime}\nRUNTIME: {Runtime}\nROWS READ: {RowsRead}\n" +
                   $"ROWS WRITTEN: {RowsWritten}\n";
        }
    }
}
