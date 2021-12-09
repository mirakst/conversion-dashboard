using System.Collections.ObjectModel;

namespace Model
{
    public class Network : ObservableObject
    {
        public Network(string name, string macAddress, long? speed)
        {
            Name = name;
            MacAddress = macAddress;
            Speed = speed;
        }

        public string Name { get; } //From [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Interface 0: Name'.
        public string MacAddress { get; } //From [REPORT_STRING_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Interface 0: MAC address'.
        public long? Speed { get; } //bps //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'Interface 0: Speed'.
        //The properties above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'NETWORK_INIT'.
        private ObservableCollection<NetworkUsage> _readings;
        public ObservableCollection<NetworkUsage> Readings
        {
            get => _readings;
            set
            {
                _readings = value;
                OnPropertyChanged(nameof(Readings));
            }
        }

        public override string ToString()
        {
            return $"ADAPTER NAME: {Name}\nMAC ADDRESS: {MacAddress}\nSPEED: {Speed} bps";
        }
    }
}
