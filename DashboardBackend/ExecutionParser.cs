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
        public IList<Execution> Parse(IList<Execution> data)
        {
            return data;
        }
    }
}
