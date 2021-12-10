using Model;

namespace DashboardBackend
{
    public class ExecutionParser : IDataParser<Execution, IList<Execution>>
    {
        public ExecutionParser()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<IList<Execution>> Parse(IList<Execution> data)
        {
            List<Execution> result = new();
            await Task.Run(() =>
            {
                foreach (Execution execution in data)
                {
                    //if (!conversion.Executions.Any(e => e.Id == execution.Id))
                    //{
                    //    result.Add(execution);
                    //}
                }
            });
            return result;
        }
    }
}
