using DashboardBackend;
using DashboardBackend.Database;
using DashboardFrontend.ViewModels;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Model;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        #region Timers
        private readonly PeriodicTimer CPUTimer;
        private readonly PeriodicTimer RAMTimer;
        private readonly PeriodicTimer ReadTimer;
        private readonly PeriodicTimer WrittenTimer;
        public List<PeriodicTimer> Timers;
        #endregion

        #region Performance objects
        public ManagerPerformanceViewModel CPUPerformance { get; private set; } = new("load");
        public ManagerPerformanceViewModel RAMPerformance { get; private set; } = new("load");
        public ManagerPerformanceViewModel ReadPerformance { get; private set; } = new("Rows");
        public ManagerPerformanceViewModel WrittenPerformance { get; private set; } = new("Rows");
        public List<ManagerPerformanceViewModel> DataCollections { get; private set; } = new();
        #endregion

        #region Chart objects
        public LiveChartViewModel CPUChart { get; private set; } = new();
        public LiveChartViewModel RAMChart { get; private set; } = new();
        public LiveChartViewModel ReadChart { get; private set; } = new();
        public LiveChartViewModel WrittenChart { get; private set; } = new();
        public List<LiveChartViewModel> Charts { get; private set; } = new();
        #endregion

        public ManagerListDetached()
        {
            InitializeComponent();

            DataUtilities.DatabaseHandler = new SqlDatabase();                                                                          // Remove later once the Start monitoring functions has been made
            Conversion conv = new();                                                                                                    //
            conv.Executions = DataUtilities.GetExecutions();                                                                            //
            conv.ActiveExecution.Managers = DataUtilities.GetManagers();                                                                //
            conv.HealthReport = DataUtilities.BuildHealthReport();                                                                      //
            DataUtilities.AddHealthReportReadings(conv.HealthReport);                                                                   //
                                                                                                                                        //
            Random random = new(5);                                                                                                     //
            foreach (Manager manager in conv.ActiveExecution.Managers)                                                                  //
            {                                                                                                                           //
                manager.Readings.Add(new ManagerUsage(random.Next() % 100, random.Next() % 100, random.Next() % 100, DateTime.Now));    //
                datagridManagers.Items.Add(manager);                                                                                    //
            }                                                                                                                           // Down to here

            #region Timers
            CPUTimer = new(TimeSpan.FromSeconds(1));
            RAMTimer = new(TimeSpan.FromSeconds(1));
            ReadTimer = new(TimeSpan.FromSeconds(1));
            WrittenTimer = new(TimeSpan.FromSeconds(1));
            #endregion

            #region Lists
            Charts = new List<LiveChartViewModel> { CPUChart, RAMChart, ReadChart, WrittenChart };
            DataCollections = new List<ManagerPerformanceViewModel> { CPUPerformance, RAMPerformance, ReadPerformance, WrittenPerformance };
            Timers = new List<PeriodicTimer> { CPUTimer, RAMTimer, ReadTimer, WrittenTimer };
            #endregion

            int i = 0;
            foreach (LiveChartViewModel chart in Charts)
            {
                chart.NewChart(DataCollections[i].Series, DataCollections[i].ManagerData, DataCollections[i].XAxis, DataCollections[i].YAxis);
                chart.StartGraph(Timers[i]);
                i++;
            }

            DataContext = this;
        }

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
                }
            }
            else if (!datagridManagerDetails.Items.Contains(selectedManager))
            {
                DatagridManagerMover("Add", selectedManager);
            }
            datagridManagers.SelectedItems.Clear();
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
            CPUChart.Series.Clear();
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
                    AddChartLinesHelper(manager);
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

        public void AddChartLinesHelper(Manager manager)
        {   
            foreach (LiveChartViewModel chart in Charts)
            {
                var managerValues = new ObservableCollection<ObservablePoint>();

                chart.AddData(new LineSeries<ObservablePoint>
                {
                    Name = $"{manager.Name}",
                    Stroke = new SolidColorPaint(new SKColor(133, 222, 118)),
                    Fill = null,
                    GeometryFill = new SolidColorPaint(new SKColor(133, 222, 118)),
                    GeometryStroke = new SolidColorPaint(new SKColor(133, 222, 118)),
                    GeometrySize = 0.4,
                }, managerValues);
            }
        }

        private void CartesianChart_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CPUChart.AutoFocusOn();
            RAMChart.AutoFocusOn();
            ReadChart.AutoFocusOn();
            WrittenChart.AutoFocusOn();
        }

        private void CartesianChart_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CPUChart.AutoFocusOff();
            RAMChart.AutoFocusOff();
            ReadChart.AutoFocusOff();
            WrittenChart.AutoFocusOff();
        }
    }
}