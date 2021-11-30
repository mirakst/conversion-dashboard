using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Keyless]
    [Table("COLUMN_VALUE")]
    public partial class ColumnValue
    {
        [Required]
        [Column("ID")]
        [StringLength(400)]
        public string Id { get; set; }
        [Required]
        [Column("COLUMN_NAME")]
        [StringLength(400)]
        public string ColumnName { get; set; }
        [Required]
        [Column("VALUE")]
        [StringLength(400)]
        public string Value { get; set; }
        [Column("CREATED", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Required]
        [Column("VOTE_COMBINATION_FK")]
        [StringLength(400)]
        public string VoteCombinationFk { get; set; }
    }
}
