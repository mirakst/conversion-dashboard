namespace Model
{
    public class Conversion
    {
        #region Constructors
        public Conversion()
        {
        }
        #endregion

        #region Properties
        public string Name { get; set; } //Assigned by user in dialog popup.
        public DateTime DateModified { get; set; } //DateTime.Now when configuration is updated.
        public bool IsInitialized { get; set; } //If the conversion has been built.
        public List<Execution> Executions { get; set; } = new(); //Created on new entry in [dbo].[EXECUTIONS]
        public Execution ActiveExecution  => Executions.LastOrDefault();
        public DateTime LastExecutionUpdate { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
        public DateTime LastLogUpdate { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
        public DateTime LastManagerUpdate { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
        public List<Manager> AllManagers { get; set; } = new();
        public HealthReport HealthReport { get; set; } = new();

        #endregion

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, DateModified);
        }

        public override bool Equals(object obj)
        {
            if (obj is not Conversion other)
                    return false;

            return GetHashCode() == other.GetHashCode();
        }

        public void AddExecution(Execution execution)
        {
            if (ActiveExecution != null)
            {
                ActiveExecution.EndTime = DateTime.Now;
                ActiveExecution.Status = Execution.ExecutionStatus.Finished;
                if (ActiveExecution.StartTime.HasValue && ActiveExecution.EndTime.HasValue)
                {
                    ActiveExecution.Runtime = ActiveExecution.EndTime.Value.Subtract(ActiveExecution.StartTime.Value);
                }
            }
            Executions.Add(execution);
        }
    }
}
