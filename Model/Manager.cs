using System.Diagnostics.CodeAnalysis;

namespace Model
{
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
        public string ShortName { get; set; }
        public DateTime? StartTime { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'START_TIME'.
        public DateTime? EndTime { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'END_TIME'.
        public TimeSpan? Runtime { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES] for [MANAGER] = Name, where [KEY] = 'runtimeOverall'.
        public ManagerStatus Status { get; set; } //[STATUS] from [dbo].[MANAGER_TRACKING], where [MGR] = Name - until a manager start is logged, in which case it is RUNNING until a manager finishing is logged.
        public int? RowsRead { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='READ [TOTAL]'.
        public int? RowsWritten { get; set; } //Key, value pair from [dbo].[ENGINE_PROPERTIES], where [KEY]='WRITE [TOTAL]'.
        public double Score { get; set; }
        public bool IsMissingValues => !StartTime.HasValue || !EndTime.HasValue || !Runtime.HasValue || !RowsRead.HasValue || !RowsWritten.HasValue;
        #endregion

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
