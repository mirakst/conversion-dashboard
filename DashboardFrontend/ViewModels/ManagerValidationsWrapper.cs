using Model;
using System.Linq;
using System.Collections.Generic;
using static Model.ValidationTest;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace DashboardFrontend.ViewModels
{
    /// <summary>
    /// A viewmodel used to wrap, or group, <see cref="ValidationTest"/> objects by their associated <see cref="Manager"/>.
    /// </summary>
    public class ManagerValidationsWrapper : BaseViewModel
    {
        public ManagerValidationsWrapper(ValidationReportViewModel vm, string manager)
        {
            _vm = vm;
            ManagerName = manager;
        }

        #region Properties
        private readonly ValidationReportViewModel _vm;

        private ObservableCollection<ValidationTest> ValidationsList = new();
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
        public string ManagerName { get; set; }
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
        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                OnPropertyChanged(nameof(TotalCount));
            }
        }
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
        #endregion

        /// <summary>
        /// Adds a test to the list of validations, and updates the counters and the view.
        /// </summary>
        /// <param name="test">The test to be added.</param>
        public void AddTest(ValidationTest test)
        {
            ValidationsList.Add(test);
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

        /// <summary>
        /// Sets the Valida
        /// </summary>
        public void UpdateView()
        {
            ValidationView = (CollectionView)CollectionViewSource.GetDefaultView(ValidationsList);
            ValidationView.Filter = OnValidationsFilter;
        }

        /// <summary>
        /// Used as a filter for the Validations CollectionView.
        /// </summary>
        /// <param name="item">A ValidationTest object.</param>
        /// <returns>True if the object should be shown in the CollectionView, and false otherwise.</returns>
        public bool OnValidationsFilter(object item)
        {
            if (item is ValidationTest val)
            {
                return val.Status is ValidationStatus.Ok && _vm.ShowOk
                    || val.Status is ValidationStatus.Failed or ValidationStatus.FailMismatch && _vm.ShowFailed
                    || val.Status is ValidationStatus.Disabled && _vm.ShowDisabled;
            }
            return false;
        }
    }
}
