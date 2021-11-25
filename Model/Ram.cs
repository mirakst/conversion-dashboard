namespace Model
{
    public class Ram
    {
        #region Constructors
        public Ram(long total)
        {
            Total = total;
        }
        #endregion Constructors

        #region Properties
        public long Total { get; set; } //bytes //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'TOTAL'.
        //The property above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'CPU_INIT'.
        public DateTime LastPlot { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue; //Date of last modification.
        private readonly List<RamLoad> _readings = new();

        public List<RamLoad> Readings
        {
            get => _readings;
            set => _readings.AddRange(value);
        }
        #endregion Properties

        public override string ToString()
        {
            return $"TOTAL MEMORY: {Total} bytes";
        }
    }
}
