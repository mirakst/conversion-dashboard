namespace Model
{
    public class HealthReport 
    {
        #region Constructors
        public HealthReport(string hostName, string monitorName)
        {
            HostName = hostName;
            MonitorName = monitorName;
        }
        public HealthReport(string hostName, string monitorName, Cpu cpu, Network network, Ram ram) : this(hostName, monitorName)
        {
            Cpu = cpu;
            Network = network;
            Ram = ram;
        }
        #endregion Constructors

        #region Properties
        public string HostName { get; } //Key, value pair from [REPORT_KEY], [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Hostname'.
        public string MonitorName { get; } //Key, value pair from [REPORT_KEY], [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Monitor Name'.
        //The properties above can be found in the collection of entries from [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'INIT'.
        public Cpu Cpu { get; } //Described in Cpu.cs
        public Network Network {  get; } //Described in Network.cs
        public Ram Ram { get; } //Described in Ram.cs
        public DateTime LastModified { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue; //Date of last modification.
        #endregion Properties

        public override string ToString()
        {
            return $"SYSTEM INFO:\nHOSTNAME: {HostName}\nMONITOR NAME: {MonitorName}\n"+
                   $"CPU: {Cpu}\nNETWORK: {Network}\nRAM: {Ram}";
        }
    }
}
