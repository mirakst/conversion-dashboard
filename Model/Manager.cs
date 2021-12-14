using static Model.ValidationTest;

namespace Model
{
    public delegate void ManagerFinished(Manager manager);

    public class Manager
    {
        #region Constructors
        public Manager()
        {
            Status = ManagerStatus.Ready;
        }
        #endregion

        #region Enums
        public enum ManagerStatus : byte
        {
            Ready, Running, Ok
        }
        #endregion Enums

        public event ManagerFinished OnManagerFinished;

        #region Properties     
        public List<ValidationTest> Validations { get; set; } = new();
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
        } //[MANAGER_NAME] from [dbo].[MANAGERS]
        public int ContextId { get; set; }
        public string ShortName { get; set; }
        public DateTime? StartTime { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'START_TIME'.
        public DateTime? EndTime { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'END_TIME'.
        public TimeSpan? Runtime { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'runtimeOverall'.
        private ManagerStatus _status;
        public ManagerStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                if (value is ManagerStatus.Ok)
                {
                    OnManagerFinished?.Invoke(this);
                    UpdateValidationScore();
                    UpdatePerformanceScore();
                }
            }
        }
        public int? RowsRead { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='READ [TOTAL]'.
        public int? RowsWritten { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='WRITE [TOTAL]'.
        public double? PerformanceScore { get; set; }
        public double? ValidationScore { get; set; }
        public bool IsMissingValues => !StartTime.HasValue || !EndTime.HasValue || !Runtime.HasValue || !RowsRead.HasValue || !RowsWritten.HasValue;
        #endregion

        private void UpdateValidationScore()
        {
            double OkCount = Validations.Count(v => v.Status is ValidationStatus.Ok);
            double TotalCount = Validations.Count(v => v.Status is not ValidationStatus.Disabled);
            ValidationScore = TotalCount > 0 ? OkCount / TotalCount * 100.0d : 100.0d;
        }

        public void UpdatePerformanceScore()
        {
            if (Runtime.HasValue && Runtime.Value.TotalSeconds > 0)
            {
                PerformanceScore = (RowsWritten ?? 0 / Runtime.Value.TotalSeconds) * 0.01;
            }
        }

        public void AddValidation(ValidationTest v)
        {
            Validations.Add(v);
            UpdateValidationScore();
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
