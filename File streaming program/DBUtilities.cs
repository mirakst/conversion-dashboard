using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Filestreaming_Program
{
    static class DBUtilities
    {
        public static string GenerateQueryString(IDatabaseTable tbl, string condition = "") //Generates a select query string with option for conditions
        {
            return "Select * From " + tbl.TableName + " " + condition;
        }

        public static string GenerateExecutionString(IDatabaseTable tbl) //Generates an execution string for data insertion in table
        {
            return "Insert Into " + tbl.TableName + " (" + tbl.ColumnNames + ") Values (" + tbl.OutputColumnNames + ")";
        }

        public static int ClearDestTables() //Clears all tables in destination database
        {
            Console.WriteLine();
            var conn = DBInfo.ConnectToDestDB();
            return conn.Execute($"use [{DBInfo.DestDatabase}];\nDelete From AFSTEMNING;\nDelete From ENGINE_PROPERTIES;\nDelete From EXECUTIONS;\n" +
                                $"Delete From HEALTH_REPORT;\nDelete From LOGGING;\nDelete From LOGGING_CONTEXT;\n" +
                                $"Delete From MANAGER_TRACKING;\nDelete from MANAGERS;");
        }

        public static List<T> QueryTable<T>(IDatabaseTable tbl) //Queries a table based on sqlQuery string
        {
            var conn = DBInfo.ConnectToSrcDB();
            var queryString = GenerateQueryString(tbl);
            System.Console.WriteLine("Attempting query on table: "+tbl.TableName);
            List<T> outputList = conn.Query<T>(queryString).ToList();
            System.Console.WriteLine("Query complete. Entries: " + outputList.Count);
            return outputList;
        }
        
        public static void InsertTable<T>(List<T> entryList, IDatabaseTable tbl) //Populates a table in the destination database
        {
            SqlConnection conn = DBInfo.ConnectToDestDB();
            string sqlExecution = GenerateExecutionString(tbl);
            Console.WriteLine($"Inserted table {tbl.TableName} with {conn.Execute(sqlExecution, entryList)} entries.");
        }

        public async static Task SetTimeout(TimeSpan ts) //Calls itself after a timeout
        {
            await Task.Delay(ts).ConfigureAwait(false);
        }

        public static async Task AsyncExecution<T>(List<T> entryList, IDatabaseTable tbl) where T : ITimestampedDatabaseEntry //Inserts data in destination database based on timestamp in source database
        {
            string executionString = GenerateExecutionString(tbl);
            await Task.Run(() =>
            {
                var startTime = DBInfo.convStartTime;

                foreach (T entry in entryList)
                {
                    TimeSpan offset = entry.CREATED.Subtract(startTime);
                    SetTimeout(offset).ContinueWith(t =>
                    {
                        entry.CREATED = DateTime.Now;
                        using (SqlConnection conn = DBInfo.ConnectToDestDB())
                        {
                            conn.Execute(executionString, entry);
                        }
                        Console.WriteLine(entry.ToString());
                    });
                }
            });
        }
    }
}