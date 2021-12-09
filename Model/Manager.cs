using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Model
{
    public delegate void ManagerFinished();

    public enum ManagerStatus : byte
    {
        Ready, Running, Ok
    }

    public class Manager : ObservableObject
    {
        private static int _nextId = 1;

        public Manager()
        {
            Id = Interlocked.Increment(ref _nextId);
            Status = ManagerStatus.Ready;
            Validations.CollectionChanged += UpdateValidationCounters;
        }

        public event ManagerFinished OnManagerFinished;

        /// Unique identifier for the Manager in the Dashboard system. Do not confuse with Context ID, which is only unique within an execution.
        public int Id { get; }
        private WpfObservableRangeCollection<ValidationTest> _validations;
        public WpfObservableRangeCollection<ValidationTest> Validations
        {
            get => _validations; set
            {
                _validations = value;
                OnPropertyChanged(nameof(Validations));
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
        public int ValidationsDisabled
        {
            get => _validationsDisabled;
            set
            {
                _validationsDisabled = value;
                OnPropertyChanged(nameof(ValidationsDisabled));
            }
        }
        private int _validationsFailed;
        public int ValidationsFailed
        {
            get => _validationsFailed; 
            set
            {
                _validationsFailed = value;
                OnPropertyChanged(nameof(ValidationsFailed));
            }
        }
        private List<CpuLoad> _cpuReadings;
        public List<CpuLoad> CpuReadings
        {
            get => _cpuReadings; set
            {
                _cpuReadings = value;
                OnPropertyChanged(nameof(CpuReadings));
            }
        }
        private List<RamLoad> _ramReadings;
        public List<RamLoad> RamReadings
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
                ShortName = value.Split('.').Last();
            }
        }
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
            Score = ValidationsTotal > 0 ? ValidationsOk / (double)ValidationsTotal * 100.0d : 100.0d;
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

        public void AddValidation(IEnumerable<ValidationTest> v)
        {
            Validations.AddRange(v);
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

        private void UpdateValidationCounters(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in e.NewItems)
            {
                ValidationsTotal++;
                switch ((item as ValidationTest)?.Status)
                {
                    case ValidationStatus.Ok:
                        ValidationsOk++;
                        break;
                    case ValidationStatus.FailMismatch:
                    case ValidationStatus.Failed:
                        ValidationsFailed++;
                        break;
                    case ValidationStatus.Disabled:
                        ValidationsDisabled++;
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
