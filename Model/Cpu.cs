namespace Model
{
    public class Cpu
    {
        #region Constructors
        public Cpu(string name, int cores, long maxFreq)
        {
            Name = name;
            Cores = cores;
            MaxFrequency = maxFreq;
        }
        #endregion Constructors

        #region Properties
        public string Name { get; } //From [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'CPU Name'.
        public int Cores { get; } //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'LogicalCores or PhysicalCores'. Cores is too abstract, and this should be discussed later!!
        public long MaxFrequency { get; } //Hz //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'CPU Max frequency'.
                                          //The properties above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'CPU_INIT'.
        private readonly List<CpuLoad> _readings = new();

        public List<CpuLoad> Readings
        {
            get => _readings;
            set => _readings.AddRange(value);
        }

        #endregion Properties

    public override string ToString()
        {
            return $"CPU NAME: {Name}\nCPU CORES: {Cores}\nCPU MAX FREQUENCY: {MaxFrequency} Hz";
        }
    }
}