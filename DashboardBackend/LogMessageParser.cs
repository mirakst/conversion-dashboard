using System.Text.RegularExpressions;

using Model;

namespace DashboardBackend
{
    public class LogMessageParser
    {
        private readonly Conversion _conversion;

        public LogMessageParser(Conversion conversion)
        {
            _conversion = conversion;
        }

        // Log messages that indicate the end of an execution.
        public List<string> ExecutionFinishedMessages { get; } = new()
        {
            "Program closing due to the following error:",
            "Exiting from GuiManager...",
            "No managers left to start automatically for BATCH",
            "Deploy is finished!!",
        };

        /// <summary>
        /// Iteratively parses the specified list of log messages and performs the relevant actions based on their contents.
        /// </summary>
        /// <remarks>The main output is a list of newly created managers, but the method updates existing managers and executions as a side-effect.</remarks>
        /// <param name="messages">The list of messages to parse.</param>
        /// <returns>A list of any managers that were created during parsing.</returns>
        public List<Manager> Parse(IList<LogMessage> messages)
        {
            List<Manager> managers = new();

            foreach (LogMessage message in messages)
            {
                string associatedManager = null;

                #region Check for any execution updates
                Execution execution = _conversion.Executions.FirstOrDefault(e => e?.Id == message.ExecutionId);
                if (execution is null)
                {
                    execution = new(message.ExecutionId, message.Date);
                    _conversion.AddExecution(execution);
                }
                if (ExecutionFinishedMessages.Contains(message.Content))
                {
                    execution.Status = ExecutionStatus.Finished;
                }
                #endregion

                #region Check for any manager started/finished updates
                if (message.Content.StartsWith("Starting manager: ") && message.Content.Split(": ") is string[] args)
                {
                    if (TryGetManager(execution, message, args, out Manager manager))
                    {
                        managers.Add(manager);
                        associatedManager = manager.Name;
                    }
                }
                else if (message.Content.StartsWith("Manager execution done."))
                {
                    if (execution.Managers.FirstOrDefault(m => m.ContextId == message.ContextId) is Manager manager)
                    {
                        manager.Status = ManagerStatus.Ok;
                    }
                }
                #endregion

                #region Set context tooltip
                // execution.Managers.FirstOrDefault(m => m.ContextId == message.ContextId) is Manager m 
                if (associatedManager is null && managers.FirstOrDefault(m => m.ContextId == message.ContextId) is Manager m) // FIX DET!
                {
                    associatedManager = m.Name;
                }
                message.ManagerName = associatedManager ?? "Context not found";
                #endregion
            }

            return managers;
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
