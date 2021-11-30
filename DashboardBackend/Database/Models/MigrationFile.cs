using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Table("MIGRATION_FILE")]
    public partial class MigrationFile
    {
        [Key]
        [Column("ID")]
        [StringLength(400)]
        public string Id { get; set; }
        [Column("CONTENT")]
        public byte[] Content { get; set; }
        [Required]
        [Column("FILE_NAME")]
        [StringLength(400)]
        public string FileName { get; set; }
        [Column("RELATIVE_PATH")]
        [StringLength(400)]
        public string RelativePath { get; set; }
        [Column("JENKINS_JOB")]
        [StringLength(400)]
        public string JenkinsJob { get; set; }
        [Column("TITLE")]
        [StringLength(400)]
        public string Title { get; set; }
        [Column("CREATED", TypeName = "datetime")]
        public DateTime? Created { get; set; }
    }
}
