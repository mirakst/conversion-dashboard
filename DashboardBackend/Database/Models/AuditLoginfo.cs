using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Table("AUDIT_LOGINFO")]
    [Index(nameof(Businessid), Name = "AUDIT_LOGINFO_BUSINESSID_INDX")]
    [Index(nameof(Cprnr), Name = "AUDIT_LOGINFO_CPRNR_INDX")]
    [Index(nameof(Id), nameof(Mgrname), nameof(Created), Name = "AUDIT_LOGINFO_ID_INDX")]
    [Index(nameof(Mgrname), Name = "AUDIT_LOGINFO_MGRNAME_INDX")]
    public partial class AuditLoginfo
    {
        [Key]
        [Column("ID")]
        [StringLength(400)]
        public string Id { get; set; }
        [Required]
        [Column("MGRNAME")]
        [StringLength(200)]
        public string Mgrname { get; set; }
        [Column("CREATED", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column("BUSINESSID")]
        public int? Businessid { get; set; }
        [Column("CPRNR")]
        [StringLength(400)]
        public string Cprnr { get; set; }
        [Column("MESSAGE")]
        [StringLength(1999)]
        public string Message { get; set; }
        [Column("RECONCILIATIONVALUE")]
        public int? Reconciliationvalue { get; set; }
    }
}
