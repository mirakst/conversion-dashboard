using System;
using System.Linq;
using DashboardBackend.Database;
using DashboardBackend.Database.Models;

namespace DevConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IDatabaseHandler databaseHandler = new SqlDatabase();

            var logs = databaseHandler.GetLogMessages();
            foreach (var item in logs)
            {
                Console.WriteLine($"[{item.Created}]: {item.ContextId}");
            }
        }
    }
}
