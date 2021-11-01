namespace Model
{
    public class Network
    {
        public Network(string name, string macAddress, int speed)
        {
            Name = name;
            MacAddress = macAddress;
            Speed = speed;
        }

        public string Name { get; }
        public string MacAddress { get; } 
        public int Speed { get; } // bps
        public List<NetworkUsage> Readings { get; set; }
    }
}
