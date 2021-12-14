using DashboardBackend.Database.Models;
using Model;

namespace DashboardBackend.Parsers
{
    public class HealthReportParser : IDataParser<HealthReportEntry, HealthReport>
    {
        public HealthReport Parse(List<HealthReportEntry> data)
        {
            HealthReport result = new();
            foreach (var entry in data)
            {
                switch (entry.ReportType)
                {
                    #region Host info
                    case "INIT" when entry.ReportKey == "Hostname":
                        result.HostName = entry.ReportStringValue;
                        break;
                    case "INIT" when entry.ReportKey == "Monitor Name":
                        result.MonitorName = entry.ReportStringValue;
                        break;
                    #endregion
                    #region CPU info
                    case "CPU_INIT" when entry.ReportKey == "CPU Name":
                        result.Cpu.Name = entry.ReportStringValue;
                        break;
                    case "CPU_INIT" when entry.ReportKey == "PhysicalCores":
                        result.Cpu.Cores = Convert.ToInt32(entry.ReportNumericValue ?? 0);
                        break;
                    case "CPU_INIT" when entry.ReportKey == "CPU Max frequency":
                        result.Cpu.MaxFrequency = Convert.ToInt64(entry.ReportNumericValue ?? 0);
                        break;
                    #endregion
                    #region CPU data
                    case "CPU" when entry.ReportKey == "LOAD":
                        result.Cpu.Readings.Add(GetCpuReading(entry));
                        break;
                    #endregion
                    #region RAM info
                    case "MEMORY_INIT" when entry.ReportKey == "TOTAL":
                        result.Ram.Total = entry.ReportNumericValue;
                        break;
                    #endregion
                    #region RAM data
                    case "MEMORY" when entry.ReportKey == "AVAILABLE":
                        result.Ram.AddReading(GetRamReading(entry));
                        break;
                    #endregion
                    #region Network info
                    case "NETWORK_INIT" when entry.ReportKey == "Interface 0: Name":
                        result.Network.Name = entry.ReportStringValue;
                        break;
                    case "NETWORK_INIT" when entry.ReportKey == "Interface 0: MAC address":
                        result.Network.MacAddress = entry.ReportStringValue;
                        break;
                    case "NETWORK_INIT" when entry.ReportKey == "Interface 0: Speed":
                        result.Network.Speed = entry.ReportNumericValue ?? 0;
                        break;
                    #endregion
                    #region Network data
                    case "NETWORK" when entry.ReportKey == "Interface 0: Bytes Send":
                        var reading = result.Network.Readings.FirstOrDefault(r => r.BytesSend is null && r.Date.Subtract(entry.LogTime.Value) < TimeSpan.FromSeconds(1));
                        if (reading is null)
                        {
                            reading = new()
                            {
                                ExecutionId = entry.ExecutionId.Value,
                                Date = entry.LogTime.Value,
                            };
                            result.Network.Readings.Add(reading);
                        }
                        reading.BytesSend = entry.ReportNumericValue;
                        break;
                    case "NETWORK" when entry.ReportKey == "Interface 0: Bytes Received":
                        reading = result.Network.Readings.FirstOrDefault(r => r.BytesReceived is null && r.Date.Subtract(entry.LogTime.Value) < TimeSpan.FromSeconds(1));
                        if (reading is null)
                        {
                            reading = new()
                            {
                                ExecutionId = entry.ExecutionId.Value,
                                Date = entry.LogTime.Value,
                            };
                            result.Network.Readings.Add(reading);
                        }
                        reading.BytesReceived = entry.ReportNumericValue;
                        break;
                    case "NETWORK" when entry.ReportKey == "Interface 0: Bytes Send (Delta)":
                        reading = result.Network.Readings.FirstOrDefault(r => r.BytesSendDelta is null && r.Date.Subtract(entry.LogTime.Value) < TimeSpan.FromSeconds(1));
                        if (reading is null)
                        {
                            reading = new()
                            {
                                ExecutionId = entry.ExecutionId.Value,
                                Date = entry.LogTime.Value,
                            };
                            result.Network.Readings.Add(reading);
                        }
                        reading.BytesSendDelta = entry.ReportNumericValue;
                        break;
                    case "NETWORK" when entry.ReportKey == "Interface 0: Bytes Received (Delta)":
                        reading = result.Network.Readings.FirstOrDefault(r => r.BytesReceivedDelta is null && r.Date.Subtract(entry.LogTime.Value) < TimeSpan.FromSeconds(1));
                        if (reading is null)
                        {
                            reading = new()
                            {
                                ExecutionId = entry.ExecutionId.Value,
                                Date = entry.LogTime.Value,
                            };
                            result.Network.Readings.Add(reading);
                        }
                        reading.BytesReceivedDelta = entry.ReportNumericValue.Value;
                        break;
                    case "NETWORK" when entry.ReportKey == "Interface 0: Bytes Send (Speed)":
                        reading = result.Network.Readings.FirstOrDefault(r => r.BytesSendSpeed is null && r.Date.Subtract(entry.LogTime.Value) < TimeSpan.FromSeconds(1));
                        if (reading is null)
                        {
                            reading = new()
                            {
                                ExecutionId = entry.ExecutionId.Value,
                                Date = entry.LogTime.Value,
                            };
                            result.Network.Readings.Add(reading);
                        }
                        reading.BytesSendSpeed = entry.ReportNumericValue.Value;
                        break;
                    case "NETWORK" when entry.ReportKey == "Interface 0: Bytes Received (Speed)":
                        reading = result.Network.Readings.FirstOrDefault(r => r.BytesReceivedSpeed is null && r.Date.Subtract(entry.LogTime.Value) < TimeSpan.FromSeconds(1));
                        if (reading is null)
                        {
                            reading = new()
                            {
                                ExecutionId = entry.ExecutionId.Value,
                                Date = entry.LogTime.Value,
                            };
                            result.Network.Readings.Add(reading);
                        }
                        reading.BytesReceivedSpeed = entry.ReportNumericValue.Value;
                        break;
                    #endregion
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// Creates a list of CPU Readings for the system model, which is returned.
        /// </summary>
        /// <param name="item">A state database entry with cpu load readings</param>
        /// <returns>A CPU load reading.</returns>
        private static CpuLoad GetCpuReading(HealthReportEntry item)
        {
            int executionId = item.ExecutionId.Value;
            double reportNumValue = Convert.ToDouble(item.ReportNumericValue) / 100;
            DateTime logTime = item.LogTime.Value;
            CpuLoad cpuReading = new(executionId, reportNumValue, logTime);
            return cpuReading;
        }

        /// <summary>
        /// Creates a list of RAM Readings for the system model, which is returned.
        /// </summary>
        /// <param name="item">A state database entry with cpu load readings</param>
        /// <returns>A RAM usage reading.</returns>
        private static RamLoad GetRamReading(HealthReportEntry item)
        {
            int executionId = item.ExecutionId.Value;
            long available = item.ReportNumericValue.Value;
            DateTime logTime = item.LogTime.Value;
            RamLoad ramReading = new(executionId, available, logTime);
            return ramReading;
        }

        /// <summary>
        /// Builds network usage readings by coupling network entries 6 at a time.
        /// </summary>
        /// <param name="data">A list of network usage entries from the state database.</param>
        /// <returns>A coupled list of network usage entries.</returns>
        private List<List<HealthReportEntry>> GetDistinctNetworkSets(List<HealthReportEntry> data)
        {
            List<List<HealthReportEntry>> result = new();
            for (int i = 0; i < data.Count; i += 6)
            {
                result.Add(data.Skip(i).Take(6).ToList());
            }
            return result;
        }
    }
}
