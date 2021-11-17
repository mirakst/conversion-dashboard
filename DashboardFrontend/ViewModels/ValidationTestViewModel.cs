using Model;
using System.Linq;
using System.Collections.Generic;
using static Model.ValidationTest;

namespace DashboardFrontend.ViewModels
{
    public class ValidationTestViewModel
    {
        public ValidationTestViewModel(string manager, string managerNameFull, List<ValidationTest> tests)
        {
            ManagerName = manager;
            ManagerNameFull = managerNameFull;
            Tests = tests;
        }

        public string ManagerName { get; set; }
        public string ManagerNameFull { get; set; }
        public List<ValidationTest> Tests { get; set; }
        public int OkCount => Tests.Count(t => t.Status == ValidationStatus.Ok);
        public int DisabledCount => Tests.Count(t => t.Status == ValidationStatus.Disabled);
        public int FailedCount => Tests.Count(t => t.Status is ValidationStatus.Failed or ValidationStatus.FailMismatch);
        public int TotalCount => Tests.Count;
    }
}
