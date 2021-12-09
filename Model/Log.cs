using System.Collections.ObjectModel;

namespace Model
{
    public delegate void OnFilterChanged();

    public class Log : ObservableObject
    {
        public Log()
        {
            Messages = new();
            LastModified = SqlMinDateTime;
            ShowInfo = true;
            ShowWarn = true;
            ShowErrors = true;
            ShowFatal = true;
            ShowValidations = true;
        }

        public event OnFilterChanged OnFilterChanged;

        private SmartCollection<LogMessage> _messages;
        public SmartCollection<LogMessage> Messages
        {
            get => _messages;
            set
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
                OnFilterChanged?.Invoke();
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
                OnFilterChanged?.Invoke();
            }
        }
        private bool _showErrors;
        public bool ShowErrors
        {
            get => _showErrors;
            set
            {
                _showErrors = value;
                OnPropertyChanged(nameof(ShowErrors));
                OnFilterChanged?.Invoke();
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
                OnFilterChanged?.Invoke();
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
                OnFilterChanged?.Invoke();
            }
        }
        public int InfoCount => Messages.Count(m => m.Type.HasFlag(LogMessageType.Info));
        public int WarnCount => Messages.Count(m => m.Type.HasFlag(LogMessageType.Warning));
        public int ErrorCount => Messages.Count(m => m.Type.HasFlag(LogMessageType.Error));
        public int FatalCount => Messages.Count(m => m.Type.HasFlag(LogMessageType.Fatal));
        public int ValidationCount => Messages.Count(m => m.Type.HasFlag(LogMessageType.Validation));
    }
}
