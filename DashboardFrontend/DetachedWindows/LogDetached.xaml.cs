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

        private void ListViewLog_MouseOverChanged(object sender, MouseEventArgs e)
        {
            var vm = (LogViewModel)DataContext;
            vm.DoAutoScroll = !vm.DoAutoScroll;
        }

        private void ContextIdCheckbox_OnToggle(object sender, RoutedEventArgs e)
        {
            var vm = (LogViewModel) DataContext;
            vm.MessageView.Refresh();
            vm.ScrollToLast();
        }

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
    }
}
