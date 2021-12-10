using DashboardFrontend.DetachedWindows;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DashboardFrontend.NewViewModels;
using DashboardBackend;
using DashboardBackend.Settings;
using System.Collections.Specialized;

namespace DashboardFrontend
{
    public partial class MainWindow : Window
    {
        private bool _listViewLogShouldAutoScroll = true;

        public MainWindow()
        {
            // Setup views
            MaxHeight = SystemParameters.WorkArea.Height;
            MaxWidth = SystemParameters.WorkArea.Width;
            InitializeComponent();
            ((ICollectionView)ListViewLog.Items).CollectionChanged += ListViewLog_CollectionChanged;

            // Setup back-end
            IUserSettings userSettings = new UserSettings();
            IDataHandler dataHandler = new DataHandler();
            Controller = new DashboardController(userSettings, dataHandler);
            DataContext = new MainViewModel(Controller);
        }

        public IDashboardController Controller { get; }

        #region Main Window Events
        public void ButtonStartStopClick(object sender, RoutedEventArgs e)
        {
            Controller.ChangeMonitoringState();
        }

        public void ButtonSettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new(Controller);
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        public void DetachManagerButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void DetachLogButtonClick(object sender, RoutedEventArgs e)
        {
            var viewModel = new NewLogViewModel(Controller);
            var logWindow = new LogWindow(viewModel);
            logWindow.Show();
        }

        public void DetachValidationReportButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void DetachHealthReportButtonClick(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Window Commands
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
        #endregion

        #region Health Report Component
        private void CartesianChart_MouseLeave(object sender, MouseEventArgs e)
        {
            //DataChart? chart = (DataChart)(sender as CartesianChart)?.DataContext!;
            //chart?.AutoFocusOn();
        }

        private void CartesianChart_MouseEnter(object sender, MouseEventArgs e)
        {
            //DataChart? chart = (DataChart)(sender as CartesianChart)?.DataContext!;
            //chart?.AutoFocusOff();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (ViewModel is null) return;
            //_ = int.TryParse(((FrameworkElement)ComboBoxMaxView.SelectedItem).Tag as string, out int comboBoxItemValue);
            //ViewModel.HealthReportViewModel.SystemLoadChart.ChangeMaxView(comboBoxItemValue);
            //ViewModel.HealthReportViewModel.NetworkChart.ChangeMaxView(comboBoxItemValue);
            //ViewModel.HealthReportViewModel.NetworkDeltaChart.ChangeMaxView(comboBoxItemValue);
            //ViewModel.HealthReportViewModel.NetworkSpeedChart.ChangeMaxView(comboBoxItemValue);
        }
        #endregion

        #region Validation Report Component
        /// <summary>
        /// Called once a TreeViewItem is expanded. Gets the item's ManagerValidationsWrapper, and adds the manager name to a list of expanded TreeViewItems in the Validation Report viewmodel.
        /// </summary>
        /// <remarks>This ensures that the items stay expanded when the data is updated/refreshed.</remarks>
        private void TreeViewValidations_Expanded(object sender, RoutedEventArgs e)
        {
            //TreeView tree = (TreeView)sender;
            //TreeViewItem item = (TreeViewItem)e.OriginalSource;
            //if (tree.ItemContainerGenerator.ItemFromContainer(item) is ManagerObservable manager)
            //{
            //    if (!ViewModel.ValidationReportViewModel.ExpandedManagerNames.Contains(manager.Name))
            //    {
            //        ViewModel.ValidationReportViewModel.ExpandedManagerNames.Add(manager.Name);
            //    }
            //}
        }

        /// <summary>
        /// Called once a TreeViewItem is collapsed. Gets the item's ManagerValidationsWrapper, and removes the manager name to a list of expanded TreeViewItems in the Validation Report viewmodel.
        /// </summary>
        private void TreeViewValidations_Collapsed(object sender, RoutedEventArgs e)
        {
            //TreeView tree = (TreeView)sender;
            //TreeViewItem item = (TreeViewItem)e.OriginalSource;
            //item.IsSelected = false;
            //if (tree.ItemContainerGenerator.ItemFromContainer(item) is ManagerObservable manager)
            //{
            //    if (ViewModel.ValidationReportViewModel.ExpandedManagerNames.Contains(manager.Name))
            //    {
            //        ViewModel.ValidationReportViewModel.ExpandedManagerNames.Remove(manager.Name);
            //    }
            //}
        }

        private void CopySrcSql_Click(object sender, RoutedEventArgs e)
        {
            //var button = (Button)sender;
            //if (button.DataContext is ValidationTest test)
            //{
            //    Clipboard.SetText(test.SrcSql);
            //    TextBlockPopupSql.Content = "SQL source copied to clipboard";
            //    PopupCopySql.IsOpen = true;
            //}
        }

        private void CopyDestSql_Click(object sender, RoutedEventArgs e)
        {
            //var button = (Button)sender;
            //if (button.DataContext is ValidationTest test)
            //{
            //    Clipboard.SetText(test.DstSql);
            //    TextBlockPopupSql.Content = "SQL destination copied to clipboard";
            //    PopupCopySql.IsOpen = true;
            //}
        }

        private void ButtonCopySql_MouseLeave(object sender, MouseEventArgs e)
        {
            //PopupCopySql.IsOpen = false;
        }
        #endregion

        #region Log Component
        private void ContextIdCheckbox_OnToggle(object sender, RoutedEventArgs e)
        {
            //ViewModel.LogViewModel.MessageView.Refresh();
            //ViewModel.LogViewModel.ScrollToLast();
        }

        private void ContextIdFilter_OnClick(object sender, RoutedEventArgs e)
        {
            //bool buttonVal = bool.Parse((string) ((FrameworkElement) sender).Tag);
            //if (ViewModel.LogViewModel.SelectedExecution is null) return;
            //foreach (var manager in ViewModel.LogViewModel.SelectedExecution.Managers)
            //{
            //    manager.IsChecked = buttonVal;
            //}
            //ViewModel.LogViewModel.MessageView.Refresh();
            //ViewModel.LogViewModel.ScrollToLast();
        }

        private void GridPopup_Opened(object sender, DependencyPropertyChangedEventArgs e)
        {
            //this.AddHandler(UIElement.MouseDownEvent, (MouseButtonEventHandler)GridPopupLogFilter_PreviewMouseDown, true);
        }

        private void GridPopupLogFilter_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (!GridPopupLogFilter.IsMouseOver && !ButtonLogFilter.IsMouseOver)
            //{
            //    ButtonLogFilter.IsChecked = false;
            //    this.RemoveHandler(UIElement.MouseDownEvent, (MouseButtonEventHandler)GridPopupLogFilter_PreviewMouseDown);
            //}
        }

        private void ListViewLog_MouseOverChanged(object sender, MouseEventArgs e)
        {
            _listViewLogShouldAutoScroll = !_listViewLogShouldAutoScroll;
        }

        private void ListViewLog_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is ItemCollection l && l.Count > 0 && _listViewLogShouldAutoScroll) 
            {
                int lastIndex = l.Count - 1;
                ListViewLog.ScrollIntoView(ListViewLog.Items[lastIndex]);
            }
        }
        #endregion
    }
}