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
            conv.ActiveExecution.Managers = DataUtilities.GetManagers();
            conv.ActiveExecution.Log.Messages = DataUtilities.GetLogMessages(conv.ActiveExecution.Id); //Maybe automatically populate this? When executions are made, query messages that match Id and populate log.
            conv.ActiveExecution.ValidationReport.ValidationTests = DataUtilities.GetAfstemninger();
            conv.HealthReport = DataUtilities.BuildHealthReport();
            DataUtilities.AddHealthReportReadings(conv.HealthReport);

            foreach (var item in conv.Executions)
            {
                Console.WriteLine(item);
            }
            foreach (var item in conv.ActiveExecution.Managers)
            {
                Console.WriteLine(item);
            }
            foreach (var item in conv.ActiveExecution.Log.Messages)
            {
                Console.WriteLine(item);
            }
            foreach (var item in conv.ActiveExecution.ValidationReport.ValidationTests)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine(conv.HealthReport.ToString());

            foreach (var item in conv.HealthReport.Cpu.Readings)
            {
                Console.WriteLine(item.ToString());
            }
            foreach (var item in conv.HealthReport.Network.Readings)
            {
                Console.WriteLine(item.ToString());
            }
            foreach (var item in conv.HealthReport.Ram.Readings)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
