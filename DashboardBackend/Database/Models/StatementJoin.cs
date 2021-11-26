using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Keyless]
    [Table("STATEMENT_JOINS")]
    public partial class StatementJoin
    {
        [Column("MGR")]
        [StringLength(400)]
        public string Mgr { get; set; }
        [Column("IDENTIFIER")]
        [StringLength(400)]
        public string Identifier { get; set; }
        [Column("IDENTIFIER_SHORT")]
        [StringLength(60)]
        public string IdentifierShort { get; set; }
        [Column("SCHEMA_NAME_1")]
        [StringLength(60)]
        public string SchemaName1 { get; set; }
        [Column("SCHEMA_NAME_FULL_1")]
        [StringLength(60)]
        public string SchemaNameFull1 { get; set; }
        [Column("TABLE_NAME_1")]
        [StringLength(400)]
        public string TableName1 { get; set; }
        [Column("COLUMN_NAME_1")]
        [StringLength(40)]
        public string ColumnName1 { get; set; }
        [Column("SCHEMA_NAME_2")]
        [StringLength(60)]
        public string SchemaName2 { get; set; }
        [Column("SCHEMA_NAME_FULL_2")]
        [StringLength(60)]
        public string SchemaNameFull2 { get; set; }
        [Column("TABLE_NAME_2")]
        [StringLength(400)]
        public string TableName2 { get; set; }
        [Column("COLUMN_NAME_2")]
        [StringLength(40)]
        public string ColumnName2 { get; set; }
        [Column("SCHEMA_NAME_3")]
        [StringLength(60)]
        public string SchemaName3 { get; set; }
        [Column("SCHEMA_NAME_FULL_3")]
        [StringLength(60)]
        public string SchemaNameFull3 { get; set; }
        [Column("TABLE_NAME_3")]
        [StringLength(400)]
        public string TableName3 { get; set; }
        [Column("COLUMN_NAME_3")]
        [StringLength(40)]
        public string ColumnName3 { get; set; }
        [Column("CRITERION")]
        [StringLength(400)]
        public string Criterion { get; set; }
        [Column("CREATED", TypeName = "datetime")]
        public DateTime? Created { get; set; }
    }
}
