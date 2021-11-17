using Model;
using System.Linq;
using System.Collections.Generic;
using static Model.ValidationTest;

namespace DashboardFrontend.ViewModels
{
    public class ValidationTestViewModel
    {
        public ValidationTestViewModel(string manager)
        {
            ManagerName = manager;
        }

        public string ManagerName { get; set; }
        public List<ValidationTest> TestsOk { get; set; } = new();
        public List<ValidationTest> TestsDisabled { get; set; } = new();
        public List<ValidationTest> TestsFailed { get; set; } = new();
        public int OkCount => TestsOk.Count;
        public int DisabledCount => TestsDisabled.Count;
        public int FailedCount => TestsFailed.Count;
        public int TotalCount => OkCount + DisabledCount + FailedCount;

        public void AddTest(ValidationTest test)
        {
            switch(test.Status)
            {
                case ValidationStatus.Disabled:
                    TestsDisabled.Add(test);
                    break;
                case ValidationStatus.Failed:
                case ValidationStatus.FailMismatch:
                    TestsFailed.Add(test);
                    break;
                case ValidationStatus.Ok:
                    TestsOk.Add(test);
                    break;
                default:
                    return;
            }
        }
    }
}
