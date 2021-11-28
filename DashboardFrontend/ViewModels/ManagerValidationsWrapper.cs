using Model;
using System.Linq;
using System.Collections.Generic;
using static Model.ValidationTest;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace DashboardFrontend.ViewModels
{
    public class ManagerValidationsWrapper : BaseViewModel
    {
        public ManagerValidationsWrapper(ValidationReportViewModel vm, string manager)
        {
            _vm = vm;
            ManagerName = manager;
        }

        #region Properties
        public string ManagerName { get; set; }

        private ICollectionView _validationsView;
        public ICollectionView Validations
        {
            get => _validationsView;
            set
            {
                _validationsView = value;
                OnPropertyChanged(nameof(Validations));
            }
        }

        private List<ValidationTest> _validationsData = new();

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
        private int _totalCount;
        private readonly ValidationReportViewModel _vm;

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                OnPropertyChanged(nameof(TotalCount));
            }
        }
        #endregion

        public void AddTest(ValidationTest test)
        {
            _validationsData.Add(test);
            TotalCount++;
            switch(test.Status)
            {
                case ValidationStatus.Disabled:
                    DisabledCount++;
                    break;
                case ValidationStatus.Failed:
                case ValidationStatus.FailMismatch:
                    FailedCount++;
                    break;
                case ValidationStatus.Ok:
                    OkCount++;
                    break;
                default:
                    return;
            }
            UpdateView();
        }

        public void UpdateView()
        {
            _validationsView = CollectionViewSource.GetDefaultView(_validationsData.OrderBy(x => x.Status));
            _validationsView.Filter = _vm.ValidationsFilter;
        }
    }
}
