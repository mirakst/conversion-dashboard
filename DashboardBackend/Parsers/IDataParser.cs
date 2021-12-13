namespace DashboardBackend.Parsers
{
    public interface IDataParser<TInput, TOutput>
        where TInput : class
        where TOutput : class
    {
        TOutput Parse(List<TInput> data);
    }
}
