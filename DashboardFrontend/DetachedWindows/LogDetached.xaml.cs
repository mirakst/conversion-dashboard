using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using DashboardFrontend.NewViewModels;

namespace DashboardFrontend.DetachedWindows
{
    public partial class LogWindow
    {
        private bool _listViewLogShouldAutoScroll;

        public LogWindow(NewLogViewModel logViewModel)
        {
            InitializeComponent();
            ((ICollectionView)ListViewLog.Items).CollectionChanged += ListViewLog_CollectionChanged;
            DataContext = logViewModel;
        }

        private void ContextIdCheckbox_OnToggle(object sender, RoutedEventArgs e)
        {
            //LogViewModel? vm = (LogViewModel)DataContext;
            //vm.MessageView.Refresh();
            //vm.ScrollToLast();
        }

        private void ContextIdFilter_OnClick(object sender, RoutedEventArgs e)
        {
            //LogViewModel? vm = (LogViewModel)DataContext;
            //bool buttonVal = bool.Parse((string)((FrameworkElement)sender).Tag);
            //if (vm.SelectedExecution is null) return;
            //foreach (ManagerObservable? manager in vm.SelectedExecution.Managers)
            //{
            //    manager.IsChecked = buttonVal;
            //}
            //vm.MessageView.Refresh();
            //vm.ScrollToLast();
        }

        private void GridPopup_Opened(object sender, DependencyPropertyChangedEventArgs e)
        {
            //AddHandler(MouseDownEvent, GridPopupLogFilter_PreviewMouseDown, true);
        }

        private void GridPopupLogFilter_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (!GridPopupLogFilter.IsMouseOver && !ButtonLogFilter.IsMouseOver)
            //{
            //    ButtonLogFilter.IsChecked = false;
            //    RemoveHandler(MouseDownEvent, GridPopupLogFilter_PreviewMouseDown);
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
    }
}
