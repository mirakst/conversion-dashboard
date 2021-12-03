namespace Model
{
    public class ManagerUsage
    {
        #region Constructors
        public ManagerUsage(int execId, int rowsRead, int rowsWritten, DateTime date)
        {
            ExecutionId = execId;
            RowsRead = rowsRead;
            RowsWritten = rowsWritten;
            Date = date;
        }
        #endregion Constructors

        #region Properties
        public int ExecutionId { get; } //From parentManager.ExecutionId (?), or SomeExecution.Id
        public int RowsRead { get; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='READ [TOTAL]'.
        public int RowsWritten { get; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='WRITE [TOTAL]'.
        public DateTime Date { get; } //From [TIMESTAMP] in [dbo].[ENGINE_PROPERTIES].
        #endregion Properties

        public override string ToString()
        {
            return $"Execution Id: {ExecutionId}\n" +
                   $"Date: {Date}\n" +
                   $"Rows read: {RowsRead}\n" +
                   $"Rows written: {RowsWritten}\n";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ExecutionId, Date.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is not ManagerUsage other)
                return false;

            return GetHashCode() == other.GetHashCode();
        }
    }
}
