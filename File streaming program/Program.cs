using System;
using System.Threading.Tasks;

namespace Filestreaming_Program
{
    internal class Program
    {
        static void Main()
        {
            //*** INIT ***
            Console.WriteLine("--------- Initializing streaming ----------");

            //CLEAR DESTINATION DB
            Console.WriteLine("Clearing destination DB tables...");
            Console.WriteLine($"Rows affected: " + DBUtilities.ClearDestTables());

            //INSERTING STATIC TABLES
            Console.WriteLine("\n---------- Inserting static tables ----------");
            ManagerTable managerInfo = new();
            ManagerTrackingTable managerTracking = new();
            LoggingContextTable loggingContext = new();

            DBUtilities.InsertTable<Manager>(managerInfo.Entries, managerInfo);
            DBUtilities.InsertTable<ManagerTracking>(managerTracking.Entries, managerTracking);
            DBUtilities.InsertTable<LoggingContext>(loggingContext.Entries, loggingContext);

            //QUERY TABLES
            Console.WriteLine("\n---------- Querying tables ----------");
            LoggingTable logs = new();
            VoteTable votes = new();
            ExecutionTable executions = new();
            EnginePropertyTable engineProperties = new();
            HealthReportTable healthReports = new();

            //*** STREAMING ***
            Console.WriteLine("\n---------- Stream starting ----------");

            //STREAM FILES FROM THE STORED TABLES
            Task task1 = DBUtilities.AsyncExecution<LogMsg>(logs.Entries, logs);
            Task task2 = DBUtilities.AsyncExecution<Vote>(votes.Entries, votes);
            Task task3 = DBUtilities.AsyncExecution<Execution>(executions.Entries, executions);
            Task task4 = DBUtilities.AsyncExecution<EngineProperty>(engineProperties.Entries, engineProperties);
            Task task5 = DBUtilities.AsyncExecution<HealthReport>(healthReports.Entries, healthReports);

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
    }
}