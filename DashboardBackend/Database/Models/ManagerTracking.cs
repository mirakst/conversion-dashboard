using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Keyless]
    [Table("MANAGER_TRACKING")]
    public partial class ManagerTracking
    {
        [Column("MGR")]
        [StringLength(400)]
        public string Mgr { get; set; }
        [Column("STATUS")]
        [StringLength(400)]
        public string Status { get; set; }
        [Column("RUNTIME")]
        public int? Runtime { get; set; }
        [Column("PERFORMANCECOUNTROWSREAD")]
        public int? Performancecountrowsread { get; set; }
        [Column("PERFORMANCECOUNTROWSWRITTEN")]
        public int? Performancecountrowswritten { get; set; }
        [Column("STARTTIME", TypeName = "datetime")]
        public DateTime? Starttime { get; set; }
        [Column("ENDTIME", TypeName = "datetime")]
        public DateTime? Endtime { get; set; }
        [Column("WEEK")]
        public int? Week { get; set; }
    }
}
