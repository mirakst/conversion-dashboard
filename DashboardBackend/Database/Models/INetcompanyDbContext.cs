using Microsoft.EntityFrameworkCore;

namespace DashboardBackend.Database.Models
{
    public interface INetcompanyDbContext : IDisposable
    {
        DbSet<AfstemningEntry> Afstemnings { get; set; }
        DbSet<AuditFkError> AuditFkErrors { get; set; }
        DbSet<AuditLogerror> AuditLogerrors { get; set; }
        DbSet<AuditLoginfo> AuditLoginfos { get; set; }
        DbSet<AuditLoginfoType> AuditLoginfoTypes { get; set; }
        DbSet<ColumnValue> ColumnValues { get; set; }
        DbSet<DestTable> DestTables { get; set; }
        DbSet<EnginePropertyEntry> EngineProperties { get; set; }
        DbSet<ExecutionEntry> Executions { get; set; }
        DbSet<HealthReportEntry> HealthReports { get; set; }
        DbSet<LoggingEntry> Loggings { get; set; }
        DbSet<LoggingContext> LoggingContexts { get; set; }
        DbSet<ManagerEntry> Managers { get; set; }
        DbSet<ManagerTracking> ManagerTrackings { get; set; }
        DbSet<MigrationFile> MigrationFiles { get; set; }
        DbSet<SequenceTracking> SequenceTrackings { get; set; }
        DbSet<StatementColumn> StatementColumns { get; set; }
        DbSet<StatementJoin> StatementJoins { get; set; }
        DbSet<StatementTable> StatementTables { get; set; }
        DbSet<SysHousekeeping> SysHousekeepings { get; set; }
        DbSet<SysHousekeepingUuid> SysHousekeepingUuids { get; set; }
        DbSet<TableLog> TableLogs { get; set; }
        DbSet<VEngineProperty> VEngineProperties { get; set; }
        DbSet<VoteCombination> VoteCombinations { get; set; }
        DbSet<VoteResult> VoteResults { get; set; }
    }
}