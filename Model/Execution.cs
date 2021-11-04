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
            Status = ExecutionStatus.FINISHED;
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
            STARTED, FINISHED
        }
        #endregion

        #region Properties
        public int Id { get; } //From [EXECUTION_ID] in [dbo].[EXECUTIONS].
        public DateTime StartTime { get; private set; } //From [CREATED] in [dbo].[EXECUTIONS].
        public DateTime EndTime { get; private set; } //DateTime.Now when an execution is registered as done (from log?).
        public TimeSpan Runtime { get; private set; } //EndTime.Subtract(StartTime)
        public int RowsReadTotal { get; set; } //OnExecutionFinished, for each manager, RowsReadTotal += RowsRead.
        public int RowsWrittenTotal { get; set; } //OnExecutionFinished, for each manager, RowsWrittenTotal += RowsWritten.
        public ExecutionStatus Status { get; set; } //This one is obvious. Private set when better solution is found?
        public List<Manager> Managers { get; set; } = new();  //From [dbo].[MANAGERS], where [EXECUTIONS_ID] = Id.
        public ValidationReport ValidationReport { get; set; } = new();
        public Log Log { get; set; } = new();
        #endregion

        public override string ToString()
        {
            return $"ID: {Id}\nSTART TIME: {StartTime}\nEND TIME: {EndTime}\n" +
                   $"RUNTIME: {Runtime}\nROWS READ TOTAL: {RowsReadTotal}\n" +
                   $"ROWS WRITTEN TOTAL: {RowsWrittenTotal}\nSTATUS: {Status}\n" +
                   $"MANAGERS: {Managers.Count}\n";
        }
    }
}
