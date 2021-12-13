namespace Model
{
    public class HealthReport 
    {
        public HealthReport()
        {
            LastModified = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            Cpu = new();
            Network = new();
            Ram = new();
        }

        public string HostName { get; set; } //Key, value pair from [REPORT_KEY], [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Hostname'.
        public string MonitorName { get; set; } //Key, value pair from [REPORT_KEY], [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Monitor Name'.
        //The properties above can be found in the collection of entries from [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'INIT'.
        public Cpu Cpu { get; set; } //Described in Cpu.cs
        public Network Network { get; set; } //Described in Network.cs
        public Ram Ram { get; set; } //Described in Ram.cs
        public bool IsInitialized => Ram?.Total is null; //If the health report has been built.
        public DateTime LastModified { get; set; } //Date of last modification.

        public override string ToString()
        {
            return $"SYSTEM INFO:\nHOSTNAME: {HostName}\nMONITOR NAME: {MonitorName}\n"+
                   $"CPU: {Cpu}\nNETWORK: {Network}\nRAM: {Ram}";
        }
    }
}
