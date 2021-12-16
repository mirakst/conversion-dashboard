using Model;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Data;
using static Model.Reconciliation;
using System.ComponentModel;

namespace DashboardFrontend.ViewModels
{
    public class ManagerObservable : BaseViewModel
    {
        public ManagerObservable(Manager mgr)
        {
            Name = mgr.Name;
            ContextId = mgr.ContextId;
            StartTime = mgr.StartTime;
            PerformanceScore = mgr.PerformanceScore;
            ReconciliationScore = mgr.ReconciliationScore;
            Reconciliations = new(mgr.Reconciliations);
            ReconciliationView = (CollectionView)CollectionViewSource.GetDefaultView(Reconciliations);
            OriginalManager = mgr;
        }

        public Manager OriginalManager { get; private set; }
        public List<Reconciliation> Reconciliations = new();
        private CollectionView _reconciliationView;
        public CollectionView ReconciliationView
        {
            get => _reconciliationView;
            set
            {
                _reconciliationView = value;
                OnPropertyChanged(nameof(ReconciliationView));
            } 
        }
        public string Name { get; private set; }
        public int ContextId { get; private set; }
        private bool _isChecked = true;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }
        public System.DateTime? StartTime { get; private set; }
        public double? PerformanceScore { get; private set; }
        public double? ReconciliationScore { get; private set; }
        public int FailedCount => Reconciliations.Count(v => v.Status is ReconciliationStatus.Failed or ReconciliationStatus.FailMismatch);
        public int DisabledCount => Reconciliations.Count(v => v.Status is ReconciliationStatus.Disabled);
        public int OkCount => Reconciliations.Count(v => v.Status is ReconciliationStatus.Ok);
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
