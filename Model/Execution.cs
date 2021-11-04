using System;
using System.Collections.Generic;

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
        public long Id { get; } //From [EXECUTION_ID] in [dbo].[EXECUTIONS].
        public DateTime StartTime { get; private set; } //From [CREATED] in [dbo].[EXECUTIONS].
        public DateTime EndTime { get; private set; } //DateTime.Now when an execution is registered as done (from log?).
        public TimeSpan Runtime { get; private set; } //EndTime.Subtract(StartTime)
        public int RowsReadTotal { get; set; } //OnExecutionFinished, for each manager, RowsReadTotal += RowsRead.
        public int RowsWrittenTotal { get; set; } //OnExecutionFinished, for each manager, RowsWrittenTotal += RowsWritten.
        public ExecutionStatus Status { get; private set; } //This one is obvious, dummy.
        public List<Manager> Managers { get; } = new(); //From [dbo].[MANAGERS], where [EXECUTIONS_ID] = Id.
        #endregion
    }
}
