using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Keyless]
    [Table("SEQUENCE_TRACKING")]
    public partial class SequenceTracking
    {
        [Column("MGR")]
        [StringLength(200)]
        public string Mgr { get; set; }
        [Column("START_SEQ_VAL")]
        public int? StartSeqVal { get; set; }
        [Column("END_SEQ_VAL")]
        public int? EndSeqVal { get; set; }
        [Column("TABLE_NAME")]
        [StringLength(400)]
        public string TableName { get; set; }
        [Column("COLUMN_NAME")]
        [StringLength(200)]
        public string ColumnName { get; set; }
    }
}
