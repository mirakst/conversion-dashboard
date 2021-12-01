using System.Diagnostics.CodeAnalysis;

namespace Model
{
    public class Manager
    {
        #region Constructors
        public Manager()
        {
            Status = ManagerStatus.Ready;
        }

        public Manager(int id, int execId, string name)
        {
            ContextId = id;
            ExecutionId = execId;
            Name = name;
            Status = ManagerStatus.Ready;
        }

        public Manager(int id, int execId, string name, DateTime startTime)
            : this(id, execId, name)
        {
            StartTime = startTime;
            Status = ManagerStatus.Running;
        }

        public Manager(int id, int execId, string name, DateTime startTime, DateTime endTime)
            : this(id, execId, name, startTime)
        {
            EndTime = endTime;
            Status = ManagerStatus.Ok;
        }
        #endregion

        #region Enums
        public enum ManagerStatus : byte
        {
            Ready, Running, Ok
        }
        #endregion Enums

        #region Properties      
        public int ContextId { get; set; } //[CONTEXT_ID] from [dbo].[LOGGING_CONTEXT], [ROW_ID] from [dbo].[MANAGERS]
        public int ExecutionId { get; set; } //[EXECUTION_ID] from [dbo].[MANAGERS]
        public List<CpuLoad> CpuReadings { get; set; } = new();
        public List<RamLoad> RamReadings { get; set; } = new();
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                var splitName = value.Split('.');
                if (splitName.Contains("managers"))
                {
                    ShortName = "(...)" + string.Join(".", splitName.TakeLast(2));
                }
                else
                {
                    ShortName = "(...)" + string.Join(".", splitName.Skip(4));
                }
            }
        } //[MANAGER_NAME] from [dbo].[MANAGERS]

        public string ShortName { get; set; }
        public DateTime? StartTime { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'START_TIME'.
        public DateTime? EndTime { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'END_TIME'.
        public TimeSpan? Runtime { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'runtimeOverall'.
        public ManagerStatus? Status { get; set; } //[STATUS] from [dbo].[MANAGER_TRACKING], where [MGR] = Name - until a manager start is logged, in which case it is RUNNING until a manager finishing is logged.
        /*        public List<ManagerUsage> Readings { get; set; } = new(); //Readings from [dbo].[MANAGER_TRACKING], where [MGR] = Name.*/
        public int? RowsRead { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='READ [TOTAL]'.
        public int? RowsWritten { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='WRITE [TOTAL]'.
        public double Score { get; set; }
        #endregion

        public override string ToString()
        {
            return $"MANAGER ID: {ContextId}\nMANAGER EXECUTION ID: {ExecutionId}\nMANAGER NAME: {Name}\n" +
                   $"START TIME: {StartTime}\nEND TIME: {EndTime}\nRUNTIME: {Runtime}\nROWS READ: {RowsRead}\n" +
                   $"ROWS WRITTEN: {RowsWritten}\n";
        }

        public override bool Equals(object obj)
        {
            return obj is Manager other && other.ContextId == ContextId && other.Name == Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ContextId, Name);
        }
    }
}
