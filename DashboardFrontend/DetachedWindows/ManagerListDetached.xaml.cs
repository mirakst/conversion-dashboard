using System;
using System.Collections.Generic;
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

namespace DashboardFrontend.DetachedWindows
{
    public class Item
    {
        public string? Name { get; set; }
        public int Score {  get; set; }
        public string? Status { get; set; }
        public int ID {  get; set; }
        public int ExecId {  get; set; }
        public int Run {  get; set; }
        public int StartTime {  get; set; }
        public int EndTime {  get; set; }
        public int Runtime {  get; set; }
        public int RowsRead {  get; set; }
        public int RowsWritten {  get; set; }
    }
    /// <summary>
    /// Interaction logic for ManagerListDetached.xaml
    /// </summary>
    public partial class ManagerListDetached : Window
    {
        public ManagerListDetached()
        {
            InitializeComponent();

            Random random= new Random(2);

            for (int i = 0; i < 50;  i++)
            {
                datagridManagers.Items.Add(new Item() { ID = random.Next() % 100, Score = random.Next() % 100 });
            }
        }

        private void AddManager_Click(object sender, RoutedEventArgs e)
        {
            Item? selectedManager = datagridManagers.SelectedItem as Item;

            if (datagridManagers.SelectedItems.Count > 1)
            {
                foreach (Item manager in datagridManagers.SelectedItems)
                {
                    if (!datagridManagerDetails.Items.Contains(manager))
                    {
                        datagridManagerDetails.Items.Add(manager);
                        datagridManagerCharts.Items.Add(manager);
                    }
                }
            }
            else
            {
                _ = datagridManagerDetails.Items.Add(selectedManager);
                _ = datagridManagerCharts.Items.Add(selectedManager);
            }
        }

        private void RemoveManager_Click(object sender, RoutedEventArgs e)
        {
            Item? selectedManager = ((Button)sender).Tag as Item;
            List<Item> managers = new() { };
            
            if (datagridManagerDetails.SelectedItems.Count > 1 || datagridManagerCharts.SelectedItems.Count > 1)
            {
                foreach (Item manager in TabInfo.IsSelected == true ? datagridManagerDetails.SelectedItems : datagridManagerCharts.SelectedItems)
                {
                    managers.Add(manager);
                }
                foreach (Item manager in managers)
                {
                    datagridManagerDetails.Items.Remove(manager);
                    datagridManagerCharts.Items.Remove(manager);
                }
            }
            else
            {
                datagridManagerDetails.Items.Remove(selectedManager);
                datagridManagerCharts.Items.Remove(selectedManager);
            }
        }

        private void SearchManagers_Click(object sender, RoutedEventArgs e) 
        {
            SearchManagerDatagrid();
        }

        private void ResetManagers_Click(object sender, RoutedEventArgs e)
        {
            datagridManagerDetails.Items.Clear();
            datagridManagerCharts.Items.Clear();
        }

        private void textboxSearchbar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && textboxSearchbar != null)
            {
                SearchManagerDatagrid();
            }
        }

        private void SearchManagerDatagrid() //Selection color should probably be changed so it is easier to find.
        {
            for (int i = 0; i < datagridManagers.Items.Count; i++)
            {
                Item manager = (Item)datagridManagers.Items[i];
                if (manager.Name == textboxSearchbar.Text || manager.ID.ToString() == textboxSearchbar.Text)
                {
                    datagridManagers.SelectedItem = manager;
                    datagridManagers.ScrollIntoView(manager);
                    return;
                }
            }
            // Add tooltip for when item is not found
        }
    }
}
