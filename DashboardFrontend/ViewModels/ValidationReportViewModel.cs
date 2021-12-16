using Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Data;
using static Model.Reconciliation;
using System.ComponentModel;

namespace DashboardFrontend.ViewModels
{
    /// <summary>
    /// View model for the Reconciliation Report UI module which displays information about the <see cref="Reconciliation"/> class. 
    /// </summary>
    public class ReconciliationReportViewModel : BaseViewModel
    {
        public ReconciliationReportViewModel()
        {
        }

        #region Properties
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
                RefreshViews();
            }
        }
        public List<string> ExpandedManagerNames = new();
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
                RefreshViews();
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
        private bool _showEmpty = false;
        public bool ShowEmpty
        {
            get => _showEmpty;
            set
            {
                _showEmpty = value;
                OnPropertyChanged(nameof(ShowEmpty));
                RefreshViews();
            }
        }
        #endregion

        /// <summary>
        /// Updates the list of observable executions in this viewmodel, which is the source of the displayed Reconciliations.
        /// </summary>
        public void UpdateData(List<Execution> executions)
        {
            Executions.Clear();
            for (int i = 0; i < executions.Count; i++)
            {
                Executions.Add(new ExecutionObservable(executions[i], this));
            }
            if (SelectedExecution is null)
            {
                SelectedExecution = Executions.Last();
            }
        }

        /// <summary>
        /// Sets the selected execution for the reconciliation report module.
        /// </summary>
        /// <param name="exec">The execution to show data for.</param>
        private void SetExecution(ExecutionObservable exec)
        {
            if (exec is not null)
            {
                ExpandedManagerNames.Clear();
                ManagerView = (CollectionView)CollectionViewSource.GetDefaultView(exec.Managers);
                ManagerView.Filter = OnManagersFilter;
                ManagerView.SortDescriptions.Add(new(nameof(ManagerObservable.FailedCount), ListSortDirection.Descending));
                ManagerView.SortDescriptions.Add(new(nameof(ManagerObservable.DisabledCount), ListSortDirection.Descending));
                ManagerView.SortDescriptions.Add(new(nameof(ManagerObservable.StartTime), ListSortDirection.Ascending));
            }
        }

        /// <summary>
        /// Used as a filter for the ManagerView CollectionView.
        /// </summary>
        /// <param name="item">A ManagerObservable object.</param>
        /// <returns>True if the object should be shown in the CollectionView, and false otherwise.</returns>
        private bool OnManagersFilter(object item)
        {
            ManagerObservable mgr = (ManagerObservable)item;
            return (mgr.Name.Contains(NameFilter) || mgr.ContextId.ToString() == NameFilter) && (!mgr.ReconciliationView.IsEmpty || ShowEmpty);
        }


        /// <summary>
        /// Used as a filter for reconciliations
        /// </summary>
        /// <param name="item">A reconciliation object.</param>
        /// <returns>True if the object should be shown, and false otherwise.</returns>
        public bool OnReconciliationsFilter(object item)
        {
            return item is Reconciliation val && ((ShowOk && val.Status is ReconciliationStatus.Ok)
                                              || (ShowFailed && val.Status is ReconciliationStatus.Failed or ReconciliationStatus.FailMismatch)
                                              || (ShowDisabled && val.Status is ReconciliationStatus.Disabled));
        }

        /// <summary>
        /// Refreshes all reconciliation CollectionViews in the list of Managers in this viewmodel's selected execution, and then updates the actual manager view by setting the selected execution.
        /// </summary>
        private void RefreshViews()
        {
            if (SelectedExecution != null)
            {
                foreach (var mgr in SelectedExecution.Managers)
                {
                    mgr.ReconciliationView.Refresh();
                    mgr.IsExpanded = ExpandedManagerNames.Contains(mgr.Name);
                }
                SetExecution(SelectedExecution);
            }
        }
    }
}
