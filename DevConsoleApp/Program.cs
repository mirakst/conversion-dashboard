using DashboardBackend;
using DashboardBackend.Database;
using Model;

namespace DevConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DataUtilities.DatabaseHandler = new SqlDatabase();

            //**** CONV AND EXECUTION TESTING ****
            Conversion conv = new();
            conv.Executions = DataUtilities.GetExecutions();
            Console.WriteLine(DateTime.Now);
            DataUtilities.AddManagers(conv.Executions);
            Console.WriteLine(DateTime.Now);
            DataUtilities.AddManagerReadings(conv.ActiveExecution);
            conv.ActiveExecution.Log.Messages = DataUtilities.GetLogMessages(conv.ActiveExecution.Id); //Maybe automatically populate this? When executions are made, query messages that match Id and populate log.
            Console.WriteLine(DateTime.Now);
            conv.ActiveExecution.ValidationReport.ValidationTests = DataUtilities.GetAfstemninger();
            Console.WriteLine(DateTime.Now);
            conv.HealthReport = DataUtilities.BuildHealthReport();
            Console.WriteLine(DateTime.Now);
            DataUtilities.AddHealthReportReadings(conv.HealthReport);
            Console.WriteLine(DateTime.Now);



            foreach (var manager in conv.ActiveExecution.Managers)
            {
                Console.WriteLine(manager.ToString());
            }

            Console.ReadKey();
        }
    }
}
