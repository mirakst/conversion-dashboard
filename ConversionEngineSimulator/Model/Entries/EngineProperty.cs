using System;

namespace ConversionEngineSimulator
{
    public class EngineProperty : ITimestampedDatabaseEntry, IComparable<EngineProperty>
    {
        public string MANAGER { get; set; }
        public string KEY { get; set; }
        public string VALUE { get; set; }
        public DateTime TIMESTAMP { get; set; }
        public DateTime CREATED { get => TIMESTAMP; set => TIMESTAMP = value; }

        public int RUN_NO { get; set; }
        public override string ToString()
        {
            return $"{"[ENG Property]",-15} {CREATED}: {MANAGER} {KEY} {VALUE}";
        }
        public int CompareTo(EngineProperty other)
        {
            return CREATED.CompareTo(other.CREATED);
        }
    }
}
