using Model;
using System.Linq;
using System.Collections.Generic;
using static Model.ValidationTest;
using System.Collections.ObjectModel;

namespace DashboardFrontend.ViewModels
{
    public class ValidationTestViewModel : BaseViewModel
    {
        public ValidationTestViewModel(string manager)
        {
            ManagerName = manager;
        }

        #region Properties
        public string ManagerName { get; set; }
        public ObservableCollection<ValidationTest> Tests { get; set; } = new();

        private double _score;
        public double Score
        {
            get => _score;
            set
            {
                _score = value;
                OnPropertyChanged(nameof(Score));
            }
        }
        private bool _showOk;
        public bool ShowOk
        {
            get => _showOk;
            set
            {
                _showOk = value;
                OnPropertyChanged(nameof(ShowOk));
            }
        }
        private bool _showDisabled = true;
        public bool ShowDisabled
        {
            get => _showDisabled;
            set
            {
                _showDisabled = value;
                OnPropertyChanged(nameof(ShowDisabled));
            }
        }
        private bool _showFailed = true;
        public bool ShowFailed
        {
            get => _showFailed;
            set
            {
                _showFailed = value;
                OnPropertyChanged(nameof(ShowFailed));
            }
        }
        private int _okCount;
        public int OkCount
        {
            get => _okCount;
            set
            {
                _okCount = value;
                OnPropertyChanged(nameof(OkCount));
            }
        }
        private int _disabledCount;
        public int DisabledCount
        {
            get => _disabledCount;
            set
            {
                _disabledCount = value;
                OnPropertyChanged(nameof(DisabledCount));
            }
        }
        private int _failedCount;
        public int FailedCount
        {
            get => _failedCount;
            set
            {
                _failedCount = value;
                OnPropertyChanged(nameof(FailedCount));
            }
        }
        private int _totalCount;
        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                OnPropertyChanged(nameof(TotalCount));
            }
        }
        #endregion

        public void AddTest(ValidationTest test)
        {
            Tests.Add(test);
            TotalCount++;
            switch(test.Status)
            {
                case ValidationStatus.Disabled:
                    DisabledCount++;
                    break;
                case ValidationStatus.Failed:
                case ValidationStatus.FailMismatch:
                    FailedCount++;
                    break;
                case ValidationStatus.Ok:
                    OkCount++;
                    break;
                default:
                    return;
            }
            Score = (double)OkCount / (double)(TotalCount - DisabledCount) * 100.0d;
        }
    }
}
