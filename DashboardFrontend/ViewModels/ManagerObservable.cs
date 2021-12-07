using Model;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Data;
using static Model.ValidationTest;

namespace DashboardFrontend.ViewModels
{
    public class ManagerObservable : BaseViewModel
    {
        public ManagerObservable(Manager mgr)
        {
            Name = mgr.Name;
            ContextId = mgr.ContextId;
            StartTime = mgr.StartTime;
            Score = mgr.Score;
            Validations = new(mgr.Validations);
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
        public System.DateTime? StartTime { get; private set; }
        public double? Score { get; private set; }
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
