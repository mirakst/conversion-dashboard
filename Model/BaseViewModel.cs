using System.ComponentModel;

namespace Model
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected DateTime SqlMinDateTime => (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
