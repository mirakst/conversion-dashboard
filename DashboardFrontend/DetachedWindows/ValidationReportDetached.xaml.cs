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

namespace DashboardFrontend.DetachedWindows
{

    public class ValidationEntry
    {
        public string ManagerName { get; set; } 
        public int ValidationsOK { get; set; }  
        public int ValidationsTotal { get; set; }
        public double ManagerScore { get; set; }  
    }
    public partial class ValidationReportDetached : Window
    {

        public ValidationReportDetached()
        {
            InitializeComponent();
            List<ValidationEntry> inputlist = new List<ValidationEntry>();

            inputlist.Add(new ValidationEntry() { ManagerName = "OneCleanDude", ValidationsOK = 2, ValidationsTotal = 4, ManagerScore = 250.75 });
            inputlist.Add(new ValidationEntry() { ManagerName = "FullSickoMode", ValidationsOK = 2, ValidationsTotal = 4, ManagerScore = 250.75 });
            inputlist.Add(new ValidationEntry() { ManagerName = "OneEvenMoreCleanDude", ValidationsOK = 2, ValidationsTotal = 4, ManagerScore = 250.75 });
            inputlist.Add(new ValidationEntry() { ManagerName = "HellSickoMode", ValidationsOK = 2, ValidationsTotal = 4, ManagerScore = 250.75 });

            foreach (ValidationEntry validationEntry in inputlist)
            {
                dataGridValidationReport.Items.Add(validationEntry);
            }

        }
    }
}


