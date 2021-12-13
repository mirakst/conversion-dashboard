﻿namespace Model
{
    public class Network
    {
        public Network()
        {

        }

        public Network(string name, string macAddress, long? speed)
            : this()
        {
            Name = name;
            MacAddress = macAddress;
            Speed = speed;
        }

        public string Name { get; set; } //From [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Interface 0: Name'.
        public string MacAddress { get; set; } //From [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Interface 0: MAC address'.
        public long? Speed { get; set; } //bps //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Interface 0: Speed'.
        //The properties above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'NETWORK_INIT'.
        private readonly List<NetworkUsage> _readings = new();

        public List<NetworkUsage> Readings
        {
            get => _readings;
            set => _readings.AddRange(value);
        }

        public override string ToString()
        {
            return $"ADAPTER NAME: {Name}\nMAC ADDRESS: {MacAddress}\nSPEED: {Speed} bps";
        }
    }
}
