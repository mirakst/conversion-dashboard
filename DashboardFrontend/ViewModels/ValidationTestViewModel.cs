using Model;
using System.Linq;
using System.Collections.Generic;

namespace DashboardFrontend.ViewModels
{
    public class ValidationTestViewModel
    {
        public ValidationTestViewModel(string manager, List<ValidationTest> tests)
        {
            Manager = manager;
            Tests = tests;
        }

        public string Manager { get; set; }
        public List<ValidationTest> Tests { get; set; }
        public int OkCount => Tests.Count(t => t.Status == ValidationTest.ValidationStatus.OK);
        public int TotalCount => Tests.Count();
    }
}
