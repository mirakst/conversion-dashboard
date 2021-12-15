namespace Model
{
    public class Ram
    {
        public Ram()
        {

        }

        public Ram(long? total) : this()
        {
            Total = total;
        }

        public long? Total { get; set; } //bytes //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'TOTAL'.
        //The property above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'CPU_INIT'.
        public List<RamLoad> Readings { get; set; } = new();

        public void AddReading(RamLoad reading)
        {
            if ((reading.Load == 0 && reading.Available != 0) && Total.HasValue && Total.Value > 0)
            {
                reading.Load = 1 - (double)reading.Available / Total.Value;
            }
            Readings.Add(reading);
        }

        public void AddReadings(IEnumerable<RamLoad> readings)
        {
            foreach (var reading in readings)
            {
                AddReading(reading);
            }
        }

        public override string ToString()
        {
            return $"TOTAL MEMORY: {Total} bytes";
        }

        public override int GetHashCode()
        {
            return (Total == null ? 0 : Total.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is not Ram other)
                return false;

            return GetHashCode() == other.GetHashCode();
        }
    }
}
