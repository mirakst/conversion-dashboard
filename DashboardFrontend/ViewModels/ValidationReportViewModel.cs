using Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static Model.ValidationTest;

namespace DashboardFrontend.ViewModels
{
    public class ValidationReportViewModel : BaseViewModel
    {
        public ValidationReportViewModel()
        {
            
        }

        public ValidationReportViewModel(DataGrid dataGridValidations)
        {
            DataGrid = dataGridValidations;
        }
        #region Properties
        public DataGrid DataGrid { get; set; }
        public ObservableCollection<ValidationTestViewModel> Data { get; set; } = new();

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
                Filter();
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
        private bool _showSuccessfulManagers = true;
        public bool ShowSuccessfulManagers
        {
            get => _showSuccessfulManagers;
            set
            {
                _showSuccessfulManagers = value;
                OnPropertyChanged(nameof(ShowSuccessfulManagers));
                Filter();
            }
        }
        #endregion

        public void UpdateData(ValidationReport validationReport)
        {
            OkCount = 0;
            DisabledCount = 0;
            FailedCount = 0;
            TotalCount = 0;
            Data.Clear();
            foreach (ValidationTest test in validationReport.ValidationTests)
            {
                ValidationTestViewModel? dataEntry = Data.FirstOrDefault(e => e.ManagerName == test.ManagerName);
                if (dataEntry != null)
                {
                    dataEntry.AddTest(test);
                }
                else
                {
                    dataEntry = new(test.ManagerName);
                    dataEntry.AddTest(test);
                    Data.Add(dataEntry);
                }
                UpdateCounter(test);
            }
            LastModified = validationReport.LastModified;
        }

        /// <summary>
        /// Filters the Data collection by setting the visibility property of their associated DataGridRow control
        /// </summary>
        public void Filter()
        {
            foreach (ValidationTestViewModel item in Data)
            {
                DataGridRow row = (DataGridRow)DataGrid.ItemContainerGenerator.ContainerFromItem(item);
                if (!item.ManagerName.Contains(NameFilter) || (item.OkCount == item.TotalCount && !ShowSuccessfulManagers))
                {
                    row.Visibility = Visibility.Collapsed;
                }
                else
                {
                    row.Visibility = Visibility.Visible;
                }
            }
        }

        private void UpdateCounter(ValidationTest test)
        {
            switch (test.Status)
            {
                case ValidationStatus.Ok:
                    OkCount++;
                    break;
                case ValidationStatus.Disabled:
                    DisabledCount++;
                    break;
                case ValidationStatus.Failed:
                case ValidationStatus.FailMismatch:
                    FailedCount++;
                    break;
                default: 
                    break;
            }
            TotalCount++;
        }
    }
}
