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
            DbInfo.Initialize(server, srcDb, dstDb);

            //CLEAR DESTINATION DB
            Console.WriteLine("Clearing destination DB tables...");
            Console.WriteLine($"Rows affected: " + DbUtilities.ClearDestTables());

            //INSERTING STATIC TABLES
            Console.WriteLine("\n---------- Inserting static tables ----------");
            try
            {
                ManagerTable managerInfo = new();
                DbUtilities.InsertTable(managerInfo.Entries, managerInfo);
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            ManagerTrackingTable managerTracking = new();
            LoggingContextTable loggingContext = new();

            DbUtilities.InsertTable(managerTracking.Entries, managerTracking);
            DbUtilities.InsertTable(loggingContext.Entries, loggingContext);

            //QUERY TABLES
            Console.WriteLine("\n---------- Querying tables ----------");
            LoggingTable logs = new();
            ReconciliationTable reconciliations = new();
            ExecutionTable executions = new();
            EnginePropertyTable engineProperties = new();
            HealthReportTable healthReports = new();

            //*** STREAMING ***
            Console.WriteLine("\n---------- Stream starting ----------");

            //STREAM FILES FROM THE STORED TABLES
            _ = DbUtilities.AsyncExecution(logs.Entries, logs);
            _ = DbUtilities.AsyncExecution(reconciliations.Entries, reconciliations);
            _ = DbUtilities.AsyncExecution(executions.Entries, executions);
            _ = DbUtilities.AsyncExecution(engineProperties.Entries, engineProperties);
            _ = DbUtilities.AsyncExecution(healthReports.Entries, healthReports);

            Console.ReadKey(true);
            return 0;
        }
    }
}