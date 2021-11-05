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
        List<Execution> GetExecutions(DateTime minDate = default(DateTime));

        /// <summary>
        /// Retrieves all entries in the EXECUTION table of the state database.
        /// </summary>
        /// <returns>A list of all Executions</returns>
        List<ValidationTest> GetAfstemninger(DateTime minDate = default(DateTime));

        List<LogMessage> GetLogMessages(int ExecutionId, DateTime minDate = default(DateTime));
        List<Manager> GetManagers();
        HealthReport GetHealthReport();
        List<CpuLoad> GetCpuReadings(DateTime minDate = default(DateTime));
        List<NetworkUsage> GetNetworkReadings(DateTime minDate = default(DateTime));
        List<RamUsage> GetRamReadings(DateTime minDate = default(DateTime));
    }
}