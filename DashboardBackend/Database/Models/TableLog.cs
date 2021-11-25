using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Keyless]
    [Table("TABLE_LOG")]
    public partial class TableLog
    {
        [Required]
        [Column("ID")]
        [StringLength(400)]
        public string Id { get; set; }
        [Required]
        [Column("SCHEMA_NAME")]
        [StringLength(400)]
        public string SchemaName { get; set; }
        [Required]
        [Column("TABLE_NAME")]
        [StringLength(400)]
        public string TableName { get; set; }
        [Required]
        [Column("STATE")]
        [StringLength(400)]
        public string State { get; set; }
        [Column("CREATED", TypeName = "datetime")]
        public DateTime? Created { get; set; }
    }
}
