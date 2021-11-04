using Microsoft.EntityFrameworkCore;
using System;
using DashboardBackend.Database.Models;
using Model;

namespace DashboardBackend.Database
{
    /// <summary>
    /// Contains utilities to handles all interaction with the state database.
    /// </summary>
    public interface IDatabaseHandler
    {
        /// <summary>
        /// Retrieves entries in the EXECUTION table of the state database added after the specified DateTime.
        /// </summary>
        /// <param name="minDate">A date constraint for the returned objects</param>
        /// <returns>A list of Executions no older than the specified DateTime</returns>
        List<Execution> GetExecutions(DateTime minDate);

        /// <summary>
        /// Retrieves all entries in the EXECUTION table of the state database.
        /// </summary>
        /// <returns>A list of all Executions</returns>
        List<Execution> GetExecutions();

        List<ValidationTest> GetAfstemninger(DateTime minDate);
        List<ValidationTest> GetAfstemninger();

        List<LogMessage> GetLogMessages(DateTime minDate);
        List<LogMessage> GetLogMessages();
        List<Manager> GetManagers();
        HealthReport GetHealthReport();
        List<CpuLoad> GetCpuReadings();
        List<NetworkUsage> GetNetworkReadings();
    }
}