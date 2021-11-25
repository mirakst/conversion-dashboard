﻿using DashboardBackend;
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
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DashboardFrontend.DetachedWindows
{
    /// <summary>
    /// Interaction logic for ManagerListDetached.xaml
    /// </summary>
    public partial class ManagerListDetached : Window
    {

        #region Performance objects
        public ManagerPerformanceViewModel CPUPerformance { get; private set; } = new("load");
        public ManagerPerformanceViewModel RAMPerformance { get; private set; } = new("load");
        public ManagerPerformanceViewModel ReadPerformance { get; private set; } = new("Rows");
        public ManagerPerformanceViewModel WrittenPerformance { get; private set; } = new("Rows");
        public List<ManagerPerformanceViewModel> DataCollections { get; private set; } = new();
        #endregion

        #region Chart objects
        public LiveChartViewModel CPUChart { get; private set; }
        public LiveChartViewModel RAMChart { get; private set; }
        public LiveChartViewModel ReadChart { get; private set; }
        public LiveChartViewModel WrittenChart { get; private set; }
        public List<LiveChartViewModel> Charts { get; private set; } = new();
        #endregion

        public ManagerListDetached()
        {
            #region Charts
            CPUChart = new(CPUPerformance.Series, CPUPerformance.ManagerData, CPUPerformance.XAxis, CPUPerformance.YAxis);
            RAMChart = new(RAMPerformance.Series, RAMPerformance.ManagerData, RAMPerformance.XAxis, RAMPerformance.YAxis);
            ReadChart = new(ReadPerformance.Series, ReadPerformance.ManagerData, ReadPerformance.XAxis, ReadPerformance.YAxis);
            WrittenChart = new(ReadPerformance.Series, ReadPerformance.ManagerData, ReadPerformance.XAxis, ReadPerformance.YAxis);
            #endregion

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
                
                datagridManagers.Items.Add(manager);                                                                                    
            }                                                                                                                           

            #region Lists
            Charts = new List<LiveChartViewModel> { CPUChart, RAMChart, ReadChart, WrittenChart };
            DataCollections = new List<ManagerPerformanceViewModel> { CPUPerformance, RAMPerformance, ReadPerformance, WrittenPerformance };
            #endregion

            DataContext = this;
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

        private void TextboxSearchbar_TextChanged(object sender, TextChangedEventArgs e)
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
                    datagridManagerCharts.Items.Add(new ManagerWrapper(manager));
                    AddChartLinesHelper(manager);
                    break;

                case "Remove":
                    datagridManagerDetails.Items.Remove(manager);
                    datagridManagerCharts.Items.Remove(manager);
                    RemoveChartLinesHelper(manager);
                    break;

                case "Clear":
                    datagridManagerDetails.Items.Clear();
                    datagridManagerCharts.Items.Clear();
                    ClearChartLinesHelper();
                    break;
            }
        }

        public void AddChartLinesHelper(Manager manager)
        {
            ManagerWrapper wrappedManager = new(manager);

            foreach (LiveChartViewModel chart in Charts)
            {
                var managerValues = new ObservableCollection<ObservablePoint>();

                chart.AddData(new LineSeries<ObservablePoint>
                {
                    Name = $"{wrappedManager.manager.Name.Split(".").Last()}",
                    Fill = null,
                    Stroke = new SolidColorPaint(new SKColor(wrappedManager.R, wrappedManager.G, wrappedManager.B), 3),
                    GeometryFill = new SolidColorPaint(new SKColor(wrappedManager.R, wrappedManager.G, wrappedManager.B)),
                    GeometryStroke = new SolidColorPaint(new SKColor(wrappedManager.R, wrappedManager.G, wrappedManager.B)),
                    GeometrySize = 0.4,
                    TooltipLabelFormatter = e => manager.Name.Split(".").Last() + "\n" +
                                                 "ID: " + manager.Id + " Execution " + manager.ExecutionId + "\n" +
                                                 DateTime.FromOADate(e.SecondaryValue).ToString("HH:mm:ss") + "\n" +
                                                 e.PrimaryValue.ToString("P"),
                }, managerValues);
            }
        }

        public void RemoveChartLinesHelper(Manager manager)
        {
            foreach (LiveChartViewModel chart in Charts)
            {
                var managerValues = new ObservableCollection<ObservablePoint>();
                chart.RemoveData(manager.Name, managerValues);
            }
        }

        public void ClearChartLinesHelper()
        {
            foreach (LiveChartViewModel chart in Charts)
            {
                chart.Series.Clear();
            }
        }

        private void CartesianChart_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            foreach (LiveChartViewModel chart in Charts)
            {
                chart.AutoFocusOn();
            }
        }

        private void CartesianChart_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            foreach (LiveChartViewModel chart in Charts)
            {
                chart.AutoFocusOff();
            }
        }
    }
}