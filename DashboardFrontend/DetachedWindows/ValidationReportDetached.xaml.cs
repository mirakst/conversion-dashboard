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
    public partial class ValidationReportDetached : Window
    {
        public ValidationReportDetached()
        {
            InitializeComponent();
            List<string> validationReportsOne = new();
            validationReportsOne.Add("abc");
            validationReportsOne.Add("def");
            validationReportsOne.Add("hij");
            validationReportsOne.Add("klm");
            validationReportsOne.Add("nop");
            validationReportsOne.Add("qrs");

            List<string> validationReportStatusOne = new();
            validationReportStatusOne.Add("OK");
            validationReportStatusOne.Add("FAILED");
            validationReportStatusOne.Add("FAILED_MISMATCH");
            validationReportStatusOne.Add("OK");
            validationReportStatusOne.Add("OK");
            validationReportStatusOne.Add("OK");

            List<string> validationReportsTwo = new();
            validationReportsTwo.Add("123ksdhjfbSDFÆLKJ WE");
            validationReportsTwo.Add("456S LEJFNoipdæ fnjoæiO ");
            validationReportsTwo.Add("789sad foUIHOPDHSA POIHipqhgdeipgasjkd bjasbkdcn");
            validationReportsTwo.Add("asdASLODLFJNIOBNIDFN");
            validationReportsTwo.Add("fghjSD FOUHi fhiowde GFIU gbi");
            validationReportsTwo.Add("eortiOuihasfD OHSDpiohfipbeWF Bd");

            List<string> validationReportStatusTwo = new();
            validationReportStatusTwo.Add("OK");
            validationReportStatusTwo.Add("OK");
            validationReportStatusTwo.Add("OK");
            validationReportStatusTwo.Add("OK");
            validationReportStatusTwo.Add("FAILED_MISMATCH");
            validationReportStatusTwo.Add("DISABLED");

            List<ValidationReport> validationReportsList = new();
            validationReportsList.Add(new ValidationReport() { ManagerName = "Manager 1", ValidationReportList = validationReportsOne, ValidationReportStatus = validationReportStatusOne });
            validationReportsList.Add(new ValidationReport() { ManagerName = "Manager 2", ValidationReportList = validationReportsTwo, ValidationReportStatus = validationReportStatusTwo });

            dataGridValidationReport.ItemsSource = validationReportsList;
        }

        private void OnExpandChange(object sender, RoutedEventArgs e)
        {
            if (sender is Expander expander)
            {
                var row = DataGridRow.GetRowContainingElement(expander);

                row.DetailsVisibility = expander.IsExpanded ? Visibility.Visible
                                                            : Visibility.Collapsed;
            }
        }
    }

    public class ValidationReport
    {
        public string ManagerName { get; set; }
        public List<string> ValidationReportList { get; set;  }
        
        public List<string> ValidationReportStatus { get; set; }
        public int ValidationsOK { get => ValidationReportStatus.Where(e => e == "OK").Count(); }
        public int ValidationsTotal { get => ValidationReportStatus.Count; }
        public double ManagerScore { get => (ValidationsOK / ValidationsTotal) * 100; }
    }
}