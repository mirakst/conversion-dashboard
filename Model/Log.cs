using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Model
{
    public class Log : ObservableObject
    {
        public Log()
        {
            Messages = new();
            LastModified = SqlMinDateTime;
        }

        public WpfObservableRangeCollection<LogMessage> Messages { get; }
        private DateTime _lastModified;
        public DateTime LastModified
        {
            get => _lastModified; set
            {
                _lastModified = value;
                OnPropertyChanged(nameof(LastModified));
            }
        }

        private int _infoCount;
        private int _warnCount;
        private int _errorCount;
        private int _fatalCount;
        private int _validationCount;
        public int InfoCount
        {
            get => _infoCount;
            set
            {
                _infoCount = value;
                OnPropertyChanged(nameof(InfoCount));
            }
        }
        public int WarnCount
        {
            get => _warnCount; 
            set
            {
                _warnCount = value;
                OnPropertyChanged(nameof(WarnCount));
            }
        }
        public int ErrorCount
        {
            get => _errorCount; 
            set
            {
                _errorCount = value;
                OnPropertyChanged(nameof(ErrorCount));
            }
        }
        public int FatalCount
        {
            get => _fatalCount; 
            set
            {
                _fatalCount = value;
                OnPropertyChanged(nameof(FatalCount));
            }
        }
        public int ValidationCount
        {
            get => _validationCount; 
            set
            {
                _validationCount = value;
                OnPropertyChanged(nameof(ValidationCount));
            }
        }
    }
}
