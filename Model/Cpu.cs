using System.Collections.ObjectModel;

namespace Model
{
    public class Cpu : ObservableObject
    {
        public Cpu(string name, int? cores, long? maxFreq)
        {
            Name = name;
            Cores = cores;
            MaxFrequency = maxFreq;
        }

        public string Name { get; }
        public int? Cores { get; }
        public long? MaxFrequency { get; } 
        private ObservableCollection<CpuLoad> _readings = new();
        public ObservableCollection<CpuLoad> Readings
        {
            get => _readings;
            set
            {
                _readings = value;
                OnPropertyChanged(nameof(Readings));
            }
        }

    public override string ToString()
        {
            return $"CPU: {Name}, {MaxFrequency} Hz, {Cores} cores";
        }
    }
}