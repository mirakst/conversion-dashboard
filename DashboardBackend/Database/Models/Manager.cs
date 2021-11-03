using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Keyless]
    [Table("MANAGERS")]
    public partial class Manager
    {
        [Column("MANAGER_NAME")]
        [StringLength(500)]
        public string ManagerName { get; set; }
        [Column("ROW_ID")]
        public int? RowId { get; set; }
        [Column("EXECUTIONS_ID")]
        public long? ExecutionsId { get; set; }
    }
}
