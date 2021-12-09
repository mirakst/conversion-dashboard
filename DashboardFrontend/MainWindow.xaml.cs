using DashboardFrontend.DetachedWindows;
using System;
using DashboardFrontend.ViewModels;
using LiveChartsCore.SkiaSharpView.WPF;
using Model;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DashboardFrontend.Charts;
using System.Linq;

namespace DashboardFrontend
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            MaxHeight = SystemParameters.WorkArea.Height;
            MaxWidth = SystemParameters.WorkArea.Width;
            InitializeComponent();
            ViewModel = new(ListViewLog, this);
            ViewModel.ManagerViewModel.DataGridManagers = datagridManagers;
            DataContext = ViewModel;
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An unexpected error occured: {e.Exception.Message}", "Error");
            e.Handled = true;
        }

        public MainWindowViewModel ViewModel { get; }

        public void ButtonStartStopClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Controller.OnStartPressed();
        }

        //Detach window events
        public void ButtonSettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new(ViewModel.Controller);
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        public async void DetachManagerButtonClick(object sender, RoutedEventArgs e)
        {
            ManagerViewModel detachedManagerViewModel = ViewModel.Controller.CreateManagerViewModel();
            ManagerListDetached detachedManagerWindow = new(detachedManagerViewModel);
            detachedManagerViewModel.Window = detachedManagerWindow;
            detachedManagerViewModel.DataGridManagers = detachedManagerWindow.DatagridManagers;
            detachedManagerWindow.Show();
            detachedManagerWindow.Closed += delegate
            {
                // Ensures that the ViewModel is only removed from the controller after its data has been modified, preventing an InvalidOperationException.
                _ = Task.Run(() =>
                {
                    while (ViewModel.Controller.ShouldUpdateManagers) { }
                    ViewModel.Controller.ManagerViewModels.Remove(detachedManagerViewModel);
                });
            };
            await Task.Delay(5);
            if (ViewModel.ManagerViewModel.SelectedExecution == null) return;
            int selectedExecutionId = ViewModel.ManagerViewModel.SelectedExecution.Id;
            detachedManagerViewModel.SelectedExecution = detachedManagerViewModel.Executions[selectedExecutionId - 1];
        }

        public async void DetachLogButtonClick(object sender, RoutedEventArgs e)
        {
            LogViewModel detachedLogViewModel = ViewModel.Controller.CreateLogViewModel();
            LogDetached detachLog = new(detachedLogViewModel);
            detachLog.Show();
            detachLog.Closed += delegate
            {
                _ = Task.Run(() =>
                {
                    while (ViewModel.Controller.ShouldUpdateLog) { }
                    ViewModel.Controller.LogViewModels.Remove(detachedLogViewModel);
                });
            };
            await Task.Delay(5);
            int selectedExecutionId = ViewModel.LogViewModel.SelectedExecution.Id;
            detachedLogViewModel.SelectedExecution = detachedLogViewModel.Executions[selectedExecutionId - 1];
        }

        public async void DetachValidationReportButtonClick(object sender, RoutedEventArgs e)
        {
            ValidationReportViewModel detachedValidationReportViewModel =
                ViewModel.Controller.CreateValidationReportViewModel();
            ValidationReportDetached detachVr = new(detachedValidationReportViewModel);
            detachVr.Show();
            detachVr.Closed += delegate
            {
                _ = Task.Run(() =>
                {
                    while (ViewModel.Controller.ShouldUpdateLog) { }
                    ViewModel.Controller.ValidationReportViewModels.Remove(detachedValidationReportViewModel);
                });
            };
            await Task.Delay(5);
            int selectedExecutionId = ViewModel.ValidationReportViewModel.SelectedExecution.Id;
            detachedValidationReportViewModel.SelectedExecution = detachedValidationReportViewModel.Executions[selectedExecutionId - 1];
        }

        public void DetachHealthReportButtonClick(object sender, RoutedEventArgs e)
        {
            HealthReportViewModel detachedHealthReportViewModel = ViewModel.Controller.CreateHealthReportViewModel();
            HealthReportDetached detachHr = new(detachedHealthReportViewModel);
            detachHr.Show();
            detachHr.Closed += delegate
            {
                _ = Task.Run(() =>
                {
                    while (ViewModel.Controller.ShouldUpdateLog) { }
                    ViewModel.Controller.HealthReportViewModels.Remove(detachedHealthReportViewModel);
                });
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

        private void CommandBinding_Executed_2(object sender, ExecutedRoutedEventArgs? e)
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    ResizeMode = ResizeMode.CanResizeWithGrip;
                    WindowState = WindowState.Normal;
                    break;
                case WindowState.Normal:
                    ResizeMode = ResizeMode.NoResize;
                    WindowState = WindowState.Maximized;
                    break;
            }
        }

        private void CommandBinding_Executed_3(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void ControlGridClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                CommandBinding_Executed_2(this, null);
            }
            else
            {
                DragMove();
            }
        }

        //Performance events

        private void CartesianChart_MouseLeave(object sender, MouseEventArgs e)
        {
            DataChart? chart = (DataChart)(sender as CartesianChart)?.DataContext!;
            chart?.AutoFocusOn();
        }

        private void CartesianChart_MouseEnter(object sender, MouseEventArgs e)
        {
            DataChart? chart = (DataChart)(sender as CartesianChart)?.DataContext!;
            chart?.AutoFocusOff();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel is null) return;
            _ = int.TryParse(((FrameworkElement)ComboBoxMaxView.SelectedItem).Tag as string, out int comboBoxItemValue);
            ViewModel.HealthReportViewModel.SystemLoadChart.ChangeMaxView(comboBoxItemValue);
            ViewModel.HealthReportViewModel.NetworkChart.ChangeMaxView(comboBoxItemValue);
            ViewModel.HealthReportViewModel.NetworkDeltaChart.ChangeMaxView(comboBoxItemValue);
            ViewModel.HealthReportViewModel.NetworkSpeedChart.ChangeMaxView(comboBoxItemValue);
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
            if (tree.ItemContainerGenerator.ItemFromContainer(item) is ManagerObservable manager)
            {
                if (!ViewModel.ValidationReportViewModel.ExpandedManagerNames.Contains(manager.Name))
                {
                    ViewModel.ValidationReportViewModel.ExpandedManagerNames.Add(manager.Name);
                }
            }
        }

        /// <summary>
        /// Called once a TreeViewItem is collapsed. Gets the item's ManagerValidationsWrapper, and removes the manager name to a list of expanded TreeViewItems in the Validation Report viewmodel.
        /// </summary>
        private void TreeViewValidations_Collapsed(object sender, RoutedEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            item.IsSelected = false;
            if (tree.ItemContainerGenerator.ItemFromContainer(item) is ManagerObservable manager)
            {
                if (ViewModel.ValidationReportViewModel.ExpandedManagerNames.Contains(manager.Name))
                {
                    ViewModel.ValidationReportViewModel.ExpandedManagerNames.Remove(manager.Name);
                }
            }
        }

        private void CopySrcSql_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.DataContext is ValidationTest test)
            {
                Clipboard.SetText(test.SrcSql);
                TextBlockPopupSql.Content = "SQL source copied to clipboard";
                PopupCopySql.IsOpen = true;
            }
        }

        private void CopyDestSql_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.DataContext is ValidationTest test)
            {
                Clipboard.SetText(test.DstSql);
                TextBlockPopupSql.Content = "SQL destination copied to clipboard";
                PopupCopySql.IsOpen = true;
            }
        }

        private void ButtonCopySql_MouseLeave(object sender, MouseEventArgs e)
        {
            PopupCopySql.IsOpen = false;
        }

        private void ContextIdCheckbox_OnToggle(object sender, RoutedEventArgs e)
        {
            ViewModel.LogViewModel.MessageView.Refresh();
            ViewModel.LogViewModel.ScrollToLast();
        }

        private void ContextIdFilter_OnClick(object sender, RoutedEventArgs e)
        {
            bool buttonVal = bool.Parse((string) ((FrameworkElement) sender).Tag);
            if (ViewModel.LogViewModel.SelectedExecution is null) return;
            foreach (var manager in ViewModel.LogViewModel.SelectedExecution.Managers)
            {
                manager.IsChecked = buttonVal;
            }
            ViewModel.LogViewModel.MessageView.Refresh();
            ViewModel.LogViewModel.ScrollToLast();
        }

        private void GridPopup_Opened(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.AddHandler(UIElement.MouseDownEvent, (MouseButtonEventHandler)GridPopupLogFilter_PreviewMouseDown, true);
        }

        private void GridPopupLogFilter_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!GridPopupLogFilter.IsMouseOver && !ButtonLogFilter.IsMouseOver)
            {
                ButtonLogFilter.IsChecked = false;
                this.RemoveHandler(UIElement.MouseDownEvent, (MouseButtonEventHandler)GridPopupLogFilter_PreviewMouseDown);
            }
        }

        private async void DatagridManagers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewModel.Controller.ExpandManagerView((ManagerWrapper)datagridManagers.SelectedItem);
        }
    }
}