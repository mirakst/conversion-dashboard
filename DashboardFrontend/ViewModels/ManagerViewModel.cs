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

        public DateTime LastUpdated { get; set; } = DateTime.MinValue;
        private string _managerSearch = string.Empty;
        public string ManagerSearch { get => _managerSearch; 
            set
            {
                _managerSearch = value;
                SearchManagers();
            }
        }
        private ObservableCollection<ManagerWrapper> _managers = new();
        public DataGrid DatagridManagers;
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
        public List<string> ExpandedManagerNames = new();

        public void UpdateData(List<Manager> executionManagers)
        {
            Managers.Clear();
            foreach (var manager in executionManagers)
            {
                Managers.Add(new ManagerWrapper(manager));
            }
        }

        private void SearchManagers()
        {
            DatagridManagers.SelectedItems.Clear();
            if (!string.IsNullOrEmpty(ManagerSearch))
            {
                List<ManagerWrapper> foundManagers = new();
                foreach (ManagerWrapper manager in DatagridManagers.Items)
                {
                    if (manager.Manager.Name.ToLower().Contains(ManagerSearch.ToLower()) || manager.Manager.ContextId.ToString() == ManagerSearch)
                    {
                        foundManagers.Add(manager);
                        DatagridManagers.SelectedItems.Add(manager);
                    }
                }
            }
            else
            {
                DatagridManagers.SelectedItems.Clear();
            }
        }
    }
}
