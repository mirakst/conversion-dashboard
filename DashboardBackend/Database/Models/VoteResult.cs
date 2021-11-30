using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Keyless]
    [Table("VOTE_RESULT")]
    public partial class VoteResult
    {
        [Required]
        [Column("ID")]
        [StringLength(400)]
        public string Id { get; set; }
        [Required]
        [Column("VOTE_NAME")]
        [StringLength(400)]
        public string VoteName { get; set; }
        [Required]
        [Column("PASSED")]
        [StringLength(1)]
        public string Passed { get; set; }
        [Column("VOTE_MESSAGE")]
        [StringLength(4000)]
        public string VoteMessage { get; set; }
        [Column("CREATED", TypeName = "datetime")]
        public DateTime Created { get; set; }
    }
}
