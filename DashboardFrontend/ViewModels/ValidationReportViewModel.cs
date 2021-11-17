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
using static Model.ValidationTest;

namespace DashboardFrontend.ViewModels
{
    public class ValidationReportViewModel : BaseViewModel
    {
        public ValidationReportViewModel(ValidationReport validationReport, DataGrid dataGrid)
        {
            _dataGrid = dataGrid;
            _validationReport = validationReport;
            UpdateData();
        }




        #region Properties
        private ValidationReport _validationReport;
        private DataGrid _dataGrid;

        public ObservableCollection<ValidationTestViewModel> Data { get; set; } = new();

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
        public int TotalCount => OkCount + DisabledCount + FailedCount;
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

        public void UpdateData()
        {
            OkCount = 0;
            DisabledCount = 0;
            FailedCount = 0;
            foreach (ValidationTest test in _validationReport.ValidationTests)
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
        }

        /// <summary>
        /// Filters the Data collection by setting the visibility property of their associated DataGridRow control
        /// </summary>
        public void Filter()
        {
            foreach (var item in Data)
            {
                DataGridRow row = (DataGridRow)_dataGrid.ItemContainerGenerator.ContainerFromItem(item);
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
            }
        }

    }
}
