using System.Windows.Controls;

namespace DashboardFrontend.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel(DataGrid dataGridValidations)
        {
            Controller = new(this);
            Controller.InitializeViewModels(dataGridValidations);
        }

        public Controller Controller { get; set; }
        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }
        public LogViewModel LogViewModel { get; set; }
        public ValidationReportViewModel ValidationReportViewModel { get; set; }
        public HealthReportViewModel HealthReportViewModel { get; set; }
        public ManagerViewModel ManagerViewModel {  get; set; }
    }
}
