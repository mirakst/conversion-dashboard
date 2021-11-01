namespace Model
{
    public class Manager
    {
        #region Constructors
        public Manager(int id, int execId, string name)
        {
            Id = id;
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
            Runtime = EndTime - StartTime;
            Status = ManagerStatus.OK;
        }
        #endregion

        public enum ManagerStatus
        {
            READY, RUNNING, OK
        }

        public int Id { get; }
        public int ExecutionId { get; }
        public string Name { get; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public TimeSpan Runtime { get; private set; }
        public ManagerStatus Status { get; private set; }
        public List<ManagerUsage> Readings { get; set; } = new();
    }
}
