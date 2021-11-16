using DashboardBackend;
using Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using static Model.LogMessage;

namespace DashboardFrontend.ViewModels
{
    public class LogViewModel : INotifyPropertyChanged
    {
        public LogViewModel(Log log)
        {
            _log = log;
            log.Messages = DataUtilities.GetLogMessages().Take(500).ToList();
            _allMessages = new(log.Messages);
            UpdateData();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<LogMessage> _allMessages;
        private Log _log;

        public ObservableCollection<LogMessage> Messages { get; set; } = new();

        private int _infoCount;
        public int InfoCount
        {
            get => _infoCount;
            set
            {
                _infoCount = value;
                OnPropertyChanged(nameof(InfoCount));
            }
        }
        private int _warnCount;
        public int WarnCount
        {
            get => _warnCount;
            set
            {
                _warnCount = value;
                OnPropertyChanged(nameof(WarnCount));
            }
        }
        private int _errorCount;
        public int ErrorCount
        {
            get => _errorCount;
            set
            {
                _errorCount = value;
                OnPropertyChanged(nameof(ErrorCount));
            }
        }
        private int _fatalCount;
        public int FatalCount
        {
            get => _fatalCount;
            set
            {
                _fatalCount = value;
                OnPropertyChanged(nameof(FatalCount));
            }
        }
        private int _validationCount;
        public int ValidationCount
        {
            get => _validationCount;
            set
            {
                _validationCount = value;
                OnPropertyChanged(nameof(ValidationCount));
            }
        }
        private bool _showInfo = true;
        public bool ShowInfo
        {
            get => _showInfo;
            set
            {
                _showInfo = value;
                OnPropertyChanged(nameof(ShowInfo));
                Refresh();
            }
        }
        private bool _showWarn = true;
        public bool ShowWarn
        {
            get => _showWarn;
            set
            {
                _showWarn = value;
                OnPropertyChanged(nameof(ShowWarn));
                Refresh();
            }
        }
        private bool _showError = true;
        public bool ShowError
        {
            get => _showError;
            set
            {
                _showError = value;
                OnPropertyChanged(nameof(ShowError));
                Refresh();
            }
        }
        private bool _showFatal = true;
        public bool ShowFatal
        {
            get => _showFatal;
            set
            {
                _showFatal = value;
                OnPropertyChanged(nameof(ShowFatal));
                Refresh();
            }
        }
        private bool _showValidation = true;
        public bool ShowValidation
        {
            get => _showValidation;
            set
            {
                _showValidation = value;
                OnPropertyChanged(nameof(ShowValidation));
                Refresh();
            }
        }

        /// <summary>
        /// Updates the actual data of the view-model, for use whenever a query has been executed and parsed
        /// </summary>
        public void UpdateData()
        {
            _allMessages = new(_log.Messages);
            Refresh();
        }

        /// <summary>
        /// Clears the log and iteratively adds messages to it depending on the selected filters
        /// </summary>
        private void Refresh()
        {
            Messages.Clear();
            InfoCount = 0;
            WarnCount = 0;
            ErrorCount = 0;
            FatalCount = 0;
            ValidationCount = 0;

            List<LogMessage> list = new();

            foreach (LogMessage msg in _allMessages)
            {
                UpdateCounter(msg);
                if (ShouldAddMessage(msg))
                {
                    list.Add(msg);
                }
            }
            Messages = new(list);

        }

        /// <summary>
        /// Increments the counter property that corresponds to the LogMessageType of the given LogMessage
        /// </summary>
        /// <param name="msg">LogMessage whose type counter should be updated</param>
        private void UpdateCounter(LogMessage msg)
        {
            switch (msg.Type)
            {
                case LogMessageType.INFO:
                    InfoCount++;
                    break;
                case LogMessageType.WARNING:
                    WarnCount++;
                    break;
                case LogMessageType.ERROR:
                    ErrorCount++;
                    break;
                case LogMessageType.FATAL:
                    FatalCount++;
                    break;
                case LogMessageType.VALIDATION:
                    ValidationCount++;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Determines whether the given LogMessage should be displayed in the UI log
        /// </summary>
        /// <param name="msg">The message whose type should be considered</param>
        /// <returns>True if the message type is enabled in the filter</returns>
        private bool ShouldAddMessage(LogMessage msg)
        {
            return
                (ShowInfo && msg.Type == LogMessageType.INFO) ||
                (ShowWarn && msg.Type == LogMessageType.WARNING) ||
                (ShowError && msg.Type == LogMessageType.ERROR) ||
                (ShowFatal && msg.Type == LogMessageType.FATAL) ||
                (ShowValidation && msg.Type == LogMessageType.VALIDATION);
        }

        /// <summary>
        /// Invokes the PropertyChanged event which triggers a change in the UI
        /// </summary>
        /// <param name="property">Name of the property which has changed</param>
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
