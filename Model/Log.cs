using System.Collections.ObjectModel;

namespace Model
{
    public class Log : BaseViewModel
    {
        public Log()
        {
            LastModified = SqlMinDateTime;
        }

        private ObservableCollection<LogMessage> _messages;
        public ObservableCollection<LogMessage> Messages
        {
            get => _messages; set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }
        private DateTime _lastModified;
        public DateTime LastModified
        {
            get => _lastModified; set
            {
                _lastModified = value;
                OnPropertyChanged(nameof(LastModified));
            }
        }
        private bool _showInfo;
        public bool ShowInfo
        {
            get => _showInfo; set
            {
                _showInfo = value;
                OnPropertyChanged(nameof(ShowInfo));
            }
        }
        private bool _showWarn;
        public bool ShowWarn
        {
            get => _showWarn;
            set
            {
                _showWarn = value;
                OnPropertyChanged(nameof(ShowWarn));
            }
        }
        private bool _showFailed;
        public bool ShowFailed
        {
            get => _showFailed;
            set
            {
                _showFailed = value;
                OnPropertyChanged(nameof(ShowFailed));
            }
        }
        private bool _showFatal;
        public bool ShowFatal
        {
            get => _showFatal;
            set
            {
                _showFatal = value;
                OnPropertyChanged(nameof(ShowFatal));
            }
        }
        private bool _showValidations;
        public bool ShowValidations
        {
            get => _showValidations;
            set
            {
                _showValidations = value;
                OnPropertyChanged(nameof(ShowValidations));
            }
        }
        public int WarnCount => Messages.Count(m => m.Type.HasFlag(LogMessageType.Warning));
        public int ErrorCount => Messages.Count(m => m.Type.HasFlag(LogMessageType.Error));
        public int FatalCount => Messages.Count(m => m.Type.HasFlag(LogMessageType.Fatal));
        public int ValidationCount => Messages.Count(m => m.Type.HasFlag(LogMessageType.Validation));
    }
}
