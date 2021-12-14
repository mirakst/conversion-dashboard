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

        public HealthReport(string hostName, string monitorName)
        {
            HostName = hostName;
            MonitorName = monitorName;
        }

        public HealthReport(string hostName, string monitorName, Cpu cpu, Network network, Ram ram) 
            : this(hostName, monitorName)
        {
            Cpu = cpu;
            Ram = ram;
            Network = network;
        }

        public string HostName { get; set; } //Key, value pair from [REPORT_KEY], [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Hostname'.
        public string MonitorName { get; set; } //Key, value pair from [REPORT_KEY], [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Monitor Name'.
        //The properties above can be found in the collection of entries from [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'INIT'.
        public Cpu Cpu { get; set; } //Described in Cpu.cs
        public Network Network { get; set; } //Described in Network.cs
        public Ram Ram { get; set; } //Described in Ram.cs
        public DateTime LastModified { get; set; } //Date of last modification.

        public override string ToString()
        {
            return $"SYSTEM INFO:\nHOSTNAME: {HostName}\nMONITOR NAME: {MonitorName}\n"+
                   $"CPU: {Cpu}\nNETWORK: {Network}\nRAM: {Ram}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(HostName == null ? 0 : HostName.GetHashCode(),
                                    MonitorName == null ? 0 : MonitorName.GetHashCode(),
                                    Cpu == null ? 0 : Cpu.GetHashCode(),
                                    Network == null ? 0 : Network.GetHashCode(),
                                    Ram == null ? 0 : Ram.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is not HealthReport other)
            {
                return false;
            }

            bool tempCpu = Cpu.Readings == null && other.Cpu.Readings == null ? true : Cpu.Readings.SequenceEqual(other.Cpu.Readings);
            bool tempRam = Ram.Readings == null && other.Ram.Readings == null ? true : Ram.Readings.SequenceEqual(other.Ram.Readings);
            bool tempNetwork = Network.Readings == null || other.Network.Readings == null ? true : Network.Readings.SequenceEqual(other.Network.Readings);

            return GetHashCode() == other.GetHashCode()
                   && tempCpu
                   && tempRam
                   && tempNetwork;
        }
    }
}
