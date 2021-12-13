using System.Windows.Data;

namespace Model
{
    public enum ExecutionStatus
    {
        Started, Finished
    }

    public class Execution : ObservableObject
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
        public Log Log { get; }
        private WpfObservableRangeCollection<Manager> _managers;
        public WpfObservableRangeCollection<Manager> Managers
        {
            get => _managers;
            set
            {
                _managers = value;
                OnPropertyChanged(nameof(Managers));
            }
        }
        private DateTime? _startTime;
        public DateTime? StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value;
                OnPropertyChanged(nameof(StartTime));
                if (StartTime.HasValue && EndTime.HasValue)
                {
                    Runtime = EndTime.Value.Subtract(StartTime.Value);
                }
            }
        }
        private DateTime? _endTime;
        public DateTime? EndTime
        {
            get => _endTime;
            set
            {
                _endTime = value;
                OnPropertyChanged(nameof(EndTime));
                if (StartTime.HasValue && EndTime.HasValue)
                {
                    Runtime = EndTime.Value.Subtract(StartTime.Value);
                }
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
                    if (StartTime.HasValue && EndTime.HasValue)
                    {
                        Runtime = EndTime.Value.Subtract(StartTime.Value);
                    }
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
        private int _validationsTotal;
        public int ValidationsTotal
        {
            get => _validationsTotal;
            set
            {
                _validationsTotal = value;
                OnPropertyChanged(nameof(ValidationsTotal));
            }
        }
        private int _validationsOk;
        public int ValidationsOk
        {
            get => _validationsOk;
            set
            {
                _validationsOk = value;
                OnPropertyChanged(nameof(ValidationsOk));
            }
        }
        private int _validationsDisabled;
        private int _validationsFailed;

        public int ValidationsDisabled
        {
            get => _validationsDisabled;
            set
            {
                _validationsDisabled = value;
                OnPropertyChanged(nameof(ValidationsDisabled));
            }
        }
        public int ValidationsFailed
        {
            get => _validationsFailed; 
            set
            {
                _validationsFailed = value;
                OnPropertyChanged(nameof(ValidationsFailed));
            }
        }

        /// <summary>
        /// Adds a manager to the execution. If its status is <see cref="ManagerStatus.Ok"/>, <see cref="CurrentProgress"/> is updated. Otherwise, we subscribe to its <see cref="Manager.OnManagerFinished"/> event.
        /// </summary>
        /// <remarks>It is necessary to check the manager's status since it may already be finished before it is added, which means the event is invoked with no subscribers, and as a consequence, the progress will not be updated.</remarks>
        /// <param name="manager">The manager to add.</param>
        public void AddManager(Manager manager)
        {
            Managers.Add(manager);
            if (manager.Status is ManagerStatus.Ok)
            {
                UpdateProgress();
            }
            else
            {
                manager.OnManagerFinished += UpdateProgress;
            }

            manager.Validations.CollectionChanged += ManagerValidations_CollectionChanged;
        }

        /// <summary>
        /// Adds a list of managers to the execution.
        /// </summary>
        /// <remarks>This method is similar to <see cref="AddManager(Manager)"/>, but only updates progress and invokes OnCollectionChanged after all managers have been processed.</remarks>
        /// <param name="managers">The list of managers to add.</param>
        public void AddManagers(IList<Manager> managers)
        {
            Managers.AddRange(managers);
            foreach (Manager manager in managers)
            {
                manager.Validations.CollectionChanged += ManagerValidations_CollectionChanged;
                if (manager.Status is not ManagerStatus.Ok)
                {
                    manager.OnManagerFinished += UpdateProgress;
                }
            }
            UpdateProgress();
        }

        private void ManagerValidations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateValidationCounters();
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

        private void UpdateValidationCounters()
        {
            int total = 0;
            int ok = 0;
            int disabled = 0;
            int failed = 0;
            lock (Managers)
            {
                foreach (var manager in Managers)
                {
                    total += manager.Validations.Count;
                    ok += manager.ValidationsOk;
                    disabled += manager.ValidationsDisabled;
                    failed += manager.ValidationsFailed;
                }
            }
            ValidationsTotal = total;
            ValidationsOk = ok;
            ValidationsDisabled = disabled;
            ValidationsFailed = failed;
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
