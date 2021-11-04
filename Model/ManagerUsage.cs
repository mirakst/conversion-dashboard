using System;

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

        public int ExecutionId { get; } //From parentManager.ExecutionId (?), or SomeExecution.Id
        public int RowsRead { get; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='READ [TOTAL]'.
        public int RowsWritten { get; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='WRITE [TOTAL]'.
        public DateTime Date { get; } //From [TIMESTAMP] in [dbo].[ENGINE_PROPERTIES].
    }
}
