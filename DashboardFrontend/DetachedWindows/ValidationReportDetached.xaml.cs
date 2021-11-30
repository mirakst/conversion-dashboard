using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Model;
using DashboardBackend;
using DashboardFrontend.ViewModels;
using System.Diagnostics;

namespace DashboardFrontend.DetachedWindows
{
    public partial class ValidationReportDetached
    {
        public ValidationReportDetached(ValidationReportViewModel validationReportViewModel)
        {
            InitializeComponent();
            ViewModel = validationReportViewModel;
            DataContext = ViewModel;
        }
        
        public ValidationReportViewModel ViewModel { get; set; }

        /// <summary>
        /// Called once a TreeViewItem is expanded. Gets the item's ManagerValidationsWrapper, and adds the manager name to a list of expanded TreeViewItems in the Validation Report viewmodel.
        /// </summary>
        /// <remarks>This ensures that the items stay expanded when the data is updated/refreshed.</remarks>
        private void TreeViewValidations_Expanded(object sender, RoutedEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            var wrapper = (ManagerValidationsWrapper)tree.ItemContainerGenerator.ItemFromContainer(item);
            if (!ViewModel.ExpandedManagerNames.Contains(wrapper.ManagerName))
            {
                ViewModel.ExpandedManagerNames.Add(wrapper.ManagerName);
            }
        }

        /// <summary>
        /// Called once a TreeViewItem is collapsed. Gets the item's ManagerValidationsWrapper, and removes the manager name to a list of expanded TreeViewItems in the Validation Report viewmodel.
        /// </summary>
        private void TreeViewValidations_Collapsed(object sender, RoutedEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            var wrapper = (ManagerValidationsWrapper)tree.ItemContainerGenerator.ItemFromContainer(item);
            ViewModel.ExpandedManagerNames.Remove(wrapper.ManagerName);
        }

        private void CopySrcSql_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.DataContext is ValidationTest test)
            {
                Clipboard.SetText(test.SrcSql);
            }
        }

        private void CopyDestSql_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.DataContext is ValidationTest test)
            {
                Clipboard.SetText(test.DstSql);
            }
        }
    }
}


