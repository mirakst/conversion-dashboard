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
        }

        private void ListViewLog_MouseOverChanged(object sender, MouseEventArgs e)
        {
            var vm = (LogViewModel)DataContext;
            vm.DoAutoScroll = !vm.DoAutoScroll;
        }
    }
}
