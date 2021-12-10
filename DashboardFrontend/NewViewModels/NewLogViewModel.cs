using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using Model;

namespace DashboardFrontend.NewViewModels
{
    public class NewLogViewModel : BaseViewModel
    {
        public NewLogViewModel(IDashboardController controller)
        {
            _executions = new();
            _messageView = CollectionViewSource.GetDefaultView(null);
            ShowInfo = true;
            ShowWarn = true;
            ShowErrors = true;
            ShowFatal = true;
            ShowValidations = true;

            if (controller.Conversion is not null)
            {
                Controller_OnConversionCreated(controller.Conversion);
            }
            else
            {
                controller.OnConversionCreated += Controller_OnConversionCreated;
            }
        }

        private void Controller_OnConversionCreated(Conversion conversion)
        {
            Executions = conversion.Executions;
            SelectedExecution = conversion.ActiveExecution;
            Executions.CollectionChanged += Executions_CollectionChanged;
        }

        private ObservableCollection<Execution> _executions;
        public ObservableCollection<Execution> Executions
        {
            get => _executions; 
            private set
            {
                _executions = value;
                OnPropertyChanged(nameof(Executions));
            }
        }
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
                    SetupView();
                }
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

        public void SetupView()
        {
            if (SelectedExecution is null)
            {
                throw new ArgumentNullException(nameof(SelectedExecution), "The selected execution must not be null when creating a CollectionView from its log");
            }
            MessageView = new CollectionViewSource { Source = SelectedExecution.Log.Messages }.View;
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

        private void Executions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Executions.Any())
            {
                SelectedExecution = Executions.Last();
            }
        }
    }
}
