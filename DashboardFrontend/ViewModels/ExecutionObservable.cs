using Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using static Model.ValidationTest;
using System.ComponentModel;

namespace DashboardFrontend.ViewModels
{
    public class ExecutionObservable : BaseViewModel
    {
        public ExecutionObservable(Execution exec)
        {
            Id = exec.Id;
            StartTime = exec.StartTime;
            lock (executionLock)
            {
                Managers = new(exec.Managers.Select(m => new ManagerObservable(m)));
            }
            LogMessages = new(exec.Log.Messages);
        }
        public ExecutionObservable(Execution exec, ValidationReportViewModel vm) : this(exec)
        {
            foreach (ManagerObservable manager in Managers)
            {
                manager.IsExpanded = vm.ExpandedManagerNames.Contains(manager.Name);
                manager.ValidationView.Filter = vm.OnValidationsFilter;
                manager.ValidationView.SortDescriptions.Add(new(nameof(ValidationTest.Status), ListSortDirection.Ascending));
                manager.ValidationView.SortDescriptions.Add(new(nameof(ValidationTest.Date), ListSortDirection.Descending));
                FailedTotalCount += manager.Validations.Count(v => v.Status is ValidationStatus.Failed or ValidationStatus.FailMismatch);
                DisabledTotalCount += manager.Validations.Count(v => v.Status is ValidationStatus.Disabled);
                OkTotalCount += manager.Validations.Count(v => v.Status is ValidationStatus.Ok);
                TotalCount += manager.Validations.Count;
            }
        }

        public DateTime? StartTime { get; set; }
        private int _failedTotalCount;
        private readonly object executionLock = new();
        public int FailedTotalCount
        {
            get => _failedTotalCount; set
            {
                _failedTotalCount = value;
                OnPropertyChanged(nameof(FailedTotalCount));
            }
        }
        private int _disabledTotalCount;
        public int DisabledTotalCount
        {
            get => _disabledTotalCount; set
            {
                _disabledTotalCount = value;
                OnPropertyChanged(nameof(DisabledTotalCount));
            }
        }
        private int _okTotalCount;
        public int OkTotalCount
        {
            get => _okTotalCount; set
            {
                _okTotalCount = value;
                OnPropertyChanged(nameof(OkTotalCount));
            }
        }
        private int _totalCount;
        public int TotalCount
        {
            get => _totalCount; set
            {
                _totalCount = value;
                OnPropertyChanged(nameof(TotalCount));
            }
        }
        public int InfoCount => LogMessages.Count(m => m.Type.HasFlag(LogMessage.LogMessageType.Info));
        public int WarnCount => LogMessages.Count(m => m.Type.HasFlag(LogMessage.LogMessageType.Warning));
        public int ErrorCount => LogMessages.Count(m => m.Type.HasFlag(LogMessage.LogMessageType.Error));
        public int FatalCount => LogMessages.Count(m => m.Type.HasFlag(LogMessage.LogMessageType.Fatal));
        public int ValidationCount => LogMessages.Count(m => m.Type.HasFlag(LogMessage.LogMessageType.Validation));
        public int Id { get; set; }
        private ObservableCollection<ManagerObservable> _managers = new();
        public ObservableCollection<ManagerObservable> Managers
        {
            get => _managers;
            set
            {
                _managers = value;
                OnPropertyChanged(nameof(Managers));
            }
        }
        private ObservableCollection<LogMessage> _logMessages = new();
        public ObservableCollection<LogMessage> LogMessages
        {
            get => _logMessages;
            set
            {
                _logMessages = value;
                OnPropertyChanged(nameof(LogMessages));
            }
        }
    }
}
