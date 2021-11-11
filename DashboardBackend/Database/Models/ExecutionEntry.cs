using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Keyless]
    [Table("EXECUTIONS")]
    public partial class ExecutionEntry : IDatabaseEntry
    {
        [Column("EXECUTION_ID")]
        public long? ExecutionId { get; set; }
        [Column("EXECUTION_UUID")]
        [StringLength(400)]
        public string ExecutionUuid { get; set; }
        [Column("CREATED", TypeName = "datetime")]
        public DateTime? Created { get; set; }

        public DateTime Date => (DateTime)Created;
    }
}
