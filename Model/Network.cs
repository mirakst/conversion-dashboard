namespace Model
{
    public class Network
    {
        #region Constructors
        public Network(string name, string macAddress, long speed)
        {
            Name = name;
            MacAddress = macAddress;
            Speed = speed;
        }
        #endregion Constructors

        #region Properties
        public string Name { get; } //From [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Interface 0: Name'.
        public string MacAddress { get; } //From [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Interface 0: MAC address'.
        public long Speed { get; } //bps //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Interface 0: Speed'.
        //The properties above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'NETWORK_INIT'.
        public DateTime LastPlot { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue; //Date of last modification.
        private readonly List<NetworkUsage> _readings = new();

        public List<NetworkUsage> Readings
        {
            get => _readings;
            set => _readings.AddRange(value);
        }
        #endregion Properties

        public override string ToString()
        {
            return $"ADAPTER NAME: {Name}\nMAC ADDRESS: {MacAddress}\nSPEED: {Speed} bps";
        }
    }
}
