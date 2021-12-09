using System.Collections.ObjectModel;

namespace Model
{
    public class Ram : ObservableObject
    {
        public Ram(long? total)
        {
            Total = total;
            Readings = new();
        }

        private long? _total; // bytes
        public long? Total
        {
            get => _total; set
            {
                _total = value;
                OnPropertyChanged(nameof(Total));
            }
        }
        //The property above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'CPU_INIT'.

        private ObservableCollection<RamLoad> _readings;
        public ObservableCollection<RamLoad> Readings
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
            return $"TOTAL MEMORY: {Total} bytes";
        }
    }
}
