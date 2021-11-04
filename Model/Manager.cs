using System;
using System.Collections.Generic;

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

        public enum ManagerStatus : byte
        {
            READY, RUNNING, OK
        }

        public int Id { get; } //[CONTEXT_ID] from [dbo].[LOGGING_CONTEXT], [ROW_ID] from [dbo].[MANAGERS]
        public int ExecutionId { get; } //[EXECUTION_ID] from [dbo].[MANAGERS]
        public string Name { get; } //[MANAGER_NAME] from [dbo].[MANAGERS]
        public DateTime StartTime { get; private set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'START_TIME'.
        public DateTime EndTime { get; private set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'END_TIME'.
        public TimeSpan Runtime { get; private set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'runtimeOverall'.
        public ManagerStatus Status { get; private set; } //[STATUS] from [dbo].[MANAGER_TRACKING], where [MGR] = Name - until a manager start is logged, in which case it is RUNNING until a manager finishing is logged.
        public List<ManagerUsage> Readings { get; set; } = new(); //Readings from [dbo].[MANAGER_TRACKING], where [MGR] = Name.

        public override string ToString()
        {
            return $"MANAGER ID: {Id}\nMANAGER EXECUTION ID: {ExecutionId}\nMANAGER NAME: {Name}\n";
        }
    }
}
