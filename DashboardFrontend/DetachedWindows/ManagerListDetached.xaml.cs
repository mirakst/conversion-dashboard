using DashboardFrontend.Charts;
using DashboardFrontend.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            foreach (ManagerWrapper manager in DatagridManagers.SelectedItems)
            {
                if (!Vm.WrappedManagers.Any(e => e.Manager.ContextId == manager.Manager.ContextId))
                {
                    DatagridManagerMover("Add", manager);
                    manager.IsDetailedInfoShown = true;
                }
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
                manager.IsDetailedInfoShown = false;
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
        /// Searches through the manager overview based on the text present in the searchbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextboxSearchbar_TextChanged(object sender, TextChangedEventArgs e)
        {
            DatagridManagers.SelectedItems.Clear();
            if (TextboxSearchbar.Text != null)
            {
                List<ManagerWrapper> foundManagers = new();
                foreach (ManagerWrapper manager in DatagridManagers.Items)
                {
                    if (manager.Manager.Name.Contains(TextboxSearchbar.Text) || manager.Manager.ContextId.ToString() == TextboxSearchbar.Text)
                    {
                        foundManagers.Add(manager);
                        DatagridManagers.SelectedItems.Add(manager);
                    }
                }
            }
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
                    Vm.WrappedManagers.Add(manager);
                    Vm.ManagerChartViewModel.AddChartLinesHelper(manager);
                    break;

                case "Remove" when manager is not null:
                    Vm.WrappedManagers.Remove(manager);
                    Vm.ManagerChartViewModel.RemoveChartLinesHelper(manager);
                    break;

                case "Clear":
                    Vm.WrappedManagers.Clear();
                    Vm.ManagerChartViewModel.ClearChartLinesHelper();
                    break;
            }
        }
    }
}