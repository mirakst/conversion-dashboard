using System.Diagnostics;

namespace Model
{
    public class HealthReport 
    {
        #region Constructors

        public HealthReport()
        {

        }
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
        public string HostName { get; private set; } //Key, value pair from [REPORT_KEY], [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Hostname'.
        public string MonitorName { get; private set; } //Key, value pair from [REPORT_KEY], [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Monitor Name'.
        //The properties above can be found in the collection of entries from [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'INIT'.
        public Cpu Cpu { get; private set; } //Described in Cpu.cs
        public Network Network { get; private set; } //Described in Network.cs
        public Ram Ram { get; private set; } //Described in Ram.cs
        public bool IsInitialized { get; private set; } //If the health report has been built.
        public DateTime LastModified { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue; //Date of last modification.
        #endregion Properties

        public void Build(string hostName, string monitorName, Cpu cpu, Network network, Ram ram)
        {
            HostName = hostName;
            MonitorName = monitorName;
            Cpu = cpu;
            Network = network;
            Ram = ram;
            if (ram.Total is not null)
            {
                IsInitialized = true;
            }
        }
        public override string ToString()
        {
            return $"SYSTEM INFO:\nHOSTNAME: {HostName}\nMONITOR NAME: {MonitorName}\n"+
                   $"CPU: {Cpu}\nNETWORK: {Network}\nRAM: {Ram}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(HostName == null ? 0 : HostName.GetHashCode(),
                                    MonitorName == null ? 0 : MonitorName.GetHashCode(),
                                    Cpu.GetHashCode(),
                                    Network.GetHashCode(),
                                    Ram.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is not HealthReport other)
            {
                return false;
            }

            return GetHashCode() == other.GetHashCode()
                   && Cpu.Readings.SequenceEqual(other.Cpu.Readings)
                   && Network.Readings.SequenceEqual(other.Network.Readings)
                   && Ram.Readings.SequenceEqual(other.Ram.Readings);
        }
    }
}
