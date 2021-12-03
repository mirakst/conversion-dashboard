using Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
        public int ShownExecution { get; set; }
        public ListView LogListView { get; set; }
        public ObservableCollection<LogMessage> MessageList { get; private set; } = new();
        private CollectionView _messageView;
        public CollectionView MessageView
        {
            get => _messageView;
            set
            {
                _messageView = value;
                OnPropertyChanged(nameof(MessageView));
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
                MessageView?.Refresh();
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
                MessageView?.Refresh();
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
                MessageView?.Refresh();
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
                MessageView?.Refresh();
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
                MessageView?.Refresh();
            }
        }

        /// <summary>
        /// Updates the actual data of the view-model, for use whenever a query has been executed and parsed.
        /// </summary>
        public void UpdateData(Log log)
        {
            MessageList = new(log.Messages);
            MessageView = (CollectionView)CollectionViewSource.GetDefaultView(MessageList);
            MessageView.Filter = OnMessagesFilter;
            UpdateCounters(log);
            if (DoAutoScroll && LogListView is not null)
            {
                ScrollToLast(this, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Used as a filter for the MessageView CollectionView.
        /// </summary>
        /// <param name="item">A LogMessage object.</param>
        /// <returns>True if the object should be shown in the CollectionView, and false otherwise.</returns>
        private bool OnMessagesFilter(object item)
        {
            LogMessageType type = ((LogMessage)item).Type;
            return (ShowInfo && type.HasFlag(LogMessageType.Info))
                || (ShowWarn && type.HasFlag(LogMessageType.Warning))
                || (ShowError && type.HasFlag(LogMessageType.Error))
                || (ShowFatal && type.HasFlag(LogMessageType.Fatal))
                || (ShowValidation && type.HasFlag(LogMessageType.Validation));
        }

        /// <summary>
        /// Updates the number of log messages with the different possible types.
        /// </summary>
        /// <param name="log">The log to fetch updated counts from.</param>
        private void UpdateCounters(Log log)
        {
            InfoCount = log.InfoCount;
            WarnCount = log.WarnCount;
            ErrorCount = log.ErrorCount;
            FatalCount = log.FatalCount;
            ValidationCount = log.ValidationCount;
        }

        public void ScrollToLast(object sender, RoutedEventArgs e)
        {
            int itemCount = LogListView.Items.Count;
            if (itemCount > 0)
            {
                LogListView.ScrollIntoView(LogListView.Items[^1]);
            }
        }
    }
}
