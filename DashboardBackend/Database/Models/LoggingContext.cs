using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Keyless]
    [Table("LOGGING_CONTEXT")]
    public partial class LoggingContext
    {
        [Column("CONTEXT_ID")]
        public long ContextId { get; set; }
        [Column("EXECUTION_ID")]
        public long? ExecutionId { get; set; }
        [Column("CONTEXT")]
        public string Context { get; set; }
    }
}
