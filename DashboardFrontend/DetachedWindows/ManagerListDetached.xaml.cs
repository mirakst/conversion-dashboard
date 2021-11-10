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
    public class item
    {
        public string name { get; set; }
        public int score {  get; set; }
        public string status { get; set; }
        public int id {  get; set; }
        public int execId {  get; set; }
        public int run {  get; set; }
        public int startTime {  get; set; }
        public int endTime {  get; set; }
        public int Runtime {  get; set; }
        public int rowsRead {  get; set; }
        public int rowsWritten {  get; set; }
    }
    /// <summary>
    /// Interaction logic for ManagerListDetached.xaml
    /// </summary>
    public partial class ManagerListDetached : Window
    {
        public ManagerListDetached()
        {
            InitializeComponent();
            datagridManagers.Items.Add(new item() {name = "Assashdoaishdoahsdaasdaadssadasdasd", score = 3, status = "fail", id = 1023});
            datagridManagers.Items.Add(new item() {name = "usdagsgssdfsf", score = 412, status = "OK", id = 1231 });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            item? obj = ((FrameworkElement)sender).DataContext as item;
            datagridManagers.Items.Add(obj);
            datagridDetailedManagers.Items.Add(obj);
            datagridChartedManagers.Items.Add(obj); 
        }
    }
}
