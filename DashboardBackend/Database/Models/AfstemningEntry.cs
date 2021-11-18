using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Table("AFSTEMNING")]
    public partial class AfstemningEntry
    {
        [Key]
        [Column("ID")]
        [StringLength(400)]
        public string Id { get; set; }
        [Column("AFSTEMTDATO", TypeName = "datetime")]
        public DateTime Afstemtdato { get; set; }
        [Required]
        [Column("DESCRIPTION")]
        [StringLength(4000)]
        public string Description { get; set; }
        [Column("MANAGER")]
        [StringLength(400)]
        public string Manager { get; set; }
        [Column("CONTEXT")]
        [StringLength(400)]
        public string Context { get; set; }
        [Column("SRCANTAL")]
        public int? Srcantal { get; set; }
        [Column("DSTANTAL")]
        public int? Dstantal { get; set; }
        [Column("CUSTOMANTAL")]
        public int? Customantal { get; set; }
        [Column("AFSTEMRESULTAT")]
        [StringLength(4000)]
        public string Afstemresultat { get; set; }
        [Column("RUN_JOB")]
        [StringLength(400)]
        public string RunJob { get; set; }
        [Column("TOOLKIT_ID")]
        public int? ToolkitId { get; set; }
        [Column("SRC_SQL_COST")]
        public int? SrcSqlCost { get; set; }
        [Column("DST_SQL_COST")]
        public int? DstSqlCost { get; set; }
        [Column("CUSTOM_SQL_COST")]
        public int? CustomSqlCost { get; set; }
        [Column("SRC_SQL")]
        [StringLength(4000)]
        public string SrcSql { get; set; }
        [Column("DST_SQL")]
        [StringLength(4000)]
        public string DstSql { get; set; }
        [Column("CUSTOM_SQL")]
        [StringLength(4000)]
        public string CustomSql { get; set; }
        [Column("SRC_SQL_TIME")]
        public int? SrcSqlTime { get; set; }
        [Column("DST_SQL_TIME")]
        public int? DstSqlTime { get; set; }
        [Column("CUSTOM_SQL_TIME")]
        public int? CustomSqlTime { get; set; }
        [Column("START_TIME", TypeName = "datetime")]
        public DateTime? StartTime { get; set; }
        [Column("END_TIME", TypeName = "datetime")]
        public DateTime? EndTime { get; set; }
        [Column("AFSTEMNINGSDATA")]
        public byte[] Afstemningsdata { get; set; }
    }
}
