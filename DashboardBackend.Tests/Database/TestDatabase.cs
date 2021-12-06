using DashboardBackend.Database;
using DashboardBackend.Database.Models;
using System;
using System.Collections.Generic;

namespace DashboardBackend.Tests.Database
{
    public class TestDatabase : IDatabaseHandler
    {
        public List<AfstemningEntry> QueryAfstemninger(DateTime minDate)
        {
            return new List<AfstemningEntry>()
            {
                new AfstemningEntry()
                {
                    Id = string.Empty,
                    Afstemtdato = DateTime.Parse("01-01-2020 12:00:00"),
                    Description = "validationOne",
                    Manager = "managerOne",
                    Context =  String.Empty,
                    Srcantal = 0,
                    Dstantal = 0,
                    Customantal = null,
                    Afstemresultat = "OK",
                    RunJob = String.Empty,
                    ToolkitId = 0,
                    SrcSqlCost = null,
                    DstSqlCost = null,
                    CustomSqlCost = null,
                    SrcSql = "srcSql",
                    DstSql = "dstSql",
                    CustomSql = String.Empty,
                    SrcSqlTime = null,
                    DstSqlTime = null,
                    CustomSqlTime = null,
                    StartTime = null,
                    EndTime = null,
                    Afstemningsdata = Array.Empty<byte>(),
                },
                new AfstemningEntry()
                {
                    Id = string.Empty,
                    Afstemtdato = DateTime.Parse("01-01-2020 10:00:00"),
                    Description = "validationOne",
                    Manager = "managerOne",
                    Context =  String.Empty,
                    Srcantal = 0,
                    Dstantal = 0,
                    Customantal = null,
                    Afstemresultat = "OK",
                    RunJob = String.Empty,
                    ToolkitId = 0,
                    SrcSqlCost = null,
                    DstSqlCost = null,
                    CustomSqlCost = null,
                    SrcSql = "srcSql",
                    DstSql = "dstSql",
                    CustomSql = String.Empty,
                    SrcSqlTime = null,
                    DstSqlTime = null,
                    CustomSqlTime = null,
                    StartTime = null,
                    EndTime = null,
                    Afstemningsdata = Array.Empty<byte>(),
                },
                new AfstemningEntry()
                {
                    Id = string.Empty,
                    Afstemtdato = DateTime.Parse("01-01-2020 08:00:00"),
                    Description = "validationThrowTest",
                    Manager = "managerOne",
                    Context =  String.Empty,
                    Srcantal = 0,
                    Dstantal = 0,
                    Customantal = null,
                    Afstemresultat = "NOGET ANDET",
                    RunJob = String.Empty,
                    ToolkitId = 0,
                    SrcSqlCost = null,
                    DstSqlCost = null,
                    CustomSqlCost = null,
                    SrcSql = "srcSql",
                    DstSql = "dstSql",
                    CustomSql = String.Empty,
                    SrcSqlTime = null,
                    DstSqlTime = null,
                    CustomSqlTime = null,
                    StartTime = null,
                    EndTime = null,
                    Afstemningsdata = Array.Empty<byte>(),
                },
                new AfstemningEntry()
                {
                    Id = string.Empty,
                    Afstemtdato = DateTime.Parse("01-01-2020 13:00:00"),
                    Description = "validationTwo",
                    Manager = "managerTwo",
                    Context =  String.Empty,
                    Srcantal = 0,
                    Dstantal = 0,
                    Customantal = null,
                    Afstemresultat = "OK",
                    RunJob = String.Empty,
                    ToolkitId = 0,
                    SrcSqlCost = null,
                    DstSqlCost = null,
                    CustomSqlCost = null,
                    SrcSql = "srcSql",
                    DstSql = "dstSql",
                    CustomSql = String.Empty,
                    SrcSqlTime = null,
                    DstSqlTime = null,
                    CustomSqlTime = null,
                    StartTime = null,
                    EndTime = null,
                    Afstemningsdata = Array.Empty<byte>(),
                }
            }.FindAll(e => e.Afstemtdato >= minDate);
        }

