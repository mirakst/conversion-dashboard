using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Keyless]
    public partial class VEngineProperty
    {
        [Column("MANAGER")]
        [StringLength(200)]
        public string Manager { get; set; }
        [Column("KEY")]
        [StringLength(400)]
        public string Key { get; set; }
        [Column("VALUE")]
        [StringLength(400)]
        public string Value { get; set; }
        [Column("RUN_NO")]
        public int? RunNo { get; set; }
        [Column("TIMESTAMP", TypeName = "datetime")]
        public DateTime? Timestamp { get; set; }
    }
}
