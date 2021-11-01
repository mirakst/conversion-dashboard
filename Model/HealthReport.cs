namespace Model
{
    public class HealthReport 
    {
        public HealthReport(string hostName, string monitorName)
        {
            HostName = hostName;
            MonitorName = monitorName;
        }

        public string HostName { get; private set; }
        public string MonitorName { get; private set; }
        public Cpu Cpu { get; private set; }
        public Network Network {  get; private set; }
        public Ram Ram { get; private set; }
    }
}