        public List<ExecutionEntry> QueryExecutions(DateTime minDate)
        {
            return new List<ExecutionEntry>()
            {
                new ExecutionEntry()
                {
                    ExecutionId =  1,
                    ExecutionUuid = "conversionOneCopy",
                    Created = DateTime.Parse("01-01-2020 10:00:00"),
                },
                new ExecutionEntry()
                {
                    ExecutionId =  1,
                    ExecutionUuid = "conversionOne",
                    Created = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new ExecutionEntry()
                {
                    ExecutionId =  2,
                    ExecutionUuid = "conversionTwo",
                    Created = DateTime.Parse("01-01-2020 13:00:00"),
                }
            }.FindAll(e => e.Date >= minDate);
        }

        public List<LoggingEntry> QueryLogMessages(int executionId, DateTime minDate)
        {
            return new List<LoggingEntry>()
            {
                new LoggingEntry()
                {
                    Created = DateTime.Parse("01-01-2020 12:00:00"),
                    LogMessage = "Info",
                    LogLevel = "Info",
                    ExecutionId = 0,
                    ContextId = 0,
                },
                new LoggingEntry()
                {
                    Created = DateTime.Parse("01-01-2020 13:00:00"),
                    LogMessage = "Warning",
                    LogLevel = "Warning",
                    ExecutionId = 0,
                    ContextId = 1,
                }
            }.FindAll(e => e.Created >= minDate);
        }

        public List<LoggingEntry> QueryLogMessages(DateTime minDate)
        {
            return new List<LoggingEntry>()
            {
                new LoggingEntry()
                {
                    Created = DateTime.Parse("01-01-2020 17:00:00"),
                    LogMessage = "Afstemning Error",
                    LogLevel = "ERROR",
                    ExecutionId = 0,
                    ContextId = 0,
                },
                new LoggingEntry()
                {
                    Created = DateTime.Parse("01-01-2020 18:00:00"),
                    LogMessage = "Check - Error",
                    LogLevel = "INFO",
                    ExecutionId = 0,
                    ContextId = 0,
                },
                new LoggingEntry()
                {
                    Created = DateTime.Parse("01-01-2020 12:00:00"),
                    LogMessage = "Info",
                    LogLevel = "INFO",
                    ExecutionId = 0,
                    ContextId = 0,
                },
                new LoggingEntry()
                {
                    Created = DateTime.Parse("01-01-2020 13:00:00"),
                    LogMessage = "Warning",
                    LogLevel = "WARN",
                    ExecutionId = 0,
                    ContextId = 0,
                },
                new LoggingEntry()
                {
                    Created = DateTime.Parse("01-01-2020 14:00:00"),
                    LogMessage = "Error",
                    LogLevel = "ERROR",
                    ExecutionId = 0,
                    ContextId = 0,
                },
                new LoggingEntry()
                {
                    Created = DateTime.Parse("01-01-2020 15:00:00"),
                    LogMessage = "Fatal",
                    LogLevel = "FATAL",
                    ExecutionId = 0,
                    ContextId = 0,
                },
                new LoggingEntry()
                {
                    Created = DateTime.Parse("01-01-2020 16:00:00"),
                    LogMessage = "None",
                    LogLevel = string.Empty,
                    ExecutionId = 0,
                    ContextId = 0,
                },
            }.FindAll(e => e.Created >= minDate);
        }

        public List<LoggingContextEntry> QueryLoggingContext(int executionId)
        {
            return new List<LoggingContextEntry>()
            {
                new LoggingContextEntry()
                {
                    Context = string.Empty,
                    ContextId = 0,
                    ExecutionId= 0
                },
                new LoggingContextEntry()
                {
                    Context = string.Empty,
                    ContextId = 1,
                    ExecutionId= 0
                },
                new LoggingContextEntry()
                {
                    Context = string.Empty,
                    ContextId = 1,
                    ExecutionId= 1
                }
            }.FindAll(e => e.ExecutionId == executionId);
        }

        public List<HealthReportEntry> QueryHealthReport()
        {
            return new List<HealthReportEntry>()
            {
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = String.Empty,
                    ReportKey = "Hostname",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = null,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = null,
                    ReportKey = "Monitor Name",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = null,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = String.Empty,
                    ReportKey = "CPU Name",
                    ReportStringValue = "CPU",
                    ReportNumericValue = null,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = String.Empty,
                    ReportKey = "PhysicalCores",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 100,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = String.Empty,
                    ReportKey = "CPU Max frewuency",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 100,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = String.Empty,
                    ReportKey = "Total",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 100,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = String.Empty,
                    ReportKey = "Interface 0: Name",
                    ReportStringValue = "Interface",
                    ReportNumericValue = null,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = String.Empty,
                    ReportKey = "Interface 0: MAC address",
                    ReportStringValue = "MAC address",
                    ReportNumericValue = null,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = String.Empty,
                    ReportKey = "Interface 0; Speed",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 100,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
            };
        }

        public List<EnginePropertyEntry> QueryEngineProperties(DateTime minDate)
        {
            return new List<EnginePropertyEntry>()
            {
                new EnginePropertyEntry()
                {
                    Manager = "ManagerOne",
                    Key = "START_TIME",
                    Value = "01-01-2020 12:00:00",
                    Timestamp = DateTime.Parse("01-01-2020 12:00:00"),
                    RunNo = null,
                },
                new EnginePropertyEntry()
                {
                    Manager = "ManagerTwo",
                    Key = String.Empty,
                    Value = String.Empty,
                    Timestamp = DateTime.Parse("01-01-2020 12:00:00"),
                    RunNo = null,
                },
                new EnginePropertyEntry()
                {
                    Manager = "ManagerThree,longassboithatisnotgoingtoshowitsfullname",
                    Key = String.Empty,
                    Value = String.Empty,
                    Timestamp = DateTime.Parse("01-01-2020 12:00:00"),
                    RunNo = null,
                },
                new EnginePropertyEntry()
                {
                    Manager = "ManagerOne",
                    Key = "Læste rækker",
                    Value = "69420",
                    Timestamp = DateTime.Parse("01-01-2020 12:00:00"),
                    RunNo = null,
                },
                new EnginePropertyEntry()
                {
                    Manager = "ManagerOne",
                    Key = "END_TIME",
                    Value = "01-01-2020 13:00:00",
                    Timestamp = DateTime.Parse("01-01-2020 13:00:00"),
                    RunNo = null,
                },
                new EnginePropertyEntry()
                {
                    Manager = "ManagerOne",
                    Key = "Skrevne rækker",
                    Value = "69420",
                    Timestamp = DateTime.Parse("01-01-2020 13:00:00"),
                    RunNo = null,
                }
            }.FindAll(e => e.Timestamp >= minDate);
        }

        public List<HealthReportEntry> QueryPerformanceReadings(DateTime minDate)
        {
            return new List<HealthReportEntry>()
            {
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "CPU",
                    ReportKey = String.Empty,
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 10,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "MEMORY",
                    ReportKey = String.Empty,
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 10,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "CPU",
                    ReportKey = String.Empty,
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 20,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 13:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "MEMORY",
                    ReportKey = String.Empty,
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 20,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 13:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Send",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 10,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Send (Delta)",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 10,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Send (Speed)",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 10,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Received",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 10,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Received (Delta)",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 10,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Received (Speed)",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 10,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 12:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Send",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 20,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 13:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Send (Delta)",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 20,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 13:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Send (Speed)",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 20,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 13:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Received",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 20,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 13:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Received (Delta)",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 20,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 13:00:00"),
                },
                new HealthReportEntry()
                {
                    RowNo = null,
                    MonitorNo = null,
                    ExecutionId = 1,
                    ReportType = "NETWORK",
                    ReportKey = "Interface 0: Bytes Received (Speed)",
                    ReportStringValue = String.Empty,
                    ReportNumericValue = 20,
                    ReportValueType = String.Empty,
                    ReportValueHuman = String.Empty,
                    LogTime = DateTime.Parse("01-01-2020 13:00:00"),
                }
            }.FindAll(e => e.LogTime >= minDate);
        }

        public List<LoggingContextEntry> QueryManagers()
        {
            return new List<LoggingContextEntry>()
            {
                new LoggingContextEntry()
                {
                    ContextId = 1,
                    ExecutionId = 1,
                    Context = "ManagerOne",
                },
                new LoggingContextEntry()
                {
                    ContextId = 2,
                    ExecutionId = 1,
                    Context = "ManagerTwo",
                },
                new LoggingContextEntry()
                {
                    ContextId = 3,
                    ExecutionId = 1,
                    Context = "ManagerThree,longassnamethatisnotshown",
                }
            };
        }
    }
}
