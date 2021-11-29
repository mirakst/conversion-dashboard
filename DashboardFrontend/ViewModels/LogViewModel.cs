using Model;
using System.Collections.ObjectModel;

namespace DashboardFrontend.ViewModels
{
    public class LogViewModel : BaseViewModel
    {
        private ObservableCollection<LogMessage> _messages = new();
        public ObservableCollection<LogMessage> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

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
            }
        }

        /// <summary>
        /// Updates the actual data of the view-model, for use whenever a query has been executed and parsed
        /// </summary>
        public void UpdateData(Log log)
        {
            Messages = new(log.Messages);
            InfoCount = log.InfoCount;
            WarnCount = log.WarnCount;
            ErrorCount = log.ErrorCount;
            FatalCount = log.FatalCount;
            ValidationCount = log.ValidationCount;
        }
    }
}
