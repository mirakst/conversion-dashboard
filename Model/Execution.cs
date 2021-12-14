using System.Globalization;

namespace Model
{
    public delegate void ExecutionProgressUpdated(Execution execution);

    public class Execution
    {
        public Execution(int id, DateTime startTime)
        {
            Id = id;
            StartTime = startTime;
            Status = ExecutionStatus.Started;
        }

        public enum ExecutionStatus
        {
            Started, Finished
        }

        public event ExecutionProgressUpdated OnExecutionProgressUpdated;

        public int CurrentProgress { get; private set; }
        public List<Manager> Managers { get; set; } = new();  //From [dbo].[MANAGERS], where [EXECUTIONS_ID] = Id.
        public int Id { get; } //From [EXECUTION_ID] in [dbo].[EXECUTIONS].
        public DateTime? StartTime { get; set; } //From [CREATED] in [dbo].[EXECUTIONS].
        public DateTime? EndTime { get; set; } //DateTime.Now when an execution is registered as done (from log?).
        public TimeSpan? Runtime { get; set; } //EndTime.Subtract(StartTime)
        private ExecutionStatus _status;
        public ExecutionStatus Status        {            get => _status;            set            {                _status = value;                if (value is ExecutionStatus.Finished)                {                    CurrentProgress = 100;                }                OnExecutionProgressUpdated?.Invoke(this);            }        }
        public Log Log { get; set; } = new();        public int EstimatedManagerCount { get; set; }
        public void AddManager(Manager manager)        {            if (manager.Status == Manager.ManagerStatus.Ok)            {                UpdateProgress(manager);            }            else            {                manager.OnManagerFinished += UpdateProgress;            }            Managers.Add(manager);        }        private void UpdateProgress(Manager manager)        {            if (EstimatedManagerCount > 0)            {                CurrentProgress = (int)Math.Floor((double)manager.ContextId / (double)EstimatedManagerCount * 100);            }            OnExecutionProgressUpdated?.Invoke(this);        }        public override string ToString()
        {
            return $"Execution {Id}: Status={Status} Start={StartTime?.ToString(new CultureInfo("da-DK"))} End={EndTime?.ToString(new CultureInfo("da-DK"))}";
        }

        public override bool Equals(object obj)
        {
            return (obj as Execution)?.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
