using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ConversionEngineSimulator
{
    static class DBUtilities
    {
        /// <summary>
        /// Generates a raw SQL query for the specified table and with the specified conditions.
        /// </summary>
        /// <param name="tbl">The table to query.</param>
        /// <param name="condition">Any conditions to the query, formatted in SQL.</param>
        /// <returns>The resulting query string.</returns>
        public static string GenerateQueryString(IDatabaseTable tbl, string condition = "") //Generates a select query string with option for conditions
        {
            return $"Select * From { tbl.TableName } { condition }";
        }

        /// <summary>
        /// Generates a raw SQL statement that inserts the values of the specified table into their respective columns. 
        /// </summary>
        /// <param name="tbl">The table to perform the insertion on.</param>
        /// <returns>The resulting statement as a string.</returns>
        public static string GenerateExecutionString(IDatabaseTable tbl) //Generates an execution string for data insertion in table
        {
            return $"Insert Into { tbl.TableName } ({ tbl.ColumnNames }) Values ({ tbl.OutputColumnNames })";
        }

        /// <summary>
        /// Clears all tables in the destination database.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        public static int ClearDestTables() //Clears all tables in destination database
        {
            var conn = DBInfo.ConnectToDestDB();
            int rowsAffected = 0;
            try
            {
                rowsAffected = conn.Execute("Delete from MANAGERS");
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            rowsAffected += conn.Execute($"Delete From AFSTEMNING;\nDelete From ENGINE_PROPERTIES;\nDelete From EXECUTIONS;\n" +
                                         $"Delete From HEALTH_REPORT;\nDelete From LOGGING;\nDelete From LOGGING_CONTEXT;\n" +
                                         $"Delete From MANAGER_TRACKING;\n");
            return rowsAffected;
        }

        /// <summary>
        /// Queries the specified table by generating a query string.
        /// </summary>
        /// <typeparam name="T">The class (/model) representing the specified table's contents.</typeparam>
        /// <param name="tbl">The table to query.</param>
        /// <returns>The result of the query.</returns>
        public static List<T> QueryTable<T>(IDatabaseTable tbl)
        {
            var conn = DBInfo.ConnectToSrcDB();
            var queryString = GenerateQueryString(tbl);
            System.Console.WriteLine("Attempting query on table: "+tbl.TableName);
            List<T> outputList = conn.Query<T>(queryString).ToList();
            System.Console.WriteLine("Query complete. Entries: " + outputList.Count);
            return outputList;
        }

        /// <summary>
        /// Inserts the specified entries into the specified table in the destination database.
        /// </summary>
        /// <typeparam name="T">The class (/model) representing the specified table's contents.</typeparam>
        /// <param name="entryList">The entries to insert.</param>
        /// <param name="tbl">The table to insert into.</param>
        public static void InsertTable<T>(List<T> entryList, IDatabaseTable tbl) //Populates a table in the destination database
        {
            SqlConnection conn = DBInfo.ConnectToDestDB();
            string sqlExecution = GenerateExecutionString(tbl);
            Console.WriteLine($"Inserted table {tbl.TableName} with {conn.Execute(sqlExecution, entryList)} entries.");
        }

        /// <summary>
        /// Helper method to perform a task after a specified delay.
        /// </summary>
        /// <param name="ts">The timespan of the delay.</param>
        public async static Task SetTimeout(TimeSpan ts) //Calls itself after a timeout
        {
            await Task.Delay(ts).ConfigureAwait(false);
        }

        /// <summary>
        /// Inserts the specified entries into the specified table in the destination database.
        /// </summary>
        /// <remarks>To simulate an actual running Conversion, each entry is offset by the time of its occurrance respective to the start of the first execution.</remarks>
        /// <typeparam name="T">A model for a timestamped database entry.</typeparam>
        /// <param name="entryList">The list of entries to insert.</param>
        /// <param name="tbl">The table to insert into.</param>
        public static async Task AsyncExecution<T>(List<T> entryList, IDatabaseTable tbl) 
            where T : ITimestampedDatabaseEntry 
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
                        if (entry is EngineProperty e && (e.KEY == "START_TIME" || e.KEY == "END_TIME"))
                        {
                            e.VALUE = entry.CREATED.ToString();
                        }
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