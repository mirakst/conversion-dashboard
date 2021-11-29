using Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DashboardFrontend.ViewModels
{
    public class ManagerViewModel : BaseViewModel
    {
        public ManagerViewModel(DataGrid dataGrid)
        {
            _datagrid = dataGrid;
        }

        private DataGrid _datagrid;
        private ObservableCollection<Manager> _managers;
        public ObservableCollection<Manager> Managers
        {
            get { return _managers; }
            set
            {
                _managers = value;
                OnPropertyChanged(nameof(Managers));
            }
        }

        private string _managerSearch = string.Empty;
        public string ManagerSearch
        {
            get { return _managerSearch; }
            set
            {
                _managerSearch = value;
                OnPropertyChanged(nameof(ManagerSearch));
                SearchManagers();
            }
        }

        public void SearchManagers()
        {
            foreach (Manager manager in Managers)
            {
                DataGridRow row = (DataGridRow)_datagrid.ItemContainerGenerator.ContainerFromItem(manager);
                if (!manager.Name.Contains(ManagerSearch))
                {
                    row.Visibility = Visibility.Collapsed;
                }
                else
                {
                    row.Visibility = Visibility.Visible;
                }
            }
        }

        public void UpdateData()
        {
            Managers.Clear();
            foreach (Manager manager in _managers)
            {
                Trace.WriteLine(nameof(manager.Name));
                Managers.Add(manager);
            }
        }
    }
}
