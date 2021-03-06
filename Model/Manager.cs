using static Model.Reconciliation;

namespace Model
{
    public delegate void ManagerFinished(Manager manager);

    public class Manager
    {
        public Manager()
        {
            Status = ManagerStatus.Ready;
        }

        public enum ManagerStatus : byte
        {
            Ready, Running, Ok
        }

        public event ManagerFinished OnManagerFinished;

        public List<Reconciliation> Reconciliations { get; set; } = new();
        public List<CpuLoad> CpuReadings { get; set; } = new();
        public List<RamLoad> RamReadings { get; set; } = new();
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                ShortName = value.Split(".").Last();
            }
        }
        public int ContextId { get; set; }
        public int ExecutionId { get; set; }
        public string ShortName { get; set; }
        private DateTime? _startTime;
        public DateTime? StartTime
        {
            get => _startTime; set
            {
                _startTime = value;
                if (value is not null)
                {
                    Status = ManagerStatus.Running;
                    if (EndTime.HasValue)
                    {
                        Runtime = EndTime.Value.Subtract(value.Value);
                    }
                }
            }
        }
        private DateTime? _endTime;
        public DateTime? EndTime
        {
            get => _endTime; set
            {
                _endTime = value;
                if (value is not null)
                {
                    Status = ManagerStatus.Ok;
                    if (StartTime.HasValue)
                    {
                        Runtime = value.Value.Subtract(StartTime.Value);
                    }
                }
            }
        }
        public TimeSpan? Runtime
        {
            get => _runtime; 
            set
            {
                _runtime = value;
                if (value is not null)
                {
                    UpdatePerformanceScore();
                }
            }
        }
        private ManagerStatus _status;
        private TimeSpan? _runtime;
        public ManagerStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                if (value is ManagerStatus.Ok)
                {
                    OnManagerFinished?.Invoke(this);
                    UpdateReconciliationScore();
                    UpdatePerformanceScore();
                }
            }
        }
        public int? RowsRead { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='READ [TOTAL]'.
        public int? RowsWritten { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='WRITE [TOTAL]'.
        public bool IsMissingValues => !(StartTime.HasValue && EndTime.HasValue && Runtime.HasValue && RowsRead.HasValue && RowsWritten.HasValue);
        public double? PerformanceScore { get; set; }
        public double? ReconciliationScore { get; set; }

        private void UpdateReconciliationScore()
        {
            double OkCount = Reconciliations.Count(v => v.Status is ReconciliationStatus.Ok);
            double TotalCount = Reconciliations.Count(v => v.Status is not ReconciliationStatus.Disabled);
            ReconciliationScore = TotalCount > 0 ? OkCount / TotalCount * 100.0d : 100.0d;
        }

        public void UpdatePerformanceScore()
        {
            if (Runtime.HasValue && Runtime.Value.TotalSeconds > 0)
            {
                PerformanceScore = (RowsWritten ?? 0 / Runtime.Value.TotalSeconds) * 0.01;
            }
        }

        public void AddReconciliation(Reconciliation v)
        {
            Reconciliations.Add(v);
            UpdateReconciliationScore();
        }

        /// <summary>
        /// Adds performance readings to the manager, and ensures that entries are never added twice.
        /// </summary>
        /// <param name="cpuReadings">A list of CPU readings.</param>
        /// <param name="ramReadings">A list of RAM readings.</param>
        public void AddReadings(List<CpuLoad> cpuReadings, List<RamLoad> ramReadings)
        {
            if (CpuReadings.Any())
            {
                DateTime lastReading = CpuReadings.Last().Date;
                CpuReadings.AddRange(cpuReadings.Where(r => r.Date > lastReading));
            }
            else
            {
                CpuReadings.AddRange(cpuReadings);
            }
            if (RamReadings.Any())
            {
                DateTime lastReading = RamReadings.Last().Date;
                RamReadings.AddRange(ramReadings.Where(r => r.Date > lastReading));
            }
            else
            {
                RamReadings.AddRange(ramReadings);
            }
        }

        public override string ToString()
        {
            return $"Manager [{Name}], status [{Status}]";
        }

        public override bool Equals(object obj)
        {
            return obj is Manager other && other.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, ContextId);
        }
    }
}
