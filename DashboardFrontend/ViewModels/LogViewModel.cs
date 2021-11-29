using DashboardBackend;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using static Model.LogMessage;

namespace DashboardFrontend.ViewModels
{
    public class LogViewModel : BaseViewModel
    {
        public LogViewModel()
        {
        }

        public LogViewModel(ListView logListView)
        {
            LogListView = logListView;
        }
        
        public bool DoAutoScroll { get; set; } = true;
        public ListView LogListView { get; set; }
        private ObservableCollection<LogMessage> _messages = new();
        public ObservableCollection<LogMessage> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
                if (DoAutoScroll && LogListView is not null && LogListView.Items.Count > 0)
                {
                    LogListView.ScrollIntoView(LogListView.Items[^1]);
                }
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
                UpdateData();
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
                UpdateData();
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
                UpdateData();
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
                UpdateData();
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
                UpdateData();
            }
        }

        public ListView LogListView { get; }

        /// <summary>
        /// Updates the actual data of the view-model, for use whenever a query has been executed and parsed.
        /// </summary>
        public void UpdateData(Log log)
        {
            Messages = new(log.Messages.Where(x =>
            {
                return x.Type == LogMessageType.Info && ShowInfo
                    || x.Type == LogMessageType.Warning && ShowWarn
                    || x.Type == LogMessageType.Error && ShowError
                    || x.Type == LogMessageType.Fatal && ShowFatal
                    || x.Type == LogMessageType.Validation && ShowValidation;
            }));
            UpdateCounters(log);
        }

        /// <summary>
        /// Increments the counter property that corresponds to the LogMessageType of the given LogMessage.
        /// </summary>
        /// <param name="msg">LogMessage whose type counter should be updated.</param>
        private void UpdateCounters(Log log)
        {
            InfoCount = 0;
            WarnCount = 0;
            ErrorCount = 0;
            FatalCount = 0;
            ValidationCount = 0;
            foreach(LogMessage msg in log.Messages)
            {
                switch (msg.Type)
                {
                    case LogMessageType.Info:
                        InfoCount++;
                        break;
                    case LogMessageType.Warning:
                        WarnCount++;
                        break;
                    case LogMessageType.Error:
                        ErrorCount++;
                        break;
                    case LogMessageType.Fatal:
                        FatalCount++;
                        break;
                    case LogMessageType.Validation:
                        ValidationCount++;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
