using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Table("AUDIT_LOGINFO_TYPES")]
    public partial class AuditLoginfoType
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("TITLE")]
        [StringLength(500)]
        public string Title { get; set; }
        [Column("ENUMNAME")]
        [StringLength(500)]
        public string Enumname { get; set; }
        [Column("DESCRIPTION")]
        [StringLength(1999)]
        public string Description { get; set; }
    }
}
