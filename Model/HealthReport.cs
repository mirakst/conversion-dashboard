namespace Model
{
    public class HealthReport 
    {
        public HealthReport(string hostName, string monitorName)
        {
            HostName = hostName;
            MonitorName = monitorName;
        }

        public string HostName { get; private set; } //Key, value pair from [REPORT_KEY], [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Hostname'.
        public string MonitorName { get; private set; } //Key, value pair from [REPORT_KEY], [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Monitor Name'.
        //The properties above can be found in the collection of entries from [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'INIT'.
        public Cpu Cpu { get; private set; } //Described in Cpu.cs
        public Network Network {  get; private set; } //Described in Network.cs
        public Ram Ram { get; private set; } //Described in Ram.cs
    }
}
