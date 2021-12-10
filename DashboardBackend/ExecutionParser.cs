using Model;

namespace DashboardBackend
{
    public class ExecutionParser : IDataParser<Execution, Execution>
    {
        private readonly Conversion _conversion;

        public ExecutionParser(Conversion conversion)
        {
            _conversion = conversion;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IList<Execution> Parse(IList<Execution> data)
        {
            List<Execution> result = new();
            foreach (Execution execution in data)
            {
                if (!_conversion.Executions.Any(e => e.Id == execution.Id))
                {
                    result.Add(execution);
                }
            }
            return result;
        }
    }
}
