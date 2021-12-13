using System.Data.SqlTypes;

namespace Model
{
    public class Conversion
    {
        #region Constructors
        public Conversion()
        {
            Executions = new();
            AllManagers = new();
            HealthReport = new();
            LastExecutionQuery = (DateTime)SqlDateTime.MinValue;
            LastLogQuery = (DateTime)SqlDateTime.MinValue;
            LastManagerQuery = (DateTime)SqlDateTime.MinValue;
            LastValidationsQuery = (DateTime)SqlDateTime.MinValue;
        }
        #endregion

        #region Properties
        public DateTime DateModified { get; set; } //DateTime.Now when configuration is updated.
        public bool IsInitialized { get; set; } //If the conversion has been built.
        public List<Execution> Executions { get; set; } //Created on new entry in [dbo].[EXECUTIONS]
        public Execution ActiveExecution  => Executions.LastOrDefault();

        public DateTime LastExecutionQuery { get; set; }
        public DateTime LastLogQuery { get; set; }
        public DateTime LastManagerQuery { get; set; }
        public DateTime LastValidationsQuery { get; set; }
        public DateTime LastLogUpdated { get; set; }
        public DateTime LastManagerUpdated { get; set; }
        public DateTime LastHealthReportUpdated { get; set; }
        public DateTime LastValidationsUpdated { get; set; }

        public List<Manager> AllManagers { get; set; }
        public HealthReport HealthReport { get; set; }

        #endregion

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