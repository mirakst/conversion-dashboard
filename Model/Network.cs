namespace Model
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
        public List<NetworkUsage> Readings { get; set; } = new();

        public override string ToString()
        {
            return $"ADAPTER NAME: {Name}\nMAC ADDRESS: {MacAddress}\nSPEED: {Speed} bps";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name == null ? 0 : Name.GetHashCode(),
                                    MacAddress == null ? 0 : MacAddress.GetHashCode(),
                                    Speed == null ? 0 : Speed.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is not Network other)
                return false;

            return GetHashCode() == other.GetHashCode();
        }
    }
}
