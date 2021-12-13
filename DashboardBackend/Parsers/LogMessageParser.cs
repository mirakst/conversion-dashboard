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
                    if (message.Content.StartsWith("Starting manager: ") && message.Content.Split(": ") is string[] args)
                    {
                        managers.Add(new() 
                        { 
                            Name = args[1].Split(',')[0],
                            ContextId = message.ContextId,
                            Status = ManagerStatus.Running,
                            ExecutionId = message.ExecutionId,
                        });
                    }
                }
            }
            return new(managers, executions);
        }
    }
}
