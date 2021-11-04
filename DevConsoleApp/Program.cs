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
            Console.WriteLine(NowString+" -- Started! --");
            /*conv.Executions = databaseHandler.GetExecutions();
            Console.WriteLine(NowString+" -- Executions fetched --");
            conv.ActiveExecution.Managers = databaseHandler.GetManagers();
            Console.WriteLine(NowString+" -- Managers fetched --");
            conv.ActiveExecution.Log.Messages = databaseHandler.GetLogMessages();
            Console.WriteLine(NowString+" -- Log messages fetched --");
            conv.ActiveExecution.ValidationReport.ValidationTests = databaseHandler.GetAfstemninger();
            Console.WriteLine(NowString+" -- Validations fetched --");
            conv.HealthReport = databaseHandler.GetHealthReport();
            Console.WriteLine(NowString+" -- Health report fetched --");
            conv.HealthReport.Cpu.Readings = databaseHandler.GetCpuReadings();
            Console.WriteLine(NowString+" -- Cpu readings fetched --");
            conv.HealthReport.Network.Readings = databaseHandler.GetNetworkReadings();*/
            List<NetworkUsage> networkUsage = databaseHandler.GetNetworkReadings();
            Console.WriteLine(NowString+" -- Network readings fetched --");
            /*foreach (var item in conv.Executions)
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
            }*/

/*            Console.WriteLine(conv.HealthReport.ToString());
*/
            /*foreach (var item in conv.HealthReport.Cpu.Readings)
            {
                Console.WriteLine(item.ToString());
            }*/
            /*foreach (var item in conv.HealthReport.Network.Readings)
            {
                Console.WriteLine(item.ToString());
            }*/
            foreach (var item in networkUsage)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
