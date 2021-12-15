using DashboardBackend.Database.Models;

namespace DashboardBackend.Database
{
    /// <summary>
    /// Contains method signatures to handle all interaction with the state database.
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Retrieves all entries in the AFSTEMNING table of the state database added after the specified date.
        /// </summary>
        /// <param name="minDate">A minimum date constraint for the returned objects.</param>
        /// <returns>The result of the query, ordered by their timestamp.</returns>
        List<AfstemningEntry> QueryAfstemninger(DateTime minDate);

        /// <summary>
        /// Retrieves all entries in the EXECUTIONS table of the state database added after the specified date.
        /// </summary>
        /// <param name="minDate">A minimum date constraint for the returned objects.</param>
        /// <returns>The result of the query, ordered by their timestamp.</returns>
        List<ExecutionEntry> QueryExecutions(DateTime minDate);

        /// <summary>
        /// Retrieves all entries in the LOGGING table of the state database added after the specified date.
        /// </summary>
        /// <param name="minDate">A minimum date constraint for the returned objects.</param>
        /// <returns>The result of the query, ordered by their timestamp.</returns>
        List<LoggingEntry> QueryLogMessages(DateTime minDate);

        /// <summary>
        /// Retrieves all entries from the LOGGING_CONTEXT table of the state database that match the specified execution ID.
        /// </summary>
        /// <returns>The result of the query.</returns>
        List<LoggingContextEntry> QueryLoggingContext(int executionId);

        /// <summary>
        /// Retrieves all entries in the HEALTH_REPORT table of the state database added after the specified date.
        /// </summary>
        /// <param name="minDate">A minimum date constraint for the returned objects.</param>
        /// <returns>The result of the query, ordered by their timestamp.</returns>
        List<HealthReportEntry> QueryHealthReport(DateTime minDate);

        /// <summary>
        /// Retrieves all entries from the ENGINE_PROPERTIES table of the state database added after the specified date.
        /// </summary>
        /// <param name="minDate">A minimum date constraint for the returned objects.</param>
        /// <returns>The result of the query, ordered by their timestamp.</returns>
        List<EnginePropertyEntry> QueryEngineProperties(DateTime minDate);
    }
}