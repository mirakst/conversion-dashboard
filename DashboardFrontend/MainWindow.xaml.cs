using DashboardFrontend;
using DashboardFrontend.ViewModels;
using DashboardFrontend.DetachedWindows;
using DashboardFrontend.Settings;
using DashboardBackend;
using DashboardBackend.Database;
using Model;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;

namespace DashboardFrontend
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new(DataGridValidations, ListViewLog);
            DataContext = ViewModel;
        }

        public MainWindowViewModel ViewModel { get; }

        public void ButtonStartStopClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Controller.OnStartPressed();
        }

        //Detach window events
        public void ButtonSettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new(ViewModel.Controller.UserSettings);
            //settingsWindow.Closing += OnSettingsWindowClosing;
            //settingsWindow.IsEnabled = false;
            //settingsWindow.Owner = Application.Current.MainWindow;
            settingsWindow.ShowDialog();
        }

        public void DetachManagerButtonClick(object sender, RoutedEventArgs e)
        {
            //ManagerWindow detachManager = new();
            //detachManager.Closing += OnManagerWindowClosing;
            //buttonDetachManager.IsEnabled = false;
            //detachManager.Show();
        }

        public void DetachLogButtonClick(object sender, RoutedEventArgs e)
        {
            LogViewModel detachedLogViewModel = ViewModel.Controller.CreateLogViewModel();
            LogDetached detachLog = new(detachedLogViewModel);
            detachLog.Show();
            detachLog.Closed += delegate
            {
                ViewModel.Controller.LogViewModels.Remove(detachedLogViewModel);
            };
        }

        public void DetachValidationReportButtonClick(object sender, RoutedEventArgs e)
        {
            ValidationReportViewModel detachedValidationReportViewModel =
                ViewModel.Controller.CreateValidationReportViewModel();
            ValidationReportDetached detachVr = new(detachedValidationReportViewModel);
            detachedValidationReportViewModel.DataGrid = detachVr.DataGridValidations;
            detachVr.Show();
            detachVr.Closed += delegate
            {
                ViewModel.Controller.ValidationReportViewModels.Remove(detachedValidationReportViewModel);
            };
        }

        public void DetachHealthReportButtonClick(object sender, RoutedEventArgs e)
        {
            HealthReportViewModel detachedHealthReportViewModel = ViewModel.Controller.CreateHealthReportViewModel();
            HealthReportDetached detachHr = new(detachedHealthReportViewModel);
            detachHr.Show();
            detachHr.Closed += delegate
            {
                ViewModel.Controller.HealthReportViewModels.Remove(detachedHealthReportViewModel);
            };
        }

        //OnWindowClosing events
        private void OnSettingsWindowClosing(object? sender, CancelEventArgs e)
        {
            ButtonSettings.IsEnabled = true;
        }

        private void OnManagerWindowClosing(object sender, CancelEventArgs e)
        {
            ButtonDetachManager.IsEnabled = true;
        }

        private void ValidationsDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ButtonLogDetach.IsEnabled = true;
        }

        private void OnValidationWindowClosing(object? sender, CancelEventArgs e)
        {
            ButtonValidationReportDetach.IsEnabled = true;
        }

        private void OnHealthWindowClosing(object? sender, CancelEventArgs e)
        {
            ButtonHealthReportDetach.IsEnabled = true;
        }

        private void CommandBinding_CanExecute_1(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void CommandBinding_Executed_2(object sender, ExecutedRoutedEventArgs e)
        {

            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            this.ButtonMaximize.Visibility = Visibility.Collapsed;
            this.ButtonRestore.Visibility = Visibility.Visible;
        }

        private void CommandBinding_Executed_3(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void CommandBinding_Executed_4(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
            this.ButtonMaximize.Visibility = Visibility.Visible;
            this.ButtonRestore.Visibility = Visibility.Collapsed;
        }

        private void DraggableGrid(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //Performance events

        private void CartesianChart_MouseLeave(object sender, MouseEventArgs e)
        {
            ViewModel.HealthReportViewModel.SystemLoadChart.AutoFocusOn();

        }

        private void CartesianChart_MouseEnter(object sender, MouseEventArgs e)
        {
            ViewModel.HealthReportViewModel.SystemLoadChart.AutoFocusOff();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel is not null)
            {
                _ = int.TryParse(((FrameworkElement)ComboBoxMaxView.SelectedItem).Tag as string, out int comboBoxItemValue);
                ViewModel.HealthReportViewModel.SystemLoadChart.ChangeMaxView(comboBoxItemValue);
                ViewModel.HealthReportViewModel.NetworkChart.ChangeMaxView(comboBoxItemValue);
            }
        }

        private void ListViewLog_MouseOverChanged(object sender, MouseEventArgs e)
        {
            ViewModel.LogViewModel.DoAutoScroll = !ViewModel.LogViewModel.DoAutoScroll;
        }

        /// <summary>
        /// Called once a TreeViewItem is expanded. Gets the item's ManagerValidationsWrapper, and adds the manager name to a list of expanded TreeViewItems in the Validation Report viewmodel.
        /// </summary>
        /// <remarks>This ensures that the items stay expanded when the data is updated/refreshed.</remarks>
        private void TreeViewValidations_Expanded(object sender, RoutedEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            var wrapper = (ManagerValidationsWrapper)tree.ItemContainerGenerator.ItemFromContainer(item);
            if (!ViewModel.ValidationReportViewModel.ExpandedManagerNames.Contains(wrapper.ManagerName))
            {
                ViewModel.ValidationReportViewModel.ExpandedManagerNames.Add(wrapper.ManagerName);
            }
        }

        /// <summary>
        /// Called once a TreeViewItem is collapsed. Gets the item's ManagerValidationsWrapper, and removes the manager name to a list of expanded TreeViewItems in the Validation Report viewmodel.
        /// </summary>
        private void TreeViewValidations_Collapsed(object sender, RoutedEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            var wrapper = (ManagerValidationsWrapper)tree.ItemContainerGenerator.ItemFromContainer(item);
            ViewModel.ValidationReportViewModel.ExpandedManagerNames.Remove(wrapper.ManagerName);
        }

        private void CopySrcSql_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.DataContext is ValidationTest test)
            {
                Clipboard.SetText(test.SrcSql);
            }
        }

        private void CopyDestSql_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.DataContext is ValidationTest test)
            {
                Clipboard.SetText(test.DstSql);
            }
        }
    }
}