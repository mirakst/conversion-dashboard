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



        #region Properties
        private ValidationReport _validationReport;
        public List<string> ExpandedManagerNames = new();
        public ObservableCollection<ManagerValidationsWrapper> ManagerList { get; private set; } = new();
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
        public void UpdateData(ValidationReport validationReport)
        {
            _validationReport = validationReport;
            ManagerList = GetManagerList(validationReport.ValidationTests);
            ManagerView = GetManagerCollectionView(ManagerList);
            ManagerView.Filter = OnManagersFilter;
            ManagerView.SortDescriptions.Add(new(nameof(FailedCount), ListSortDirection.Descending));
            ManagerView.SortDescriptions.Add(new(nameof(DisabledCount), ListSortDirection.Descending));
            UpdateCounters(validationReport);
        }

        /// <summary>
        /// Used as a filter for the ManagerView CollectionView.
        /// </summary>
        /// <param name="item">A ManagerValidationsWrapper object.</param>
        /// <returns>True if the object should be shown in the CollectionView, and false otherwise.</returns>
        private bool OnManagersFilter(object item)
        {
            ManagerValidationsWrapper wrapper = (ManagerValidationsWrapper)item;
            return wrapper.ManagerName.Contains(NameFilter) && !wrapper.ValidationView.IsEmpty;
        }

        /// <summary>
        /// Refreshes all Validation CollectionViews in the ManagerValidationsWrappers, and then updates and filters the actual list of wrappers.
        /// </summary>
        private void RefreshViews()
        {
            if (ManagerView != null)
            {
                foreach (object item in ManagerView)
                {
                    ManagerValidationsWrapper wrapper = (ManagerValidationsWrapper)item;
                    wrapper.ValidationView?.Refresh();
                }
                UpdateData(_validationReport);
            }
        }

        /// <summary>
        /// Updates the number of validation tests with the different possible statuses.
        /// </summary>
        private void UpdateCounters(ValidationReport validationReport)
        {
            OkCount = validationReport.ValidationTests.Count(x => x.Status is ValidationStatus.Ok);
            DisabledCount = validationReport.ValidationTests.Count(x => x.Status is ValidationStatus.Disabled);
            FailedCount = validationReport.ValidationTests.Count(x => x.Status is ValidationStatus.Failed or ValidationStatus.FailMismatch);
            TotalCount = validationReport.ValidationTests.Count;
        }

        /// <summary>
        /// Goes through the specified list and groups Validations together by their associated Manager, which is represented by a list of ManagerValidationsWrappers.
        /// </summary>
        /// <param name="validations">List of validation test data.</param>
        /// <returns>A collection of ManagerValidationsWrappers containing the given validation tests.</returns>
        private ObservableCollection<ManagerValidationsWrapper> GetManagerList(IList<ValidationTest> validations)
        {
            ObservableCollection<ManagerValidationsWrapper> result = new();
            foreach (ValidationTest test in validations)
            {
                ManagerValidationsWrapper? dataEntry = result.FirstOrDefault(e => e.ManagerName == test.ManagerName);
                if (dataEntry != null)
                {
                    dataEntry.AddTest(test);
                }
                else
                {
                    dataEntry = new(this, test.ManagerName);
                    dataEntry.AddTest(test);
                    if (ExpandedManagerNames.Contains(test.ManagerName))
                    {
                        dataEntry.IsExpanded = true;
                    }
                    result.Add(dataEntry);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a CollectionView with a default view of the specified list of ManagerValidationsWrappers.
        /// </summary>
        /// <param name="wrappers">List of all wrappers.</param>
        /// <returns>A CollectionView with the specified list of items.</returns>
        private CollectionView GetManagerCollectionView(IList<ManagerValidationsWrapper> wrappers)
        {
            return (CollectionView)CollectionViewSource.GetDefaultView(wrappers);
        }
    }
}
