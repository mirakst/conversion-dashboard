using DashboardBackend;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static Model.ValidationTest;

namespace DashboardFrontend.ViewModels
{
    public class ValidationReportViewModel : BaseViewModel
    {
        public ValidationReportViewModel(ValidationReport validationReport)
        {
            _validationReport = validationReport;
            UpdateData();
        }

        #region Properties
        private ValidationReport _validationReport;

        private ICollectionView _managerView;
        public ICollectionView Managers
        {
            get => _managerView;
            set
            {
                _managerView = value;
                OnPropertyChanged(nameof(Managers));
            }
        }
        
        private DateTime _lastModified;
        public DateTime LastModified
        {
            get => _lastModified;
            set
            {
                _lastModified = value;
                OnPropertyChanged(nameof(LastModified));
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
                Managers.Refresh();
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
                Filter();
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
                Filter();
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
                Filter();
            }
        }
        #endregion

        public void UpdateData()
        {
            List<ManagerValidationsWrapper> list = new();
            for (int i = 0; i < _validationReport.ValidationTests.Count; i++)
            {
                ValidationTest test = _validationReport.ValidationTests[i];
                ManagerValidationsWrapper? dataEntry = list.FirstOrDefault(e => e.ManagerName == test.ManagerName);
                if (dataEntry != null)
                {
                    dataEntry.AddTest(test);
                }
                else
                {
                    dataEntry = new(this, test.ManagerName);
                    dataEntry.AddTest(test);
                    list.Add(dataEntry);
                }
            }
            UpdateCounters();
            LastModified = _validationReport.LastModified;
            Managers = CollectionViewSource.GetDefaultView(list.OrderByDescending(x => x.FailedCount).ThenByDescending(x => x.DisabledCount));
            Managers.Filter = ManagerFilter;
        }

        private bool ManagerFilter(object item)
        {
            if (item is ManagerValidationsWrapper wrapper)
            {
                return wrapper.ManagerName.Contains(NameFilter);
            }
            return false;
        }

        public bool ValidationsFilter(object item)
        {
            if (item is ValidationTest val)
            {
                return val.Status == ValidationStatus.Ok && ShowOk
                    || val.Status == ValidationStatus.Failed && ShowFailed
                    || val.Status == ValidationStatus.FailMismatch && ShowFailed
                    || val.Status == ValidationStatus.Disabled && ShowDisabled;
            }
            return false;
        }

        private void Filter()
        {
            foreach (var item in Managers)
            {
                var wrapper = (ManagerValidationsWrapper)item;
                wrapper.Validations.Refresh();
            }
        }

        private void UpdateCounters()
        {
            OkCount = _validationReport.ValidationTests.Count(x => x.Status == ValidationStatus.Ok);
            DisabledCount = _validationReport.ValidationTests.Count(x => x.Status == ValidationStatus.Disabled);
            FailedCount = _validationReport.ValidationTests.Count(x => x.Status == ValidationStatus.Failed || x.Status == ValidationStatus.FailMismatch);
            TotalCount = _validationReport.ValidationTests.Count;
        }
    }
}
