using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using static Model.Execution;
using static Model.Manager;

namespace DashboardBackend.Parsers
{
    public class LogMessageParser : IDataParser<LogMessage, Tuple<List<Manager>, List<Execution>>>
    {
        public LogMessageParser()
        {

        }

        // Log messages that indicate the end of an execution.
        public List<string> ExecutionFinishedMessages { get; } = new()
        {
            "Program closing due to the following error:",
            "Exiting from GuiManager...",
            "No managers left to start automatically for BATCH",
            "Deploy is finished!!",
        };

        public Tuple<List<Manager>, List<Execution>> Parse(List<LogMessage> data)
        {
            List<Manager> managers = new();
            List<Execution> executions = new();

            foreach (LogMessage message in data)
            {
                Execution execution = executions.FirstOrDefault(e => e?.Id == message.ExecutionId);
                if (execution is null)
                {
                    execution = new(message.ExecutionId, message.Date);
                    executions.Add(execution);
                }
                if (ExecutionFinishedMessages.Contains(message.Content))
                {
                    execution.Status = ExecutionStatus.Finished;
                }

                // Ensure that a manager is created with the specified context ID
                if (message.ContextId > 0)
                {
                    Manager manager = managers.Find(m => m?.ContextId == message.ContextId && m?.ExecutionId == message.ExecutionId);
                    if (manager is null)
                    {
                        manager = new() { ContextId = message.ContextId, ExecutionId = message.ExecutionId };
                        managers.Add(manager);
                    }

                    if (message.Content.StartsWith("Starting manager: ") && message.Content.Split(": ") is string[] args)
                    {
                        manager.Name = args[1].Split(',')[0];
                        manager.Status = ManagerStatus.Running;
                    }
                    else if (message.Content == "Manager execution done.")
                    {
                        manager.Status = ManagerStatus.Ok;
                    }

                    message.ManagerName = manager.Name ?? "Context not found";
                }
            }
            return new(managers, executions);
        }

        /// <summary>
        /// Searches the list of managers in the execution associated with the log message, and if it is found, its context ID is updated.
        /// If the manager does not already exist, it is created.
        /// </summary>
        /// <param name="message">The log message being parsed.</param>
        /// <param name="args">The contents of the log message split by ': '.</param>
        /// <param name="manager">If a new manager is created, this is the output.</param>
        /// <returns>True if a new manager was created, and false otherwise.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        private bool TryGetManager(Execution execution, LogMessage message, string[] args, out Manager manager)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("Expected two arguments ('Starting manager:' and a name), log message may have been incorrectly parsed", nameof(args));
            }
            bool hasCreatedNewManager = false;
            manager = null;
            string name = args[1].Split(',')[0];
            int contextId = message.ContextId;
            int executionId = message.ExecutionId;

            // If the manager already exists without a ContextID, it can be updated
            if (execution.Managers.FirstOrDefault(m => m.Name == name && m.ContextId == 0 && m.ExecutionId == 0) is Manager existingManager)
            {
                existingManager.ContextId = contextId;
                existingManager.ExecutionId = executionId;
            }
            // Otherwise create the manager
            else
            {
                manager = new Manager { Name = name, ContextId = contextId, ExecutionId = executionId };
                hasCreatedNewManager = true;
            }

            return hasCreatedNewManager;
        }

        #region Other details that may be worth extracting at a later time
        //// Indicates that a Manager's number of rows to process has been calculated during its post-run script. This may be useful for estimating runtimes.
        //else if (message.StartsWith("Total count:"))
        //{
        //    string value = Regex.Match(message, @"\d+\.?\d*").Value;
        //    int rowsToProcess = int.Parse(value, NumberStyles.AllowThousands);
        //    Console.WriteLine($"[INFO][ROWS:{rowsToProcess}]");
        //}
        //// Indicates that the message contains an SQL statement cost which may be relevant for calculating manager scores.
        //else if (Regex.IsMatch(message, @"^\[.+\] SQL statement has a cost of [\d+.]+, and a total of [\d+.]+ full table scans.$"))
        //{
        //    MatchCollection matches = Regex.Matches(message, @"(?<= )\d+\.?\d*");
        //    // Since the statement cost is printed in da-DK formatting, it must be parsed as such.
        //    float statementCost = float.Parse(matches[0].Value, new CultureInfo("da-DK"));
        //    int fullTableScans = int.Parse(matches[1].Value);
        //    Console.WriteLine($"[INFO][COST: {statementCost}][SCANS: {fullTableScans}]");
        //}
        #endregion
    }
}
