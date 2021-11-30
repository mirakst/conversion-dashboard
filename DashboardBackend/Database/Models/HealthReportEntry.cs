using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Keyless]
    [Table("HEALTH_REPORT")]
    public partial class HealthReportEntry
    {
        [Column("ROW_NO")]
        public int? RowNo { get; set; }
        [Column("MONITOR_NO")]
        public int? MonitorNo { get; set; }
        [Column("EXECUTION_ID")]
        public int? ExecutionId { get; set; }
        [Column("REPORT_TYPE")]
        [StringLength(400)]
        public string ReportType { get; set; }
        [Column("REPORT_KEY")]
        public string ReportKey { get; set; }
        [Column("REPORT_STRING_VALUE")]
        public string ReportStringValue { get; set; }
        [Column("REPORT_NUMERIC_VALUE")]
        public long? ReportNumericValue { get; set; }
        [Column("REPORT_VALUE_TYPE")]
        [StringLength(400)]
        public string ReportValueType { get; set; }
        [Column("REPORT_VALUE_HUMAN")]
        [StringLength(250)]
        public string ReportValueHuman { get; set; }
        [Column("LOG_TIME", TypeName = "datetime")]
        public DateTime? LogTime { get; set; }
    }
}
