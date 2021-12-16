using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Model;
using DashboardFrontend.ViewModels;

namespace DashboardFrontend.DetachedWindows
{
    public partial class ReconciliationReportDetached
    {
        public ReconciliationReportDetached(ReconciliationReportViewModel reconciliationReportViewModel)
        {
            InitializeComponent();
            ViewModel = reconciliationReportViewModel;
            DataContext = ViewModel;
        }
        
        public ReconciliationReportViewModel ViewModel { get; set; }

        /// <summary>
        /// Called once a TreeViewItem is expanded. Gets the item's ManagerObservable, and adds the manager name to a list of expanded TreeViewItems in the Reconciliation Report viewmodel.
        /// </summary>
        /// <remarks>This ensures that the items stay expanded when the data is updated/refreshed.</remarks>
        private void TreeViewReconciliations_Expanded(object sender, RoutedEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            if (tree.ItemContainerGenerator.ItemFromContainer(item) is ManagerObservable manager)
            {
                if (!ViewModel.ExpandedManagerNames.Contains(manager.Name))
                {
                    ViewModel.ExpandedManagerNames.Add(manager.Name);
                }
            }
        }

        /// <summary>
        /// Called once a TreeViewItem is collapsed. Gets the item's ManagerObservable, and removes the manager name to a list of expanded TreeViewItems in the Reconciliation Report viewmodel.
        /// </summary>
        private void TreeViewReconciliations_Collapsed(object sender, RoutedEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            item.IsSelected = false;
            if (tree.ItemContainerGenerator.ItemFromContainer(item) is ManagerObservable manager)
            {
                if (ViewModel.ExpandedManagerNames.Contains(manager.Name))
                {
                    ViewModel.ExpandedManagerNames.Remove(manager.Name);
                }
            }
        }

        /// <summary>
        /// Copies the source SQL query from the Reconciliation.
        /// </summary>
        private void CopySrcSql_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.DataContext is Reconciliation test)
            {
                Clipboard.SetText(test.SrcSql);
                TextBlockPopupSql.Content = "SQL source copied to clipboard";
                PopupCopySql.IsOpen = true;
            }
        }

        /// <summary>
        /// Copies the destination SQL query from the Reconciliation.
        /// </summary>
        private void CopyDestSql_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.DataContext is Reconciliation test)
            {
                Clipboard.SetText(test.DstSql);
                TextBlockPopupSql.Content = "SQL destination copied to clipboard";
                PopupCopySql.IsOpen = true;
            }
        }

        private void ButtonCopySql_MouseLeave(object sender, MouseEventArgs e)
        {
            PopupCopySql.IsOpen = false;
        }
    }
}


