using Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Data;
using static Model.ValidationTest;
using System.ComponentModel;

namespace DashboardFrontend.ViewModels
{
    /// <summary>
    /// View model for the <see cref="ValidationReport"/> class. 
    /// </summary>
    public class ValidationReportViewModel : BaseViewModel
    {
        public ValidationReportViewModel()
        {
        }

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

        #region Properties
        public List<string> ExpandedManagerNames = new();
        public ObservableCollection<Manager> ManagerList { get; private set; } = new();
        private CollectionView _managerView;
        public CollectionView ManagerView
        {
            get => _managerView;
            private set
            {
                _managerView = value;
                OnPropertyChanged(nameof(ManagerView));
            }
        }
        private DateTime _lastUpdated;
        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set
            {
                _lastUpdated = value;
                OnPropertyChanged(nameof(LastUpdated));
            }
        }
        private string _nameFilter = string.Empty;
        public string NameFilter
        {
            get { return _nameFilter; }
            set
            {
                _nameFilter = value;
                OnPropertyChanged(nameof(NameFilter));
                ManagerView?.Refresh();
            }
        }
        private int _totalCount;
        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                OnPropertyChanged(nameof(TotalCount));
            }
        }
        private int _okCount;
        public int OkCount
        {
            get => _okCount;
            set
            {
                _okCount = value;
                OnPropertyChanged(nameof(OkCount));
            }
        }
        private int _disabledCount;
        public int DisabledCount
        {
            get => _disabledCount;
            set
            {
                _disabledCount = value;
                OnPropertyChanged(nameof(DisabledCount));
            }
        }
        private int _failedCount;
        public int FailedCount
        {
            get => _failedCount;
            set
            {
                _failedCount = value;
                OnPropertyChanged(nameof(FailedCount));
            }
        }
        private bool _showOk;
        public bool ShowOk
        {
            get => _showOk;
            set
            {
                _showOk = value;
                OnPropertyChanged(nameof(ShowOk));
                RefreshViews();
            }
        }
        private bool _showDisabled = true;
        public bool ShowDisabled
        {
            get => _showDisabled;
            set
            {
                _showDisabled = value;
                OnPropertyChanged(nameof(ShowDisabled));
                RefreshViews();
            }
        }
        private bool _showFailed = true;
        public bool ShowFailed
        {
            get => _showFailed;
            set
            {
                _showFailed = value;
                OnPropertyChanged(nameof(ShowFailed));
                RefreshViews();
            }
        }

        #endregion

        /// <summary>
        /// Gets a list of raw validation tests from the specified Validation Report which is then used to generate a CollectionView with a set filter, and updates the validation test counters.
        /// </summary>
        /// <param name="validationReport">The Validation Report to get data from.</param>
        public void UpdateData(List<Execution> executions)
        {
            Executions = new(executions.Select(e => new ExecutionObservable(e, this)));
            if (SelectedExecution is null)
            {
                SelectedExecution = Executions.Last();
            }
        }

        private void SetExecution(ExecutionObservable exec)
        {
            if (exec is not null)
            {
                ManagerView = (CollectionView)CollectionViewSource.GetDefaultView(exec.Managers);
                ManagerView.Filter = OnManagersFilter;
                ManagerView.SortDescriptions.Add(new(nameof(ManagerObservable.FailedCount), ListSortDirection.Descending));
                ManagerView.SortDescriptions.Add(new(nameof(ManagerObservable.DisabledCount), ListSortDirection.Descending));
            }
        }

        /// <summary>
        /// Used as a filter for the ManagerView CollectionView.
        /// </summary>
        /// <param name="item">A ManagerValidationsWrapper object.</param>
        /// <returns>True if the object should be shown in the CollectionView, and false otherwise.</returns>
        private bool OnManagersFilter(object item)
        {
            ManagerObservable mgr = (ManagerObservable)item;
            return mgr.Name.Contains(NameFilter);
        }

        private bool OnValidationsFilter(object item)
        {
            return item is ValidationTest val
                ? (ShowOk && val.Status is ValidationStatus.Ok)
                    || (ShowFailed && val.Status is ValidationStatus.Failed or ValidationStatus.FailMismatch)
                    || (ShowDisabled && val.Status is ValidationStatus.Disabled)
                : false;
        }

        /// <summary>
        /// Refreshes all Validation CollectionViews in the ManagerValidationsWrappers, and then updates and filters the actual list of wrappers.
        /// </summary>
        private void RefreshViews()
        {
            if (SelectedExecution != null && ManagerView != null)
            {
                foreach (object item in ManagerView)
                {
                    ManagerObservable mgr = (ManagerObservable)item;
                    mgr.ValidationView?.Refresh();
                    mgr.IsExpanded = ExpandedManagerNames.Contains(mgr.Name);
                    
                }
                ManagerView.Refresh();
            }
        }

        public class ExecutionObservable : BaseViewModel
        {
            public ExecutionObservable(Execution e, ValidationReportViewModel vm)
            {
                Id = e.Id;
                StartTime = e.StartTime;
                Managers = new(e.Managers.Select(m => new ManagerObservable(m)));
                foreach (ManagerObservable m in Managers)
                {
                    m.IsExpanded = vm.ExpandedManagerNames.Contains(m.Name);
                    m.ValidationView.Filter = vm.OnValidationsFilter;
                    m.ValidationView.SortDescriptions.Add(new(nameof(ValidationTest.Status), ListSortDirection.Ascending));
                    m.ValidationView.SortDescriptions.Add(new(nameof(ValidationTest.Date), ListSortDirection.Descending));
                    FailedTotalCount += m.Validations.Count(v => v.Status is ValidationStatus.Failed or ValidationStatus.FailMismatch);
                    DisabledTotalCount += m.Validations.Count(v => v.Status is ValidationStatus.Disabled);
                    OkTotalCount += m.Validations.Count(v => v.Status is ValidationStatus.Ok);
                    TotalCount += m.Validations.Count;
                }
            }

            public DateTime? StartTime { get; set; }
            public int FailedTotalCount
            {
                get => _failedTotalCount; set
                {
                    _failedTotalCount = value;
                    OnPropertyChanged(nameof(FailedTotalCount));
                }
            }
            public int DisabledTotalCount
            {
                get => _disabledTotalCount; set
                {
                    _disabledTotalCount = value;
                    OnPropertyChanged(nameof(DisabledTotalCount));
                }
            }
            public int OkTotalCount
            {
                get => _okTotalCount; set
                {
                    _okTotalCount = value;
                    OnPropertyChanged(nameof(OkTotalCount));
                }
            }
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
            private int _failedTotalCount;
            private int _disabledTotalCount;
            private int _okTotalCount;
            private int _totalCount;

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

        public class ManagerObservable : BaseViewModel
        {
            public ManagerObservable(Manager mgr)
            {
                Name = mgr.Name;
                ContextId = mgr.ContextId;
                Validations = mgr.Validations;
                ValidationView = (CollectionView)CollectionViewSource.GetDefaultView(Validations);
            }
            
            public List<ValidationTest> Validations = new();
            private CollectionView _validationView;
            public CollectionView ValidationView
            {
                get => _validationView; 
                set
                {
                    _validationView = value;
                    OnPropertyChanged(nameof(ValidationView));
                }
            }
            
            public string Name { get; private set; }
            public int ContextId { get; private set; }
            public int FailedCount => Validations.Count(v => v.Status is ValidationStatus.Failed or ValidationStatus.FailMismatch);
            public int DisabledCount => Validations.Count(v => v.Status is ValidationStatus.Disabled);
            public int OkCount => Validations.Count(v => v.Status is ValidationStatus.Ok);
            private bool _isExpanded;
            public bool IsExpanded
            {
                get => _isExpanded;
                set
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }
            }
        }
    }
}
