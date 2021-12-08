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

        public ObservableCollection<ExecutionObservable> Executions { get; set; } = new();
        private ExecutionObservable? _selectedExecution;
        public ExecutionObservable? SelectedExecution
        {
            get => _selectedExecution;
            set
            {
                _selectedExecution = value;
                OnPropertyChanged(nameof(SelectedExecution));
                SetExecution(value);
            }
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

        public void UpdateData(List<Execution> executions)
        {
            Executions.Clear();
            int count = executions.Count;
            for (int i = 0; i < count; i++)
            {
                Executions.Add(new ExecutionObservable(executions[i]));
            }
            if (SelectedExecution is null && count > 0)
            {
                SelectedExecution = Executions[^1];
            }
        }

        private void SetExecution(ExecutionObservable exec)
        {
            if (exec is not null)
            {
                Managers.Clear();
                foreach (var manager in exec.Managers)
                {
                    Managers.Add(new ManagerWrapper(manager.OriginalManager));
                }
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
