using DashboardBackend.Database;
using Model;
using System;
using System.Linq;

namespace DevConsoleApp
{
    class Program
    {
        public static string NowString { get => $"[{DateTime.Now.ToLongTimeString()}]"; }
        static void Main(string[] args)
        {
            IDatabaseHandler databaseHandler = new SqlDatabase();

            //**** CONV AND EXECUTION TESTING ****
            Conversion conv = new();
            conv.Executions = databaseHandler.GetExecutions();
            conv.ActiveExecution.Managers = databaseHandler.GetManagers();
            conv.ActiveExecution.Log.Messages = databaseHandler.GetLogMessages(conv.ActiveExecution.Id);
            conv.ActiveExecution.ValidationReport.ValidationTests = databaseHandler.GetAfstemninger();
            conv.HealthReport = databaseHandler.GetHealthReport();
            conv.HealthReport.Cpu.Readings = databaseHandler.GetCpuReadings();
            conv.HealthReport.Network.Readings = databaseHandler.GetNetworkReadings();
            conv.HealthReport.Ram.Readings = databaseHandler.GetRamReadings();

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
