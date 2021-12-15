namespace Model
{
    public class Cpu
    {
        public Cpu()        {        }

        public Cpu(string name, int? cores, long? maxFreq)
        {
            Name = name;
            Cores = cores;
            MaxFrequency = maxFreq;
        }

        public string Name { get; set; } //From [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'CPU Name'.
        public int? Cores { get; set; } //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'LogicalCores or PhysicalCores'. Cores is too abstract, and this should be discussed later!!
        public long? MaxFrequency { get; set; } //Hz //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'CPU Max frequency'.
        //The properties above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'CPU_INIT'.
        public List<CpuLoad> Readings { get; set; } = new();

        public override string ToString()
        {
            return $"CPU NAME: {Name}\nCPU CORES: {Cores}\nCPU MAX FREQUENCY: {MaxFrequency} Hz";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name == null ? 0 : Name.GetHashCode(),
                                    Cores == null ? 0 : Cores.GetHashCode(),
                                    MaxFrequency == null ? 0 : MaxFrequency.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is not Cpu other)
                return false;

            return GetHashCode() == other.GetHashCode();
        }
    }
}