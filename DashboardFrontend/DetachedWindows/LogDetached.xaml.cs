using DashboardFrontend.ViewModels;

namespace DashboardFrontend.DetachedWindows
{
    public partial class LogDetached
    {
        public LogDetached(LogViewModel logViewModel)
        {
            InitializeComponent();
            DataContext = logViewModel;
        }
    }
}
