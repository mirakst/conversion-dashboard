using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DashboardFrontend.ViewModels;
using Model;

namespace DashboardFrontend.NewViewModels
{
    public class NewValidationReportViewModel : BaseViewModel
    {
        public NewValidationReportViewModel(IDashboardController controller)
        {
            _executions = new();
            _nameFilter = string.Empty;
            _showEmpty = false;
            _showOk = true;
            _showDisabled = true;
            _showFailed = true;

            if (controller.Conversion is null)
            {
                controller.OnConversionCreated += Controller_OnConversionCreated;
            }
            else
            {
                Controller_OnConversionCreated(controller.Conversion);
            }
        }

        private ObservableCollection<Execution> _executions;
        public ObservableCollection<Execution> Executions
        {
            get => _executions;
            set
            {
                _executions = value;
                OnPropertyChanged(nameof(Executions));
            }
        }
        private Execution? _selectedExecution;
        public Execution? SelectedExecution
        {
            get => _selectedExecution;
            set
            {
                _selectedExecution = value;
                OnPropertyChanged(nameof(SelectedExecution));
                if (SelectedExecution is not null)
                {
                    SetupView();
                }
            }
        }
        private ListCollectionView _managerView;
        public ListCollectionView ManagerView
        {
            get => _managerView;
            set
            {
                _managerView = value;
                OnPropertyChanged(nameof(ManagerView));
            }
        }
        private string _nameFilter;
        public string NameFilter
        {
            get => _nameFilter;
            set
            {
                _nameFilter = value;
                OnPropertyChanged(nameof(NameFilter));
                ManagerView?.Refresh();
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
        private bool _showDisabled;
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
        private bool _showFailed;
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
        private bool _showEmpty;
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

        private void SetupView()
        {
            if (SelectedExecution is null)
            {
                throw new ArgumentNullException(nameof(SelectedExecution), "The selected execution must not be null when creating a CollectionView from its managers");
            }
            foreach (var manager in SelectedExecution.Managers)
            {
                manager.ValidationSource.View.Filter = OnValidationsFilter;
            }
            ManagerView = new(SelectedExecution.Managers)
            {
                Filter = OnManagersFilter,
                IsLiveFiltering = true,
                LiveFilteringProperties = { nameof(Manager.ValidationsTotal) },
                IsLiveSorting= true,
                LiveSortingProperties = { nameof(Manager.ValidationsTotal), nameof(Manager.StartTime) },
                SortDescriptions = { new(nameof(Manager.ValidationsFailed), ListSortDirection.Descending), new(nameof(Manager.ValidationsDisabled), ListSortDirection.Descending), new(nameof(Manager.StartTime), ListSortDirection.Ascending) }
            };
        }

        private bool OnManagersFilter(object item)
        {
            Manager m = (Manager)item;
            bool isEmpty = m.ValidationSource.Dispatcher.Invoke(() => m.ValidationSource.View.IsEmpty); // Ladies and gentlemen: WPF!
            return (m.Name is not null && m.Name.Contains(NameFilter) || m.ContextId.ToString() == NameFilter) && (ShowEmpty || !isEmpty);
        }

        private bool OnValidationsFilter(object item)
        {
            ValidationTest v = (ValidationTest)item;
            return (ShowOk && v.Status is ValidationStatus.Ok)
                || (ShowDisabled && v.Status is ValidationStatus.Disabled)
                || (ShowFailed && v.Status is ValidationStatus.Failed or ValidationStatus.FailMismatch);
        }

        private void RefreshViews()
        {
            if (SelectedExecution is null)
            {
                return;
            }
            foreach (var manager in SelectedExecution.Managers)
            {
                manager.ValidationSource.View.Filter = OnValidationsFilter;
            }
            ManagerView?.Refresh();
        }

        private void Controller_OnConversionCreated(Conversion conversion)
        {
            Executions = conversion.Executions;
            SelectedExecution = conversion.ActiveExecution;
            Executions.CollectionChanged += Executions_CollectionChanged;
        }

        private void Executions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Executions.Any())
            {
                SelectedExecution = Executions.Last();
            }
        }
    }
}
