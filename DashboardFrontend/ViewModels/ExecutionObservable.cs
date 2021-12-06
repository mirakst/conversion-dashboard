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
        public ExecutionObservable(Execution exec, ValidationReportViewModel vm)
        {
            Id = exec.Id;
            StartTime = exec.StartTime;
            Managers = new(exec.Managers.Select(m => new ManagerObservable(m)));
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
    }
}
