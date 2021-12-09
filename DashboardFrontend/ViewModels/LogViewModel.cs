using System;
using System.Collections.Generic;
using Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using static Model.LogMessage;
using ListView = System.Windows.Controls.ListView;

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

        public DateTime LastUpdated { get; set; }
        public bool DoAutoScroll { get; set; } = true;
        public ListView LogListView { get; set; }

        private ObservableCollection<ExecutionObservable> _executions = new();
        public ObservableCollection<ExecutionObservable> Executions
        {
            get => _executions;
            set
            {
                _executions = value;
                OnPropertyChanged(nameof(Executions));
            }
        }
        private ExecutionObservable? _selectedExecution;
        public ExecutionObservable? SelectedExecution
        {
            get => _selectedExecution;
            set
            {
                _selectedExecution = value;
                OnPropertyChanged(nameof(SelectedExecution));
                SetExecution(value);
            }
        }
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
                ScrollToLast();
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
                ScrollToLast();
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
                ScrollToLast();
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
                ScrollToLast();
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
                ScrollToLast();
            }
        }

        public void UpdateData(List<Execution> executions)
        {
            Executions = new(executions.Select(e => new ExecutionObservable(e)));

            if (SelectedExecution is null)
            {
                SelectedExecution = Executions.Last();
            }
            UpdateCounters(SelectedExecution);
            MessageView.Filter = OnMessagesFilter;
            ScrollToLast();
        }

        /// <summary>
        /// Used as a filter for the MessageView CollectionView.
        /// </summary>
        /// <param name="item">A LogMessage object.</param>
        /// <returns>True if the object should be shown in the CollectionView, and false otherwise.</returns>
        private bool OnMessagesFilter(object item)
        {
            LogMessageType type = ((LogMessage)item).Type;
            int ContextId = ((LogMessage) item).ContextId;
            if (ContextId > 0 && !SelectedExecution.Managers.Where(m => m.IsChecked).Any(m => m.ContextId == ContextId))
            {
                return false;
            }
            return ((ShowInfo && type.HasFlag(LogMessageType.Info))
                 || (ShowWarn && type.HasFlag(LogMessageType.Warning))
                 || (ShowError && type.HasFlag(LogMessageType.Error))
                 || (ShowFatal && type.HasFlag(LogMessageType.Fatal))
                 || (ShowValidation && type.HasFlag(LogMessageType.Validation)));
        }

        /// <summary>
        /// Updates the number of log messages with the different possible types.
        /// </summary>
        /// <param name="exec">The execution to fetch updated counts from.</param>
        private void UpdateCounters(ExecutionObservable exec)
        {
            InfoCount = exec.InfoCount;
            WarnCount = exec.WarnCount;
            ErrorCount = exec.ErrorCount;
            FatalCount = exec.FatalCount;
            ValidationCount = exec.ValidationCount;
        }

        public void ScrollToLast()
        {
            if (DoAutoScroll && LogListView is not null && !LogListView.Items.IsEmpty)
            {
                ScrollToLast(this, new RoutedEventArgs());
            }
        }

        public void ScrollToLast(object sender, RoutedEventArgs e)
        {
            int itemCount = LogListView.Items.Count;
            if (itemCount > 0)
            {
                LogListView.ScrollIntoView(LogListView.Items[^1]);
            }
        }

        private void SetExecution(ExecutionObservable exec)
        {
            if (exec is not null)
            {
                MessageView = (CollectionView)CollectionViewSource.GetDefaultView(exec.LogMessages);
                UpdateCounters(exec);
                MessageView.Filter = OnMessagesFilter;
                ScrollToLast();
            }
        }
    }
}
