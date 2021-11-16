using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardFrontend.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel(Log log)
        {
            LogViewModel = new(log);
        }

        public LogViewModel LogViewModel { get; set; }

        private bool _enableLogBtn = true;
        public bool EnableLogButton
        {
            get => _enableLogBtn;
            set 
            { 
                _enableLogBtn = value;
                OnPropertyChanged(nameof(EnableLogButton));
            }
        }
        private bool _healthReportIsOpen;
        public bool HealthReportIsOpen
        {
            get => _healthReportIsOpen;
            set
            {
                _healthReportIsOpen = value;
                OnPropertyChanged(nameof(HealthReportIsOpen));
            }
        }
        private bool _managerListIsOpen;
        public bool ManagerListIsOpen
        {
            get => _managerListIsOpen;
            set
            {
                _managerListIsOpen = value;
                OnPropertyChanged(nameof(ManagerListIsOpen));
            }
        }
        private bool _validationsIsOpen;
        public bool ValidationsIsOpen
        {
            get => _validationsIsOpen;
            set
            {
                _validationsIsOpen = value;
                OnPropertyChanged(nameof(ValidationsIsOpen));
            }
        }
    }
}
