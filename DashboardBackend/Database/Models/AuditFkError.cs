using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Table("AUDIT_FK_ERRORS")]
    [Index(nameof(Tablename), Name = "INDX_ERROR_ROWS_TNAME")]
    public partial class AuditFkError
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("TABLENAME")]
        [StringLength(400)]
        public string Tablename { get; set; }
        [Column("FOREIGN_KEY_VIOLATED")]
        [StringLength(400)]
        public string ForeignKeyViolated { get; set; }
        [Column("ROWDATA")]
        public string Rowdata { get; set; }
    }
}
