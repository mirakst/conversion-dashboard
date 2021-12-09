using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        public ExecutionObservable? _selectedExecution;
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
        public ObservableCollection<ManagerWrapper> Managers
        {
            get => _managers;
            set
            {
                _managers = value;
                OnPropertyChanged(nameof(Managers));
            }
        }

        private ObservableCollection<ManagerWrapper> _detailedManagers = new();
        public ObservableCollection<ManagerWrapper> DetailedManagers
        {
            get => _detailedManagers;
            set
            {
                _detailedManagers = value;
                OnPropertyChanged(nameof(DetailedManagers));
            }
        }
        private DataGrid _dataGridManagers;
        public DataGrid DataGridManagers { 
            get => _dataGridManagers; 
            set
            {
                _dataGridManagers = value;
            }
        }
        public int HiddenManagers { get; set; }

        public void UpdateHiddenManagers()
        {
            HiddenManagers = DetailedManagers.Where(m => m.Manager.CpuReadings.Count < 2).Count();
            OnPropertyChanged(nameof(HiddenManagers));
        }
        public ManagerChartViewModel ManagerChartViewModel { get; set; }
        public Window Window { get; set; }
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
                DetailedManagers.Clear();
                foreach (var manager in exec.Managers)
                {
                    Managers.Add(new ManagerWrapper(manager.OriginalManager));
                }
            }
        }
        private bool OnManagersFilter(object item)
        {
            ManagerWrapper mgr = (ManagerWrapper)item;
            return (mgr.Manager.Name.ToLower().Contains(ManagerSearch.ToLower()) || mgr.Manager.ContextId.ToString() == ManagerSearch);
        }
    }
}
