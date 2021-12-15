using System;
using System.Data.SqlClient;

namespace ConversionEngineSimulator
{
    internal class Program
    {
        static int Main(string[] args)
        {
            //*** INIT ***
            if (args.Length != 3)
            {
                Console.Error.WriteLine("Please specify a [server], [source database], and [destination database]");
                Console.ReadKey(true);
                return -1;
            }
            string server = args[0];
            string srcDb = args[1];
            string dstDb = args[2];
            Console.WriteLine("--------- Initializing streaming ----------");
            DBInfo.Initialize(server, srcDb, dstDb);

            //CLEAR DESTINATION DB
            Console.WriteLine("Clearing destination DB tables...");
            Console.WriteLine($"Rows affected: " + DBUtilities.ClearDestTables());

            //INSERTING STATIC TABLES
            Console.WriteLine("\n---------- Inserting static tables ----------");
            try
            {
                ManagerTable managerInfo = new();
                DBUtilities.InsertTable(managerInfo.Entries, managerInfo);
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            ManagerTrackingTable managerTracking = new();
            LoggingContextTable loggingContext = new();

            DBUtilities.InsertTable(managerTracking.Entries, managerTracking);
            DBUtilities.InsertTable(loggingContext.Entries, loggingContext);

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
            _ = DBUtilities.AsyncExecution(logs.Entries, logs);
            _ = DBUtilities.AsyncExecution(votes.Entries, votes);
            _ = DBUtilities.AsyncExecution(executions.Entries, executions);
            _ = DBUtilities.AsyncExecution(engineProperties.Entries, engineProperties);
            _ = DBUtilities.AsyncExecution(healthReports.Entries, healthReports);

            Console.ReadKey(true);
            return 0;
        }
    }
}