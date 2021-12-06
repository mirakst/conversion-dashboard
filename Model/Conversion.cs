using System.Data.SqlTypes;

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

        public DateTime LastExecutionQuery { get; set; } = (DateTime)SqlDateTime.MinValue;
        public DateTime LastLogQuery { get; set; } = (DateTime)SqlDateTime.MinValue;
        public DateTime LastManagerQuery { get; set; } = (DateTime)SqlDateTime.MinValue;
        public DateTime LastLogUpdated { get; set; } = DateTime.MinValue;
        public DateTime LastManagerUpdated { get; set; } = DateTime.MinValue;
        public DateTime LastHealthReportUpdated { get; set; } = DateTime.MinValue;

        public DateTime LastValidationsUpdated { get; set; } = DateTime.MinValue;
        public List<Manager> AllManagers { get; set; } = new();
        public HealthReport HealthReport { get; set; } = new();

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