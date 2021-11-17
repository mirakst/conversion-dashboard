using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Model;
using DashboardBackend;
using DashboardBackend.Database;
using DashboardFrontend;
using System.Threading;

namespace DashboardFrontend.DetachedWindows
{
    /// <summary>
    /// Interaction logic for ManagerListDetached.xaml
    /// </summary>
    public partial class ManagerListDetached : Window
    {
        private PeriodicTimer DataGenerationTimer;
        private ManagerMonitoring managerMonitoring = new();
        private bool IsRunning = false;


        // remove later
        private List<DateTime> RamDateTime = new();
        private List<DateTime> CpuDateTime = new();
        private List<DateTime> ReadDateTime = new();
        private List<DateTime> WrittenDateTime = new();

        private List<long> RamReadings = new();
        private List<long> CpuReadings = new();
        private List<long> ReadReadings = new();
        private List<long> WrittenReadings = new();
        //

        public ManagerListDetached()
        {
            InitializeComponent();

            DataUtilities.DatabaseHandler = new SqlDatabase();             //
            Conversion conv = new();                                       //
            conv.Executions = DataUtilities.GetExecutions();               // Remove once the program is set up for the database simulation.
            conv.ActiveExecution.Managers = DataUtilities.GetManagers();   //
            conv.HealthReport = DataUtilities.BuildHealthReport();         //
            DataUtilities.AddHealthReportReadings(conv.HealthReport);      //

            Random random = new(5);

            foreach (Manager manager in conv.ActiveExecution.Managers)
            {
                manager.Readings.Add(new ManagerUsage(random.Next() % 100, random.Next() % 100, random.Next() % 100, DateTime.Now)); // Temp data, remove later
                datagridManagers.Items.Add(manager);
            }
        }

        private void AddManager_Click(object sender, RoutedEventArgs e)
        {
            Manager? selectedManager = datagridManagers.SelectedItem as Manager;

            if (datagridManagers.SelectedItems.Count > 1)
            {
                foreach (Manager manager in datagridManagers.SelectedItems)
                {
                    if (!datagridManagerDetails.Items.Contains(manager))
                    {
                        DatagridManagerMover("Add", manager);

                        managerMonitoring.AddLine(CpuDateTime, CpuReadings, gridChartCPUload, manager.Name.Split('.').Last(), manager.Name.Split('.').Last());
                        managerMonitoring.AddLine(RamDateTime, RamReadings, gridChartRAMusage, manager.Name.Split('.').Last(), manager.Name.Split('.').Last());
                        managerMonitoring.AddLine(ReadDateTime, ReadReadings, gridChartRowsRead, manager.Name.Split('.').Last(), manager.Name.Split('.').Last());
                        managerMonitoring.AddLine(WrittenDateTime, WrittenReadings, gridChartRowsWritten, manager.Name.Split('.').Last(), manager.Name.Split('.').Last());
                    }
                }
            }
            else if (!datagridManagerDetails.Items.Contains(selectedManager))
            {
                DatagridManagerMover("Add", selectedManager);
            }
            datagridManagers.SelectedItems.Clear();

            if (IsRunning == false)
            {
                DataGenerationTimer = new(TimeSpan.FromSeconds(1));
                managerMonitoring.GenerateData(DataGenerationTimer, chartCPUload);
                managerMonitoring.GenerateData(DataGenerationTimer, chartRAMusage);
                managerMonitoring.GenerateData(DataGenerationTimer, chartRowsRead);
                managerMonitoring.GenerateData(DataGenerationTimer, chartRowsWritten);
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