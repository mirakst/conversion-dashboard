using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DashboardBackend.Database.Models;
using Model;
using static Model.Execution;
using static Model.LogMessage;
using static Model.Manager;
using static Model.ValidationTest;

namespace DashboardBackend.Parsers
{
    public class LogMessageParser : IDataParser<LoggingEntry, Tuple<List<LogMessage>, List<Manager>, List<Execution>>>
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

        public Tuple<List<LogMessage>, List<Manager>, List<Execution>> Parse(List<LoggingEntry> data)
        {
            List<LogMessage> messages = new();
            List<Manager> managers = new();
            List<Execution> executions = new();

            foreach (LoggingEntry entry in data)
            {
                LogMessage message = GetParsedLogMessage(entry);
                messages.Add(message);
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
            return new(messages, managers, executions);
        }

        private LogMessage GetParsedLogMessage(LoggingEntry entry)
        {
            string content = Regex.Replace(entry.LogMessage, @"\u001b\[\d*;?\d+m", ""); // Removes color coding which makes it harder to parse the text later
            LogMessageType type = GetLogMessageType(entry, content);
            int contextId = (int)entry.ContextId.Value;
            int executionId = (int)entry.ExecutionId.Value;
            DateTime date = entry.Created.Value;
            return new LogMessage(content, type, contextId, executionId, date);
        }

        /// <summary>
        /// Returns the type of the log message parameter 'entry'.
        /// </summary>
        /// <param name="entry">A single entry from the [LOGGING] table in the state database.</param>
        /// <returns>A log message type besed on the enum in the log message class.</returns>
        /// <exception cref="ArgumentException">Thrown if the parameter passed is not a legal log message type.</exception>
        private LogMessageType GetLogMessageType(LoggingEntry entry, string content)
        {
            var type = entry.LogLevel switch
            {
                "INFO" => LogMessageType.Info,
                "WARN" => LogMessageType.Warning,
                "ERROR" => LogMessageType.Error,
                "FATAL" => LogMessageType.Fatal,
                _ => LogMessageType.None,
            };
            if (content.StartsWith("Afstemning") || content.StartsWith("Check -"))
            {
                if (type.HasFlag(LogMessageType.Error))
                {
                    type |= LogMessageType.Validation;
                }
                else
                {
                    type = LogMessageType.Validation;
                }
            }
            return type;
        }
    }
}
