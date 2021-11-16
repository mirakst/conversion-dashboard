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
using static Model.ValidationTest.ValidationStatus;

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

        public ObservableCollection<ValidationTestViewModel> Data { get; set; } = new();
        
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
        private bool _showSuccessfulManagers;
        public bool ShowSuccessfulManagers
        {
            get => _showSuccessfulManagers;
            set
            {
                _showSuccessfulManagers = value;
                OnPropertyChanged(nameof(ShowSuccessfulManagers));
            }
        }
        #endregion

        public void UpdateData()
        {
            OkCount = 0;
            DisabledCount = 0;
            FailedCount = 0;
            foreach (ValidationTest test in _validationReport.ValidationTests)
            {
                ValidationTestViewModel? dataEntry = Data.FirstOrDefault(e => e.Manager == test.ManagerName);
                if (dataEntry != null)
                {
                    dataEntry.Tests.Add(test);
                }
                else
                {
                    dataEntry = new(test.ManagerName, new List<ValidationTest>());
                    dataEntry.Tests.Add(test);
                    Data.Add(dataEntry);
                }
                UpdateCounter(test);
            }
        }

        private void UpdateCounter(ValidationTest test)
        {
            switch (test.Status)
            {
                case OK:
                    OkCount++;
                    break;
                case DISABLED:
                    DisabledCount++;
                    break;
                case FAILED:
                case FAIL_MISMATCH:
                    FailedCount++;
                    break;
            }
        }

    }
}
