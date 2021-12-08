using System.Collections.ObjectModel;

namespace Model
{
    public enum ExecutionStatus
    {
        Started, Finished
    }

    public class Execution : BaseViewModel
    {
        public Execution(int id, DateTime startTime)
        {
            Id = id;
            StartTime = startTime;
            Status = ExecutionStatus.Started;
            Log = new();
            Managers = new();
        }

        public int Id { get; }
        public Log Log { get; set; }
        //public ValidationReport ValidationReport { get; set; }
        //public DateTime LastUpdatedManagers { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
        private ObservableCollection<Manager> _managers;
        public ObservableCollection<Manager> Managers
        {
            get => _managers;
            set
            {
                _managers = value;
                OnPropertyChanged(nameof(Managers));
            }
        }
        public DateTime? StartTime { get; }
        private DateTime? _endTime;
        public DateTime? EndTime
        {
            get => _endTime;
            set
            {
                _endTime = value;
                OnPropertyChanged(nameof(EndTime));
            }
        }
        private TimeSpan? _runtime;
        public TimeSpan? Runtime
        {
            get => _runtime; set
            {
                _runtime = value;
                OnPropertyChanged(nameof(Runtime));
            }
        }
        private ExecutionStatus _status;
        public ExecutionStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                if (value is ExecutionStatus.Finished)
                {
                    CurrentProgress = 100;
                }
                OnPropertyChanged(nameof(Status));
            }
        }
        private int _estimatedManagerCount;
        public int EstimatedManagerCount
        {
            get => _estimatedManagerCount; set
            {
                _estimatedManagerCount = value;
                OnPropertyChanged(nameof(EstimatedManagerCount));
            }
        }
        private int _currentProgress;
        public int CurrentProgress
        {
            get => _currentProgress;
            private set
            {
                _currentProgress = value;
                OnPropertyChanged(nameof(CurrentProgress));
            }
        }

        /// <summary>
        /// Adds a manager to the execution. If its status is <see cref="ManagerStatus.Ok"/>, <see cref="CurrentProgress"/> is updated. Otherwise, we subscribe to its <see cref="Manager.OnManagerFinished"/> event.
        /// </summary>
        /// <remarks>It is necessary to check the manager's status since it may already be finished before it is added, which means the event is invoked with no subscribers, and as a consequence, the progress will not be updated.</remarks>
        /// <param name="manager">The manager to add.</param>
        public void AddManager(Manager manager)
        {
            if (manager.Status is ManagerStatus.Ok)
            {
                UpdateProgress();
            }
            else
            {
                manager.OnManagerFinished += UpdateProgress;
            }
            Managers.Add(manager);
        }

        /// <summary>
        /// Calculates the percentage of managers in the execution with status <see cref="ManagerStatus.Ok"/> out of the estimated number of managers to run.
        /// </summary>
        /// <remarks>The estimated count is taken from the LOGGING_CONTEXT table on the assumption that all context entries are written at the start of an execution.</remarks>
        private void UpdateProgress()
        {
            if (EstimatedManagerCount > 0)
            {
                CurrentProgress = (int)Math.Floor(Managers.Count(m => m.Status is ManagerStatus.Ok) / (double)EstimatedManagerCount * 100);
            }
        }

        public override string ToString()
        {
            return $"Execution {Id} ({Status}): {StartTime}-{EndTime}, {Managers.Count} managers";
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
