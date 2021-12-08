using System.Collections.ObjectModel;

namespace Model
{
    public class Conversion : BaseViewModel
    {
        public Conversion()
        {
            Executions = new();
            AllManagers = new();
            HealthReport = new();
            LastExecutionQuery = _sqlMinDateTime;
            LastLogQuery = _sqlMinDateTime;
            LastManagerQuery = _sqlMinDateTime;
            LastValidationsQuery = _sqlMinDateTime;
        }

        private ObservableCollection<Execution> _executions;
        public ObservableCollection<Execution> Executions
        {
            get => _executions;
            set
            {
                _executions = value;
                OnPropertyChanged(nameof(Executions));
            }
        }
        private Execution _activeExecution;
        public Execution ActiveExecution
        {
            get => _activeExecution;
            set
            {
                _activeExecution = value;
                OnPropertyChanged(nameof(ActiveExecution));
            }
        }
        private ObservableCollection<Manager> _allManagers;
        public ObservableCollection<Manager> AllManagers
        {
            get => _allManagers;
            set
            {
                _allManagers = value;
                OnPropertyChanged(nameof(AllManagers));
            }
        }
        private HealthReport _healthReport;
        public HealthReport HealthReport
        {
            get => _healthReport; set
            {
                _healthReport = value;
                OnPropertyChanged(nameof(HealthReport));
            }
        }

        public DateTime LastExecutionQuery { get; set; }
        public DateTime LastLogQuery { get; set; }
        public DateTime LastManagerQuery { get; set; }
        public DateTime LastValidationsQuery { get; set; }
        public DateTime LastLogUpdated { get; set; }
        public DateTime LastManagerUpdated { get; set; }
        public DateTime LastHealthReportUpdated { get; set; }
        public DateTime LastValidationsUpdated { get; set; }

        public void AddExecution(Execution execution)
        {
            if (ActiveExecution != null)
            {
                ActiveExecution.EndTime = execution.StartTime;
                ActiveExecution.Status = ExecutionStatus.Finished;
                if (ActiveExecution.StartTime.HasValue && ActiveExecution.EndTime.HasValue)
                {
                    ActiveExecution.Runtime = ActiveExecution.EndTime.Value.Subtract(ActiveExecution.StartTime.Value);
                }
            }
            Executions.Add(execution);
        }
    }
}