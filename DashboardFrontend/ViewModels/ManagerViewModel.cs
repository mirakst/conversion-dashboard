using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DashboardFrontend.ViewModels
{
    public class ManagerViewModel : BaseViewModel
    {
        public ManagerViewModel(HealthReport healthReport)
        {
            ManagerChartViewModel = new(healthReport);
        }

        public ManagerViewModel(DataGrid dataGrid)
        {
            _datagrid = dataGrid;
        }

        private DataGrid _datagrid;
        private ObservableCollection<ManagerWrapper> _managers = new();
        public ManagerChartViewModel ManagerChartViewModel { get; set; }
        public ObservableCollection<ManagerWrapper> Managers
        {
            get => _managers;
            set
            {
                _managers = value;
                OnPropertyChanged(nameof(Managers));
            }
        }
        private ObservableCollection<ManagerWrapper> _wrappedManagers = new();
        public ObservableCollection<ManagerWrapper> WrappedManagers
        {
            get => _wrappedManagers;
            set
            {
                _wrappedManagers = value;
                OnPropertyChanged(nameof(WrappedManagers));
            }
        }

        private string _managerSearch = string.Empty;
        public string ManagerSearch
        {
            get => _managerSearch;
            set
            {
                _managerSearch = value;
                OnPropertyChanged(nameof(ManagerSearch));
                SearchManagers();
            }
        }

        public void SearchManagers()
        {
            foreach (ManagerWrapper manager in Managers)
            {
                DataGridRow row = (DataGridRow)_datagrid.ItemContainerGenerator.ContainerFromItem(manager);
                if (!manager.Manager.Name.Contains(ManagerSearch))
                {
                    row.Visibility = Visibility.Collapsed;
                }
                else
                {
                    row.Visibility = Visibility.Visible;
                }
            }
        }

        public void UpdateData(List<Manager> executionManagers)
        {
            Managers.Clear();
            foreach (Manager manager in executionManagers)
            {
                Managers.Add(new ManagerWrapper(manager));
            }
        }
    }
}
