using DashboardFrontend.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DashboardFrontend.DetachedWindows
{
    /// <summary>
    /// Interaction logic for ManagerListDetached.xaml
    /// </summary>
    public partial class ManagerListDetached : Window
    {
        public ManagerViewModel Vm { get; set; }
        public ManagerListDetached(ManagerViewModel managerViewModel) //Conversion parameter
        {
            Vm = managerViewModel;

            InitializeComponent();

            DataContext = Vm;
        }

        /// <summary>
        /// Adds managers to the from the manager overview by calling the manager mover method for every manager selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddManager_Click(object sender, RoutedEventArgs e)
        {
            List<ManagerWrapper> managers = new();
            foreach (ManagerWrapper manager in DatagridManagers.SelectedItems)
            {
                if (!Vm.DetailedManagers.Any(m => m.Manager.Name == manager.Manager.Name))
                {
                    DatagridManagerMover("Add", manager);
                }
                managers.Add(manager);
            }
            foreach (var manager in managers)
            {
                Vm.Managers.Remove(manager);
            }
        }

        /// <summary>
        /// Removes managers to the from the manager details overview and charts by calling the manager mover method for every manager selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveManager_Click(object sender, RoutedEventArgs e)
        {
            List<ManagerWrapper> managers = new() { };
            foreach (ManagerWrapper manager in TabInfo.IsSelected ? DatagridManagerDetails.SelectedItems : DatagridManagerCharts.SelectedItems)
            {
                managers.Add(manager);
            }
            foreach (ManagerWrapper manager in managers) //You cannot iterate through the datagrid while also removing from the datagrid.
            {
                DatagridManagerMover("Remove", manager);
                Vm.Managers.Add(manager);
            }
        }

        /// <summary>
        /// Resets the details overview and charts by calling the manager mover method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetManagers_Click(object sender, RoutedEventArgs e)
        {
            DatagridManagerMover("Clear", null);
        }

        /// <summary>
        /// Helper function assembling all the manager mover functionality.
        /// </summary>
        /// The function to be done to the manager <param name="method"></param>
        /// The manager that is being moved <param name="manager"></param>
        private void DatagridManagerMover(string method, ManagerWrapper? manager)
        {
            switch (method)
            {
                case "Add" when manager is not null:
                    manager.IsDetailedInfoShown = true;
                    Vm.DetailedManagers.Add(manager);
                    Vm.UpdateHiddenManagers();
                    Vm.ManagerChartViewModel.AddChartLinesHelper(manager);
                    break;

                case "Remove" when manager is not null:
                    manager.IsDetailedInfoShown = false;
                    Vm.DetailedManagers.Remove(manager);
                    Vm.UpdateHiddenManagers();
                    Vm.ManagerChartViewModel.RemoveChartLinesHelper(manager);
                    break;

                case "Clear":
                    foreach (var wrapper in Vm.DetailedManagers)
                    {
                        Vm.Managers.Add(wrapper);
                    }
                    Vm.DetailedManagers.Clear();
                    Vm.UpdateHiddenManagers();
                    Vm.ManagerChartViewModel.ClearChartLinesHelper();
                    break;
            }
        }

        /// <summary>
        /// If enter is pressed on a manager, add the manager to detailed view.
        /// </summary>
        private void DatagridManagersAdd_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                e.Handled = true;
                AddManager_Click(sender, e);
            }
        }

        /// <summary>
        /// If a manager is double clicked, add the manager to detailed view.
        /// </summary>
        private void DatagridManagersAdd_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement)?.Parent is not DataGridCell) return;
            AddManager_Click((object)sender, e);
        }

        /// <summary>
        /// If enter is pressed on a manager in the detailed view, remove the manager from the detailed view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatagridManagersRemove_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                e.Handled = true;
                RemoveManager_Click(sender, e);
            }
        }

        /// <summary>
        /// If a manager is double clicked in the detailed view, remove the manager from the detailed view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatagridManagersRemove_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement)?.Parent is not DataGridCell) return;
            RemoveManager_Click((object)sender, e);
        }
    }
}