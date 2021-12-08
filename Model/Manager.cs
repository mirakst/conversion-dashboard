using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using static Model.ValidationTest;

namespace Model
{
    public delegate void ManagerFinished();

    public enum ManagerStatus : byte
    {
        Ready, Running, Ok
    }

    public class Manager : BaseViewModel
    {
        public Manager()
        {
            Status = ManagerStatus.Ready;
        }

        public event ManagerFinished OnManagerFinished;

        public ObservableCollection<ValidationTest> Validations
        {
            get => _validations; set
            {
                _validations = value;
                OnPropertyChanged(nameof(Validations));
            }
        }
        public ObservableCollection<CpuLoad> CpuReadings
        {
            get => _cpuReadings; set
            {
                _cpuReadings = value;
                OnPropertyChanged(nameof(CpuReadings));
            }
        }
        public ObservableCollection<RamLoad> RamReadings
        {
            get => _ramReadings; set
            {
                _ramReadings = value;
                OnPropertyChanged(nameof(RamReadings));
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                var splitName = value.Split('.');
                if (splitName.Contains("managers"))
                {
                    ShortName = "(...)" + string.Join(".", splitName.TakeLast(2));
                }
                else
                {
                    ShortName = "(...)" + string.Join(".", splitName.Skip(4));
                }
            }
        } //[MANAGER_NAME] from [dbo].[MANAGERS]
        public int ContextId { get; set; }

        private string _shortName;
        public string ShortName
        {
            get => _shortName;
            set
            {
                _shortName = value;
                OnPropertyChanged(nameof(ShortName));
            }
        }
        public DateTime? StartTime { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'START_TIME'.
        public DateTime? EndTime { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'END_TIME'.
        public TimeSpan? Runtime { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'runtimeOverall'.
        private ManagerStatus _status;
        private ObservableCollection<ValidationTest> _validations;
        private ObservableCollection<CpuLoad> _cpuReadings;
        private ObservableCollection<RamLoad> _ramReadings;

        public ManagerStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                if (value is ManagerStatus.Ok)
                {
                    OnManagerFinished?.Invoke();
                    UpdateScore();
                }
            }
        }
        public int? RowsRead { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='READ [TOTAL]'.
        public int? RowsWritten { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='WRITE [TOTAL]'.
        public double? Score { get; set; }
        public bool IsMissingValues => !StartTime.HasValue || !EndTime.HasValue || !Runtime.HasValue || !RowsRead.HasValue || !RowsWritten.HasValue;
#endregion

        private void UpdateScore()
        {
            int OkCount = Validations.Count(v => v.Status is ValidationStatus.Ok);
            int TotalCount = Validations.Count(v => v.Status is not ValidationStatus.Disabled);
            Score = TotalCount > 0 ? (double)OkCount / (double)TotalCount * 100.0d : 100.0d;
        }

        public void AddValidation(ValidationTest v)
        {
            Validations.Add(v);
            UpdateScore();
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
