using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Table("AUDIT_LOGERROR")]
    [Index(nameof(Id), nameof(Mgrname), nameof(Created), Name = "AUDIT_LOGERROR_ID_INDX")]
    [Index(nameof(Mgrname), Name = "AUDIT_LOGERROR_MGRNAME_INDX")]
    public partial class AuditLogerror
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
        [Required]
        [Column("SOURCEROWS")]
        [StringLength(4000)]
        public string Sourcerows { get; set; }
        [Column("MESSAGE")]
        [StringLength(4000)]
        public string Message { get; set; }
    }
}
