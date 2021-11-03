using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Keyless]
    [Table("DEST_TABLE")]
    public partial class DestTable
    {
        [Column("MGR")]
        [StringLength(400)]
        public string Mgr { get; set; }
        [Column("ID_PREFIX")]
        [StringLength(25)]
        public string IdPrefix { get; set; }
        [Column("TABLE_NAME")]
        [StringLength(400)]
        public string TableName { get; set; }
    }
}
