namespace Model
{
    public class Cpu
    {
        public Cpu(string name, int cores, int maxFreq)
        {
            Name = name;
            Cores = cores;
            MaxFrequency = maxFreq;
        }

        public string Name { get; }
        public int Cores { get; }
        public int MaxFrequency { get; }
        public List<CpuLoad> Readings { get; set; } = new();
    }
}
