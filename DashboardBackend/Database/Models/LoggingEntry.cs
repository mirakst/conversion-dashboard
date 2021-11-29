using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Keyless]
    [Table("LOGGING")]
    public partial class LoggingEntry
    {
        [Column("CREATED", TypeName = "datetime")]
        public DateTime? Created { get; set; }
        [Column("LOG_MESSAGE")]
        public string LogMessage { get; set; }
        [Column("LOG_LEVEL")]
        [StringLength(80)]
        public string LogLevel { get; set; }
        [Column("EXECUTION_ID")]
        public long? ExecutionId { get; set; }
        [Column("CONTEXT_ID")]
        public long? ContextId { get; set; }
    }
}
