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
        public List<Manager> AllManagers { get; set; } = new();
        public HealthReport HealthReport { get; set; } = new();

        #endregion
    }
}