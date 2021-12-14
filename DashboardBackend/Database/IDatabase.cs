using DashboardBackend.Database.Models;

namespace DashboardBackend.Database
{
    /// <summary>
    /// Contains utilities to handle all interaction with the state database.
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Retrieves all entries in the AFSTEMNING table of the state database added after the specified DateTime.
        /// </summary>
        /// <param name="minDate">A date constraint for the returned objects</param>
        /// <returns>A list of all Validations no older than the specified DateTime</returns>
        List<AfstemningEntry> QueryAfstemninger(DateTime minDate);

        /// <summary>
        /// Retrieves all entries in the EXECUTIONS table of the state database added after the specified DateTime.
        /// </summary>
        /// <param name="minDate">A date constraint for the returned objects</param>
        /// <returns>A list of Executions no older than the specified DateTime</returns>
        List<ExecutionEntry> QueryExecutions(DateTime minDate);

        /// <summary>
        /// Retrieves all entries in the LOGGING table of the state database added after the specified DateTime, 
        /// matching the supplied executionId.
        /// </summary>
        /// <param name="executionId">An Execution ID constraint for the returned objects.</param>
        /// <param name="minDate">A date constraint for the returned objects.</param>
        /// <returns>A list of log messages no older than the specified DateTime, from the specific execution.</returns>
        List<LoggingEntry> QueryLogMessages(int executionId, DateTime minDate);

        /// <summary>
        /// Retrieves all entries in the LOGGING table of the state database added after the specified DateTime.
        /// </summary>
        /// <param name="minDate">A date constraint for the returned objects.</param>
        /// <returns>A list of log messages no older than the specified DateTime.</returns>
        List<LoggingEntry> QueryLogMessages(DateTime minDate);

        /// <summary>
        /// Retrieves all entries from the LOGGING_CONTEXT table of the state database .
        /// </summary>
        /// <returns>A list of Managers.</returns>
        List<LoggingContextEntry> QueryLoggingContext(int executionId);

        /// <summary>
        /// Retrieves all entries in the HEALTH_REPORT table of the state database where REPORT_TYPE ends on 'INIT'.
        /// </summary>
        /// <returns>A Health Report, complete with system info on CPU, Network and RAM.</returns>
        List<HealthReportEntry> QueryHealthReport(DateTime minDate);

        /// <summary>
        /// Retrieves all entries from the ENGINE_PROPERTIES table of the state database.
        /// </summary>
        /// <returns>A list of manager data.</returns>
        List<EnginePropertyEntry> QueryEngineProperties(DateTime minDate);
    }
}