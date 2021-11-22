using DashboardBackend;
using DashboardBackend.Database;
using Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DashboardFrontend.DetachedWindows
{
    /// <summary>
    /// Interaction logic for ManagerListDetached.xaml
    /// </summary>
    public partial class ManagerListDetached : Window
    {
        public ManagerListDetached()
        {
            InitializeComponent();

            DataUtilities.DatabaseHandler = new SqlDatabase();             //
            Conversion conv = new();                                       //
            conv.Executions = DataUtilities.GetExecutions();               // Remove once the program is set up for the database simulation.
            conv.ActiveExecution.Managers = DataUtilities.GetManagers();   //
            conv.HealthReport = DataUtilities.BuildHealthReport();         //
            DataUtilities.AddHealthReportReadings(conv.HealthReport);      //

            PeriodicTimer _dataGenerationTimer = new(TimeSpan.FromSeconds(1));

            Random random = new(5);

            foreach (Manager manager in conv.ActiveExecution.Managers)
            {
                manager.Readings.Add(new ManagerUsage(random.Next() % 100, random.Next() % 100, random.Next() % 100, DateTime.Now)); // Temp data, remove later
                datagridManagers.Items.Add(manager);
            }
        }

        private PeriodicTimer _dataGenerationTimer;
        private ManagerMonitoring managerMonitoring = new();
        private bool IsRunning = false;

        private void AddManager_Click(object sender, RoutedEventArgs e)
        {
            Manager? selectedManager = datagridManagers.SelectedItem as Manager;
            List<Manager> managers = new();

            if (datagridManagers.SelectedItems.Count > 1)
            {

                foreach (Manager manager in datagridManagers.SelectedItems)
                {
                    managers.Add(manager);

                    if (!datagridManagerDetails.Items.Contains(manager))
                    {
                        DatagridManagerMover("Add", manager);
                    }

                    if (managers.Count == datagridManagers.SelectedItems.Count)
                    {
                        //Add multiple lines to charts here.
                    }
                }
            }
            else if (!datagridManagerDetails.Items.Contains(selectedManager))
            {
                DatagridManagerMover("Add", selectedManager);
                //Add line to charts here
            }
            datagridManagers.SelectedItems.Clear();

            if (IsRunning == false)
            {
                _dataGenerationTimer = new(TimeSpan.FromSeconds(1));
                //Call GenerateData here
                IsRunning = true;
            }
        }

        private void RemoveManager_Click(object sender, RoutedEventArgs e)
        {
            Manager? selectedManager = ((Button)sender).Tag as Manager;
            List<Manager> managers = new() { };

            if (datagridManagerDetails.SelectedItems.Count > 1 || datagridManagerCharts.SelectedItems.Count > 1)
            {
                foreach (Manager manager in TabInfo.IsSelected == true ? datagridManagerDetails.SelectedItems : datagridManagerCharts.SelectedItems)
                {
                    managers.Add(manager);
                }
                foreach (Manager manager in managers) //You cannot iterate through the datagrid while also removing from the datagrid.
                {
                    DatagridManagerMover("Remove", manager);
                    //Remove multiple lines from charts here
                }
            }
            else
            {
                DatagridManagerMover("Remove", selectedManager);
                //Remove line from charts here
            }
        }

        private void ResetManagers_Click(object sender, RoutedEventArgs e)
        {
            DatagridManagerMover("Clear", null);
            //Clear charts here
        }

        private void textboxSearchbar_TextChanged(object sender, TextChangedEventArgs e)
        {
            datagridManagers.SelectedItems.Clear();
            if (textboxSearchbar.Text != null)
            {
                List<Manager> foundmanagers = new();
                foreach (Manager manager in datagridManagers.Items)
                {
                    if (manager.Name.Contains(textboxSearchbar.Text) || manager.Id.ToString() == textboxSearchbar.Text)
                    {
                        foundmanagers.Add(manager);
                        datagridManagers.SelectedItems.Add(manager);
                    }
                }
            }
        }

        private void DatagridManagerMover(string method, Manager? manager)
        {
            switch (method)
            {
                case "Add":
                    datagridManagerDetails.Items.Add(manager);
                    datagridManagerCharts.Items.Add(manager);
                    break;

                case "Remove":
                    datagridManagerDetails.Items.Remove(manager);
                    datagridManagerCharts.Items.Remove(manager);
                    break;

                case "Clear":
                    datagridManagerDetails.Items.Clear();
                    datagridManagerCharts.Items.Clear();
                    break;
            }
        }
    }
}