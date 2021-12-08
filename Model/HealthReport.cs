namespace Model
{
    public class HealthReport : BaseViewModel
    {
        public HealthReport()
        {
            LastModified = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
        }

        public string HostName { get; private set; } //Key, value pair from [REPORT_KEY], [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Hostname'.
        public string MonitorName { get; private set; } //Key, value pair from [REPORT_KEY], [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Monitor Name'.
        //The properties above can be found in the collection of entries from [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'INIT'.
        public Cpu Cpu { get; private set; } //Described in Cpu.cs
        public Network Network { get; private set; } //Described in Network.cs
        public Ram Ram { get; private set; } //Described in Ram.cs
        public bool IsInitialized { get; private set; } //If the health report has been built.
        private DateTime _lastModified;
        public DateTime LastModified
        {
            get => _lastModified; set
            {
                _lastModified = value;
                OnPropertyChanged(nameof(LastModified));
            }
        }

        /// <summary>
        /// Assigns the properties in the Health Report. Since RAM usage is converted to a percentage, the Health Report is not considered initialized until the property <see cref="Ram.Total"/> is set.
        /// </summary>
        /// <param name="hostName">Name of the machine hosting the Conversion.</param>
        /// <param name="monitorName"></param>
        /// <param name="cpu">The CPU object containing information on the CPU of the host machine.</param>
        /// <param name="network">The Network object containing information on the Network of the host machine.</param>
        /// <param name="ram">The RAM object containing information on the RAM component of the host machine.</param>
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
            LastModified = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Host: {HostName}, Monitor: {MonitorName}";
        }
    }
}
