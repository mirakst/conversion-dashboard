using System.Windows;
using DashboardFrontend.ViewModels;
using System.Windows.Input;

namespace DashboardFrontend.DetachedWindows
{
    public partial class LogDetached
    {
        public LogDetached(LogViewModel logViewModel)
        {
            InitializeComponent();
            logViewModel.LogListView = ListViewLog;
            DataContext = logViewModel;
            ListViewLog.Loaded += logViewModel.ScrollToLast;
        }

        /// <summary>
        /// Disables autoscroll when mouse is over log.
        /// </summary>
        private void ListViewLog_MouseOverChanged(object sender, MouseEventArgs e)
        {
            var vm = (LogViewModel)DataContext;
            vm.DoAutoScroll = !vm.DoAutoScroll;
        }

        /// <summary>
        /// Updates view and scrolls to bottom.
        /// </summary>
        private void ContextIdCheckbox_OnToggle(object sender, RoutedEventArgs e)
        {
            var vm = (LogViewModel) DataContext;
            vm.MessageView.Refresh();
            vm.ScrollToLast();
        }

        /// <summary>
        /// Enables or disables all managers in the context ID filter.
        /// </summary>
        private void ContextIdFilter_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = (LogViewModel)DataContext;
            bool buttonVal = bool.Parse((string)((FrameworkElement)sender).Tag);
            if (vm.SelectedExecution is null) return;
            foreach (var manager in vm.SelectedExecution.Managers)
            {
                manager.IsChecked = buttonVal;
            }
            vm.MessageView.Refresh();
            vm.ScrollToLast();
        }

        /// <summary>
        /// Adds a mouse-click handler to the surrounding window, to emulate popup behavior on log filter.
        /// </summary>
        private void GridPopup_Opened(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.AddHandler(UIElement.MouseDownEvent, (MouseButtonEventHandler)GridPopupLogFilter_PreviewMouseDown, true);
        }

        /// <summary>
        /// If mouse is clicked and it's not on the log filter button or the log filter popup, close the popup.
        /// </summary>
        private void GridPopupLogFilter_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!GridPopupLogFilter.IsMouseOver && !ButtonLogFilter.IsMouseOver)
            {
                ButtonLogFilter.IsChecked = false;
                this.RemoveHandler(UIElement.MouseDownEvent, (MouseButtonEventHandler)GridPopupLogFilter_PreviewMouseDown);
            }
        }
    }
}
