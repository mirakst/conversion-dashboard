using Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using static Model.Reconciliation;

namespace DashboardFrontend.ViewModels
{
    public class ExecutionObservable : BaseViewModel
    {
        public ExecutionObservable(Execution exec)
        {
            Id = exec.Id;
            StartTime = exec.StartTime;

            Managers = new();
            for(int i = 0; i < exec.Managers.Count; i++)
            {
                Managers.Add(new ManagerObservable(exec.Managers[i]));
            }

            LogMessages = new(exec.Log.Messages);
        }
        public ExecutionObservable(Execution exec, ReconciliationReportViewModel vm) : this(exec)
        {
            foreach (ManagerObservable manager in Managers)
            {
                manager.IsExpanded = vm.ExpandedManagerNames.Contains(manager.Name);
                manager.ReconciliationView.Filter = vm.OnReconciliationsFilter;
                manager.ReconciliationView.SortDescriptions.Add(new(nameof(Reconciliation.Status), ListSortDirection.Ascending));
                manager.ReconciliationView.SortDescriptions.Add(new(nameof(Reconciliation.Date), ListSortDirection.Descending));
                FailedTotalCount += manager.FailedCount;
                DisabledTotalCount += manager.DisabledCount;
                OkTotalCount += manager.OkCount;
                TotalCount += manager.Reconciliations.Count;
            }
        }

        public DateTime? StartTime { get; set; }
        private int _failedTotalCount;
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
        public int ReconciliationCount => LogMessages.Count(m => m.Type.HasFlag(LogMessage.LogMessageType.Reconciliation));
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
