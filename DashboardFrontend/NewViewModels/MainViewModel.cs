using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardFrontend.NewViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel(IDashboardController controller)
        {
            LogViewModel = new(controller);
            Controller = controller;
        }

        public NewLogViewModel LogViewModel { get; }
        public IDashboardController Controller { get; }
    }
}
