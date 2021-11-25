using DashboardBackend;
using DashboardBackend.Database;
using DashboardFrontend.ViewModels;
using Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DashboardFrontend.DetachedWindows
{
    /// <summary>
    /// Interaction logic for ManagerListDetached.xaml
    /// </summary>
    public partial class ManagerListDetached : Window
    {
        public ManagerChartViewModel ManagerCharts { get; private set; }

        public ManagerListDetached()
        {
            ManagerCharts = new();

            InitializeComponent();

            DataUtilities.DatabaseHandler = new SqlDatabase();                                                                          // Remove later once the Start monitoring functions has been made
            Conversion conv = new();                                                                                                    
            conv.Executions = DataUtilities.GetExecutions();                                                                            
            conv.ActiveExecution.Managers = DataUtilities.GetManagers();                                                                
            conv.HealthReport = DataUtilities.BuildHealthReport();                                                                      
            DataUtilities.AddHealthReportReadings(conv.HealthReport);                                                                   // To here

            Random random = new(5);
            foreach (Manager manager in conv.ActiveExecution.Managers)
            {
                manager.Readings.Add(new ManagerUsage(random.Next() % 100, random.Next() % 100, random.Next() % 100, DateTime.Now));    // Also delete this
                var wrapper = new ManagerWrapper(manager);
                datagridManagers.Items.Add(wrapper);
            }

            DataContext = this;
        }

        private void AddManager_Click(object sender, RoutedEventArgs e)
        {
            ManagerWrapper? selectedManager = datagridManagers.SelectedItem as ManagerWrapper;

            if (datagridManagers.SelectedItems.Count > 1)
            {
                foreach (ManagerWrapper manager in datagridManagers.SelectedItems)
                {
                    if (!datagridManagerDetails.Items.Contains(manager))
                    {
                        DatagridManagerMover("Add", manager);
                    }
                }
            }
            else if (!datagridManagerDetails.Items.Contains(selectedManager))
            {
                DatagridManagerMover("Add", selectedManager);
            }
        }

        private void RemoveManager_Click(object sender, RoutedEventArgs e)
        {
            ManagerWrapper? selectedManager = ((Button)sender).Tag as ManagerWrapper;
            List<ManagerWrapper> managers = new() { };

            if (datagridManagerDetails.SelectedItems.Count > 1 || datagridManagerCharts.SelectedItems.Count > 1)
            {


                foreach (ManagerWrapper manager in TabInfo.IsSelected == true ? datagridManagerDetails.SelectedItems : datagridManagerCharts.SelectedItems)
                {
                    managers.Add(manager);
                }
                foreach (ManagerWrapper manager in managers) //You cannot iterate through the datagrid while also removing from the datagrid.
                {
                    DatagridManagerMover("Remove", manager);
                }
            }
            else
            {
                DatagridManagerMover("Remove", selectedManager);
            }
        }

        private void ResetManagers_Click(object sender, RoutedEventArgs e)
        {
            DatagridManagerMover("Clear", null);
        }

        private void TextboxSearchbar_TextChanged(object sender, TextChangedEventArgs e)
        {
            datagridManagers.SelectedItems.Clear();
            if (textboxSearchbar.Text != null)
            {
                List<ManagerWrapper> foundmanagers = new();
                foreach (ManagerWrapper manager in datagridManagers.Items)
                {
                    if (manager.Manager.Name.Contains(textboxSearchbar.Text) || manager.Manager.Id.ToString() == textboxSearchbar.Text)
                    {
                        foundmanagers.Add(manager);
                        datagridManagers.SelectedItems.Add(manager);
                    }
                }
            }
        }

        private void DatagridManagerMover(string method, ManagerWrapper? manager)
        {
            switch (method)
            {
                case "Add" when manager is not null:
                    datagridManagerDetails.Items.Add(manager);
                    datagridManagerCharts.Items.Add(manager);
                    ManagerCharts.AddChartLinesHelper(manager);
                    DataContext = ManagerCharts;
                    break;

                case "Remove" when manager is not null:
                    datagridManagerDetails.Items.Remove(manager);
                    datagridManagerCharts.Items.Remove(manager);
                    ManagerCharts.RemoveChartLinesHelper(manager);
                    break;

                case "Clear":
                    datagridManagerDetails.Items.Clear();
                    datagridManagerCharts.Items.Clear();
                    ManagerCharts.ClearChartLinesHelper();
                    break;
            }
        }

        private void CartesianChart_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            foreach (LiveChartViewModel chart in ManagerCharts.Charts)
            {
                chart.AutoFocusOn();
            }
        }

        private void CartesianChart_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            foreach (LiveChartViewModel chart in ManagerCharts.Charts)
            {
                chart.AutoFocusOff();
            }
        }
    }
}