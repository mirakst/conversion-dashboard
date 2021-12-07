using System.Windows;
using System.Windows.Controls;

namespace DashboardFrontend.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel(ListView listViewLog)
        {
            Controller = new(this);
            Controller.InitializeViewModels(listViewLog);
        }

        public Controller Controller { get; set; }
        private string _currentStatus;
        public string CurrentStatus
        {
            get => _currentStatus;
            set
            {
                _currentStatus = value;
                OnPropertyChanged(nameof(CurrentStatus));
            }
        }
        private int _currentProgress;
        public int CurrentProgress
        {
            get => _currentProgress;
            set
            {
                _currentProgress = value;
                OnPropertyChanged(nameof(CurrentProgress));
            }
        }
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
        private Visibility _loadingVisibility = Visibility.Collapsed;
        public Visibility LoadingVisibility
        {
            get => _loadingVisibility;
            set
            {
                _loadingVisibility = value;
                OnPropertyChanged(nameof(LoadingVisibility));
            }
        }
        public LogViewModel LogViewModel { get; set; }
        public ValidationReportViewModel ValidationReportViewModel { get; set; }
        public HealthReportViewModel HealthReportViewModel { get; set; }
        public ManagerViewModel ManagerViewModel {  get; set; }

        public void UpdateView()
        {
            OnPropertyChanged(nameof(LogViewModel));
            OnPropertyChanged(nameof(ValidationReportViewModel));
            OnPropertyChanged(nameof(HealthReportViewModel));
            OnPropertyChanged(nameof(ManagerViewModel));
        }

        public void UpdateExecutionProgress(int currentProgress)
        {
            CurrentProgress = currentProgress;
        }
    }
}
