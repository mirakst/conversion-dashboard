using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DashboardFrontend.NewViewModels
{
    public class NewLogViewModel : BaseViewModel
    {
        public NewLogViewModel(Conversion conversion)
        {
            if (conversion is null)
            {
                throw new ArgumentNullException(nameof(conversion), "Conversion cannot be null");
            }
            ShowInfo = true;
            ShowWarn = true;
            ShowErrors = true;
            ShowFatal = true;
            ShowValidations = true;
            Executions = conversion.Executions;
            SelectedExecution = conversion.ActiveExecution;
        }

        public ObservableCollection<Execution> Executions { get; }
        private Execution? _selectedExecution;
        public Execution? SelectedExecution
        {
            get => _selectedExecution;
            set
            {
                _selectedExecution = value;
                OnPropertyChanged(nameof(SelectedExecution));
                if (SelectedExecution is not null)
                {
                    SetupLog();
                    SetupView();
                }
            }
        }
        private Log? _log;
        public Log? Log 
        { 
            get => _log; 
            set
            {
                _log = value;
                OnPropertyChanged(nameof(Log));
            } 
        }
        private ICollectionView _messageView;
        public ICollectionView MessageView
        {
            get => _messageView;
            set
            {
                _messageView = value;
                OnPropertyChanged(nameof(MessageView));
            }
        }
        private bool _showInfo;
        public bool ShowInfo
        {
            get => _showInfo; set
            {
                _showInfo = value;
                OnPropertyChanged(nameof(ShowInfo));
                MessageView?.Refresh();
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
                MessageView?.Refresh();
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
                MessageView?.Refresh();
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
                MessageView?.Refresh();
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
                MessageView?.Refresh();
            }
        }

        private void SetupLog()
        {
            if (SelectedExecution is null)
            {
                throw new ArgumentException("Expected a non-null execution when setting up the log", nameof(SelectedExecution));
            }
            Log = SelectedExecution.Log;
        }

        public void SetupView()
        {
            if (Log is null)
            {
                throw new ArgumentNullException(nameof(Log), "Cannot create default view when Log is null");
            }
            MessageView = new CollectionViewSource { Source = Log.Messages }.View;
            MessageView.Filter += OnMessagesFilter;
        }

        public bool OnMessagesFilter(object item)
        {
            LogMessage msg = (LogMessage)item;
            return (ShowInfo && msg.Type.HasFlag(LogMessageType.Info))
                || (ShowErrors && msg.Type.HasFlag(LogMessageType.Error))
                || (ShowFatal && msg.Type.HasFlag(LogMessageType.Fatal))
                || (ShowWarn && msg.Type.HasFlag(LogMessageType.Warning))
                || (ShowValidations && msg.Type.HasFlag(LogMessageType.Validation));
        }
    }
}
