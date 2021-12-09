using System.Collections.ObjectModel;

namespace Model
{
    public delegate void ManagerFinished();

    public enum ManagerStatus : byte
    {
        Ready, Running, Ok
    }

    public class Manager : ObservableObject
    {
        public Manager()
        {
            Status = ManagerStatus.Ready;
        }

        public event ManagerFinished OnManagerFinished;

        private ObservableCollection<ValidationTest> _validations;
        public ObservableCollection<ValidationTest> Validations
        {
            get => _validations; set
            {
                _validations = value;
                OnPropertyChanged(nameof(Validations));
            }
        }
        private ObservableCollection<CpuLoad> _cpuReadings;
        public ObservableCollection<CpuLoad> CpuReadings
        {
            get => _cpuReadings; set
            {
                _cpuReadings = value;
                OnPropertyChanged(nameof(CpuReadings));
            }
        }
        private ObservableCollection<RamLoad> _ramReadings;
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
                OnPropertyChanged(nameof(Name));
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
        private int _contextId;
        public int ContextId
        {
            get => _contextId;
            set
            {
                _contextId = value;
                OnPropertyChanged(nameof(ContextId));
            }
        }

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
        private DateTime? _startTime;
        public DateTime? StartTime
        {
            get => _startTime; set
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
            get => _endTime; set
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
        private ManagerStatus _status;
        public ManagerStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
                if (value is ManagerStatus.Ok)
                {
                    OnManagerFinished?.Invoke();
                    UpdateScore();
                }
                if (StartTime.HasValue && EndTime.HasValue)
                {
                    Runtime = EndTime.Value.Subtract(StartTime.Value);
                }
            }
        }
        private int? _rowsRead;
        public int? RowsRead
        {
            get => _rowsRead; set
            {
                _rowsRead = value;
                OnPropertyChanged(nameof(RowsRead));
            }
        }
        private int? _rowsWritten;
        public int? RowsWritten
        {
            get => _rowsWritten; set
            {
                _rowsWritten = value;
                OnPropertyChanged(nameof(RowsWritten));
            }
        }
        private double? _score;
        public double? Score
        {
            get => _score; set
            {
                _score = value;
                OnPropertyChanged(nameof(Score));
            }
        }
        public bool IsMissingValues => !(StartTime.HasValue && EndTime.HasValue && Runtime.HasValue && RowsRead.HasValue && RowsWritten.HasValue);

        private void UpdateScore()
        {
            int OkCount = Validations.Count(v => v.Status is ValidationStatus.Ok);
            int TotalCount = Validations.Count(v => v.Status is not ValidationStatus.Disabled);
            Score = TotalCount > 0 ? (double)OkCount / (double)TotalCount * 100.0d : 100.0d;
        }

        /// <summary>
        /// Adds a validation test to the manager and updates its score.
        /// </summary>
        /// <param name="v">The validation test to add.</param>
        public void AddValidation(ValidationTest v)
        {
            Validations.Add(v);
            UpdateScore();
        }

        /// <summary>
        /// Adds performance readings to the manager as long as they are not older than the latest entry in their respective list.
        /// </summary>
        /// <param name="cpuReadings">A list of CPU readings.</param>
        /// <param name="ramReadings">A list of RAM readings.</param>
        public void AddReadings(IList<CpuLoad> cpuReadings, IList<RamLoad> ramReadings)
        {
            DateTime minDate = CpuReadings.Last()?.Date ?? DateTime.MinValue;
            foreach (var reading in cpuReadings.ToList())
            {
                if (reading.Date > minDate)
                {
                    CpuReadings.Add(reading);
                }
            }
            minDate = RamReadings.Last()?.Date ?? DateTime.MinValue;
            foreach (var reading in ramReadings.ToList())
            {
                if (reading.Date > minDate)
                {
                    RamReadings.Add(reading);
                }
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
