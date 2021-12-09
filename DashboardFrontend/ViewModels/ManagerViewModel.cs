using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DashboardFrontend.ViewModels
{
    public class ManagerViewModel : BaseViewModel
    {
        public ManagerViewModel()
        {
            ManagerChartViewModel = new();
        }

        public ManagerViewModel(Window detachedWindow) : this()
        {
            Window = detachedWindow;
        }

        public DateTime LastUpdated { get; set; } = DateTime.MinValue;
        private string _managerSearch = string.Empty;
        public string ManagerSearch { get => _managerSearch; 
            set
            {
                _managerSearch = value;
                if (DataGridManagers != null) _dataGridManagers.Items.Filter = OnManagersFilter;
            }
        }
        private ObservableCollection<ManagerWrapper> _managers = new();
        private DataGrid _dataGridManagers;
        public DataGrid DataGridManagers { 
            get => _dataGridManagers; 
            set
            {
                _dataGridManagers = value;
            }
        }
        public ManagerChartViewModel ManagerChartViewModel { get; set; }
        public Window Window { get; set; }
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
        public List<string> ExpandedManagerNames = new();

        public void UpdateData(List<Manager> executionManagers)
        {
            Managers.Clear();
            foreach (var manager in executionManagers)
            {
                Managers.Add(new ManagerWrapper(manager));
            }
            if (DataGridManagers != null) DataGridManagers.Items.Filter = OnManagersFilter;
        }

        private bool OnManagersFilter(object item)
        {
            ManagerWrapper mgr = (ManagerWrapper)item;
            return (mgr.Manager.Name.ToLower().Contains(ManagerSearch.ToLower()) || mgr.Manager.ContextId.ToString() == ManagerSearch);
        }
    }
}
