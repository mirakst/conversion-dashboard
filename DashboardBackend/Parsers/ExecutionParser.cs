using DashboardBackend.Database.Models;
using Model;

namespace DashboardBackend.Parsers
{
    public class ExecutionParser : IDataParser<ExecutionEntry, List<Execution>>
    {
        public List<Execution> Parse(List<ExecutionEntry> data)
        {
            return (from item in data
                    let executionId = (int)item.ExecutionId.Value
                    let created = item.Created.Value
                    select new Execution(executionId, created))
                    .ToList();
        }
    }
}
