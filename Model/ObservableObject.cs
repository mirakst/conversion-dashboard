using System.ComponentModel;

namespace Model
{
    public class ObservableObject : INotifyPropertyChanged
    {
        protected DateTime SqlMinDateTime => (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
