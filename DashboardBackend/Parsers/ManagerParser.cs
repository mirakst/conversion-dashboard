using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DashboardBackend.Database.Models;
using Model;
using static Model.Manager;

namespace DashboardBackend.Parsers
{
    public class ManagerParser : IDataParser<EnginePropertyEntry, List<Manager>>
    {
        public List<Manager> Parse(List<EnginePropertyEntry> data)
        {
            List<Manager> result = new();
            // We will only parse the essential data:
            var filteredData = data.Where(x => x.Key == "START_TIME"
                                            || x.Key == "END_TIME"
                                            || x.Key == "Læste rækker"
                                            || x.Key == "Skrevne rækker");
            foreach (EnginePropertyEntry entry in filteredData)
            {
                string name = entry.Manager.Split(',')[0];
                Manager manager = result.FirstOrDefault(m => m.Name == name && m.ContextId == 0 && m.IsMissingValues);
                if (manager is null)
                {
                    manager = new()
                    {
                        Name = name
                    };
                    result.Add(manager);
                }
                AddEnginePropertiesToManager(manager, entry);
            }
            return result;
        }


        /// <summary>
        /// Parses the value of the specified EnginePropertyEntry and adds it to the given manager.
        /// </summary>
        /// <param name="manager">The manager object associated with the entry.</param>
        /// <param name="entry">The EnginePropertyEntry to get data from.</param>
        private void AddEnginePropertiesToManager(Manager manager, EnginePropertyEntry entry)
        {
            switch (entry.Key)
            {
                case "START_TIME":
                    if (DateTime.TryParse(entry.Value, out DateTime startTime))
                    {
                        manager.StartTime = startTime;
                        manager.Status = ManagerStatus.Running;
                        if (manager.EndTime.HasValue)
                        {
                            manager.Runtime = manager.EndTime.Value.Subtract(startTime);
                            manager.Status = ManagerStatus.Ok;
                        }
                    }
                    break;
                case "END_TIME":
                    if (DateTime.TryParse(entry.Value, out DateTime endTime))
                    {
                        manager.EndTime = endTime;
                        if (manager.StartTime.HasValue)
                        {
                            manager.Runtime = endTime.Subtract(manager.StartTime.Value);
                            manager.Status = ManagerStatus.Ok;
                        }
                    }
                    break;
                case "Læste rækker":
                    if (int.TryParse(entry.Value, out int rowsRead))
                    {
                        manager.RowsRead = rowsRead;
                    }
                    break;
                case "Skrevne rækker":
                    if (int.TryParse(entry.Value, out int rowsWritten))
                    {
                        manager.RowsWritten = rowsWritten;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
