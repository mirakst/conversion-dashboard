using DashboardBackend.Database;
using DashboardBackend.Settings;
using Model;

namespace DashboardBackend
{
    /// <summary>
    /// Provides method signatures to get parsed data from the state database into models used in the Dashboard system.
    /// </summary>
    public interface IDataHandler
    {
        IDatabase Database { get; set; }

        /// <summary>
        /// Sets up the <see cref="Database"/> with the specified profile's connection string.
        /// </summary>
        /// <param name="profile">The profile to get the connection string from.</param>
        void SetupDatabase(Profile profile);

        /// <summary>
        /// Gets log entries from the state database that are fully parsed primarily into log messages, but when possible, also into executions and managers (with context and execution ID).
        /// </summary>
        /// <param name="minDate">The time of the last query for log data.</param>
        /// <returns>A tuple of log messages, managers and executions.</returns>
        Tuple<List<LogMessage>, List<Manager>, List<Execution>> GetParsedLogData(DateTime minDate);

        /// <summary>
        /// Gets fully parsed executions from the state database.
        /// </summary>
        /// <param name="minDate">The time of the last query for executions.</param>
        /// <returns>A list of executions.</returns>
        List<Execution> GetParsedExecutions(DateTime minDate);

        /// <summary>
        /// Gets parsed managers from the state database, which do not have a context or execution ID.
        /// </summary>
        /// <param name="minDate">The time of the last query for managers.</param>
        /// <returns>A list of managers without a context or execution ID.</returns>
        List<Manager> GetParsedManagers(DateTime minDate);

        /// <summary>
        /// Gets parsed reconciliations from the state database.
        /// </summary>
        /// <param name="minDate">The time of the last query for reconciliations.</param>
        /// <returns>A list of reconciliations.</returns>
        List<Reconciliation> GetParsedReconciliations(DateTime minDate);

        /// <summary>
        /// Gets the estimated number of managers for a given execution.
        /// </summary>
        /// <param name="executionId">The execution to find an estimate for.</param>
        /// <returns>The estimate.</returns>
        int GetEstimatedManagerCount(int executionId);

        /// <summary>
        /// Gets a Health Report merged from the specified one and the newly parsed data from the state database.
        /// </summary>
        /// <param name="minDate">The time of the last query for Health Report entries.</param>
        /// <param name="healthReport">The currently active Health Report.</param>
        /// <returns>A fully parsed Health Report.</returns>
        HealthReport GetParsedHealthReport(DateTime minDate, HealthReport healthReport);
    }
}
